namespace Mp3Tagger.Core.Steps;

public interface IMp3Downloader
{
    ValueTask DownloadMp3Async(Download download, OutputContext outputContext, CancellationToken cancellationToken);
}