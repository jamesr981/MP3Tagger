using Microsoft.Extensions.Logging;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;

namespace Mp3Tagger.Core.Steps;

internal sealed class CoverDownload : ICoverDownloader
{
    private readonly ILogger<CoverDownload> _logger;
    private readonly YoutubeClient _youtube;
    private readonly HttpClient _httpClient;

    public CoverDownload(ILogger<CoverDownload> logger, YoutubeClient youtube,
        HttpClient httpClient)
    {
        _logger = logger;
        _youtube = youtube;
        _httpClient = httpClient;
    }

    public async ValueTask DownloadCoverAsync(Download download, OutputContext outputContext, CancellationToken cancellationToken = default)
    {
        var coverUrl = GetCoverYoutubeVideoUrl(download);

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

    private static string GetCoverYoutubeVideoUrl(Download download)
    {
        return !string.IsNullOrWhiteSpace(download.CoverUrl) ? download.CoverUrl : download.Mp3Url;
    }
}