using Microsoft.Extensions.Logging;

namespace Mp3Tagger.Core.Steps;

internal sealed class Cleanup : ICleanup
{
    private readonly ILogger<Cleanup> _logger;

    public Cleanup(ILogger<Cleanup> logger)
    {
        _logger = logger;
    }

    public ValueTask CleanupAsync(OutputContext outputContext, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Cleaning up...");   
        File.Delete(outputContext.Cover.FullName);
        return ValueTask.CompletedTask;
    }
}