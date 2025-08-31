using Id3;
using Id3.Frames;
using Microsoft.Extensions.Logging;

namespace Mp3Tagger.Core.Steps;

internal sealed class Mp3Tagger : IMp3Tagger
{
    private readonly ILogger<Mp3Tagger> _logger;

    public Mp3Tagger(ILogger<Mp3Tagger> logger)
    {
        _logger = logger;
    }

    public ValueTask CreateTagAsync(Download download, OutputContext outputContext, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating MP3 tag...");
        return CreateTagForFile(download, outputContext);
    }

    private ValueTask CreateTagForFile(Download download, OutputContext outputContext)
    {
        using var mp3 = new Mp3(outputContext.Mp3File, Mp3Permissions.ReadWrite);
        var tagToCreate = CreateTag(download, outputContext);
        
        mp3.WriteTag(tagToCreate);
        return new ValueTask(Task.CompletedTask);
    }

    private Id3Tag CreateTag(Download download, OutputContext outputContext)
    {
        var title = download.SongTitle;
        var artist = download.Artist;
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