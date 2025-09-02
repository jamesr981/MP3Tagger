using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YtdlpDotNet;

namespace Mp3Tagger.Core.Steps;

internal sealed class Mp3Download : IMp3Downloader
{
    private readonly Arguments _args;
    private readonly ILogger<Mp3Download> _logger;
    private readonly Ytdlp _ytdlp;
    private const string ExtensionTemplate = ".%(ext)s";

    public Mp3Download(IOptions<Arguments> args, ILogger<Mp3Download> logger, Ytdlp ytdlp)
    {
        _args = args.Value;
        _logger = logger;
        _ytdlp = ytdlp;
    }

    public async ValueTask DownloadMp3Async(Download download, OutputContext outputContext, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Downloading MP3 file...");
        var outputTemplate = GetOutputTemplate(download);
        try
        {
            _logger.LogWarning("Output folder: {OutputFolder}", outputContext.OutputFolder.FullName);
            _ytdlp.ExtractAudio("mp3")
                .AddCustomCommand("--force-ipv4")
                .SetFormat("bestaudio")
                .EmbedMetadata()
                .SetReferer(download.Mp3Url)
                .SetOutputFolder(outputContext.OutputFolder.FullName)
                .SetOutputTemplate(outputTemplate);

            if (!string.IsNullOrWhiteSpace(_args.FfmpegLocation))
            {
                _ytdlp.AddCustomCommand($"--ffmpeg-location {_args.FfmpegLocation}");
            }
            
            await _ytdlp.ExecuteAsync(download.Mp3Url, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to extract MP3 file.");
        }
    }

    private static string GetOutputTemplate(Download download)
    {
        return download.SongTitle + ExtensionTemplate;
    }
}