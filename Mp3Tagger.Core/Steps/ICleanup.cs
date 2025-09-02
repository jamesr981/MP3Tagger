namespace Mp3Tagger.Core.Steps;

public interface ICleanup
{
    ValueTask CleanupAsync(OutputContext outputContext, CancellationToken cancellationToken);
}