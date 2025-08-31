namespace Mp3Tagger.Core.Steps;

public interface ICoverDownloader
{
    ValueTask DownloadCoverAsync(Download download, OutputContext outputContext, CancellationToken cancellationToken);
}