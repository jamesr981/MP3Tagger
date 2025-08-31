namespace Mp3Tagger.Core.Steps;

public interface IMp3Tagger
{
    ValueTask CreateTagAsync(Download download, OutputContext outputContext, CancellationToken cancellationToken);
}