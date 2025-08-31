using Id3;
using Id3.Frames;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mp3Tagger.Steps;

internal sealed class Mp3Tagger
{
    private readonly Arguments _args;
    private readonly ILogger<Mp3Tagger> _logger;

    public Mp3Tagger(IOptions<Arguments> args, ILogger<Mp3Tagger> logger)
    {
        _args = args.Value;
        _logger = logger;
    }

    public async ValueTask CreateTagAsync(OutputContext outputContext, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating MP3 tag...");
        CreateTagForFile(outputContext);
        await Task.Delay(1, cancellationToken);
    }

    private void CreateTagForFile(OutputContext outputContext)
    {
        using var mp3 = new Mp3(outputContext.Mp3File, Mp3Permissions.ReadWrite);
        var tagToCreate = CreateTag(outputContext);
        mp3.WriteTag(tagToCreate);
    }

    private Id3Tag CreateTag(OutputContext outputContext)
    {
        var title = _args.OutputFileName;
        var artist = _args.Artist;
        var artistFrame = new ArtistsFrame();
        if (!string.IsNullOrWhiteSpace(artist))
        {
            artistFrame.Value.Add(artist);
        }

        var tag = new Id3Tag
        {
            Title = title,
            Album = title,
            Artists = artistFrame,
        };

        if (outputContext.Cover.Exists)
        {
            var cover = new PictureFrame
            {
                PictureType = PictureType.FrontCover
            };
        
            using var stream = outputContext.Cover.OpenRead();
            cover.LoadImage(stream);
            tag.Pictures.Add(cover);
        }
        
        return tag.ConvertTo(Id3Version.V23);
    }
}