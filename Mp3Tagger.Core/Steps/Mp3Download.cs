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
    private const string AudioOnly = "audio only";

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
            var formats = await _ytdlp.GetAvailableFormatsAsync(download.Mp3Url, cancellationToken);
            var audioFormat = formats.FirstOrDefault(z => z.Resolution == AudioOnly);
            if (audioFormat is null)
            {
                _logger.LogInformation("No audio format available.");
                return;
            }

            _ytdlp.ExtractAudio("mp3")
                .AddCustomCommand($"--ffmpeg-location {_args.FfmpegLocation}")
                .SetFormat(audioFormat.ID)
                .EmbedMetadata()
                .SetOutputFolder(outputContext.OutputFolder.FullName)
                .SetOutputTemplate(outputTemplate);
            
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