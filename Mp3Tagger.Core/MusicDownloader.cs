using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mp3Tagger.Core.Steps;

namespace Mp3Tagger.Core;

public sealed class MusicDownloader
{
    private readonly ILogger<MusicDownloader> _logger;
    private readonly IMp3Downloader _mp3Download;
    private readonly ICoverDownloader _coverDownload;
    private readonly IMp3Tagger _tagger;
    private readonly ICleanup _cleanup;
    private readonly Arguments _args;

    public MusicDownloader(IOptions<Arguments> args, ILogger<MusicDownloader> logger, IMp3Downloader mp3Download, ICoverDownloader coverDownload,
        IMp3Tagger tagger, ICleanup cleanup)
    {
        _logger = logger;
        _mp3Download = mp3Download;
        _coverDownload = coverDownload;
        _tagger = tagger;
        _cleanup = cleanup;
        _args = args.Value;
    }

    public async Task Download(Download download, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting MP3 Tagger...");
        var outputContext = GetOutputInfo(download);

        var mp3Task = _mp3Download.DownloadMp3Async(download, outputContext, cancellationToken).AsTask();
        var coverTask = _coverDownload.DownloadCoverAsync(download, outputContext, cancellationToken).AsTask();

        await Task.WhenAll(mp3Task, coverTask);

        await _tagger.CreateTagAsync(download, outputContext, cancellationToken);
        await _cleanup.CleanupAsync(outputContext, cancellationToken);
    }

    private OutputContext GetOutputInfo(Download download)
    {
        var outputFolder = Path.Combine(_args.OutputPath, download.SongTitle);

        var outputDirectoryInfo = Directory.CreateDirectory(outputFolder);
        var mp3Path = Path.Combine(outputFolder, $"{download.SongTitle}.mp3");
        var mp3Info = new FileInfo(mp3Path);

        var coverPath = Path.Combine(outputFolder, "cover.jpg");
        var coverInfo = new FileInfo(coverPath);

        var outputContext = new OutputContext
        {
            OutputFolder = outputDirectoryInfo,
            Mp3File = mp3Info,
            Cover = coverInfo
        };
        
        return outputContext;
    }
}