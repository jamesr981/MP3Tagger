using Microsoft.Extensions.Logging;
using YtdlpDotNet;
using ILogger = YtdlpDotNet.ILogger;

namespace Mp3Tagger.Logging;

internal sealed class YtdlpLogger : ILogger
{
    private readonly ILogger<YtdlpLogger> _logger;

    public YtdlpLogger(ILogger<YtdlpLogger> logger)
    {
        _logger = logger;
    }

    public void Log(LogType type, string message)
    {
        switch (type)
        {
            case LogType.Debug:
                _logger.LogDebug(message);
                break;
            case LogType.Error:
                _logger.LogError(message);
                break;
            case LogType.Info:
                _logger.LogInformation(message);
                break;
            case LogType.Warning:
                _logger.LogWarning(message);
                break;
            default:
                _logger.LogDebug(message);
                break;
        }
    }
}