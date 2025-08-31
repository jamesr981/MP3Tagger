using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mp3Tagger.Steps;

namespace Mp3Tagger;

internal sealed class App
{
    private readonly ILogger<App> _logger;
    private readonly Mp3Download _mp3Download;
    private readonly CoverDownload _coverDownload;
    private readonly Steps.Mp3Tagger _tagger;
    private readonly Cleanup _cleanup;
    private readonly Arguments _args;

    public App(IOptions<Arguments> args, ILogger<App> logger, Mp3Download mp3Download, CoverDownload coverDownload,
        Steps.Mp3Tagger tagger, Cleanup cleanup)
    {
        _logger = logger;
        _mp3Download = mp3Download;
        _coverDownload = coverDownload;
        _tagger = tagger;
        _cleanup = cleanup;
        _args = args.Value;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting MP3 Tagger...");
        var outputContext = GetOutputInfo();

        var mp3Task = _mp3Download.DownloadMp3Async(outputContext, cancellationToken).AsTask();
        var coverTask = _coverDownload.DownloadCoverAsync(outputContext, cancellationToken).AsTask();

        await Task.WhenAll(mp3Task, coverTask);

        await _tagger.CreateTagAsync(outputContext, cancellationToken);
        await _cleanup.CleanupAsync(outputContext, cancellationToken);
    }

    private OutputContext GetOutputInfo()
    {
        var outputFolder = Path.Combine(_args.OutputPath, _args.OutputFileName);

        var outputDirectoryInfo = Directory.CreateDirectory(outputFolder);
        var mp3Path = Path.Combine(outputFolder, $"{_args.OutputFileName}.mp3");
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