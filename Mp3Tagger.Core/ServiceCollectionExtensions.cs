using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mp3Tagger.Core.Logging;
using Mp3Tagger.Core.Steps;
using YoutubeExplode;
using YtdlpDotNet;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Mp3Tagger.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMp3Tagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<MusicDownloader>();
        services.AddTransient<IMp3Downloader, Mp3Download>();
        services.AddHttpClient<ICoverDownloader, CoverDownload>("Youtube");
        services.AddTransient<YoutubeClient>(z =>
        {
            var httpClientFactory = z.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("Youtube");
            return new YoutubeClient(httpClient);
        });
        services.AddTransient<YtdlpDotNet.ILogger, YtdlpLogger>();
        services.AddTransient<Ytdlp>(z =>
        {
            var logger = z.GetRequiredService<YtdlpDotNet.ILogger>();
            var options = z.GetRequiredService<IOptions<Arguments>>().Value;
            return new Ytdlp(options.YtdlpLocation, logger);
        });
        
        services.AddTransient<IMp3Tagger ,Steps.Mp3Tagger>();
        services.AddTransient<ICleanup, Cleanup>();
        
        services.AddOptions<Arguments>().Bind(configuration).ValidateDataAnnotations().ValidateOnStart()
            .PostConfigure<ILoggerFactory>(
                (z, loggerFactory ) => 
                {
                    var logger = loggerFactory.CreateLogger("Startup");
                    if (!Directory.Exists(z.OutputPath))
                    {
                        logger.LogInformation("Creating output directory: {OutputPath}", z.OutputPath);
                        Directory.CreateDirectory(z.OutputPath);
                    }
                });

        return services;
    }
}