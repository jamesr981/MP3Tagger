using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;

namespace Mp3Tagger.Steps;

internal sealed class CoverDownload
{
    private readonly Arguments _args;
    private readonly ILogger<CoverDownload> _logger;
    private readonly YoutubeClient _youtube;
    private readonly HttpClient _httpClient;

    public CoverDownload(IOptions<Arguments> args, ILogger<CoverDownload> logger, YoutubeClient youtube,
        HttpClient httpClient)
    {
        _args = args.Value;
        _logger = logger;
        _youtube = youtube;
        _httpClient = httpClient;
    }

    public async ValueTask DownloadCoverAsync(OutputContext outputContext, CancellationToken cancellationToken = default)
    {
        var coverUrl = GetCoverYoutubeVideoUrl();

        _logger.LogInformation("Downloading cover from {CoverUrl}", coverUrl);

        var videoId = VideoId.TryParse(coverUrl);
        if (!videoId.HasValue)
        {
            _logger.LogError("Invalid CoverUrl");
            return;
        }

        var video = await _youtube.Videos.GetAsync(videoId.Value, cancellationToken);
        var thumbnail = video.Thumbnails.TryGetWithHighestResolution();
        if (thumbnail is null)
        {
            _logger.LogError("Unable to retrieve thumbnail");
            return;
        }
        
        var imageBytes = await _httpClient.GetByteArrayAsync(thumbnail.Url, cancellationToken);
        await File.WriteAllBytesAsync(outputContext.Cover.FullName, imageBytes, cancellationToken);
    }

    private string GetCoverYoutubeVideoUrl()
    {
        return !string.IsNullOrWhiteSpace(_args.CoverUrl) ? _args.CoverUrl : _args.Mp3Url;
    }
}