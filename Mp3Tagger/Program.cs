using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mp3Tagger;
using Mp3Tagger.Logging;
using Mp3Tagger.Steps;
using YoutubeExplode;
using YtdlpDotNet;
using ILogger = YtdlpDotNet.ILogger;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddTransient<App>();
        services.AddTransient<Mp3Download>();
        services.AddTransient<CoverDownload>();
        services.AddHttpClient<CoverDownload>("Youtube");
        services.AddTransient<YoutubeClient>(z =>
        {
            var httpClientFactory = z.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("Youtube");
            return new YoutubeClient(httpClient);
        });
        services.AddTransient<ILogger, YtdlpLogger>();
        services.AddTransient<Ytdlp>(z =>
        {
            var logger = z.GetRequiredService<ILogger>();
            var options = z.GetRequiredService<IOptions<Arguments>>().Value;
            return new Ytdlp(options.YtdlpLocation, logger);
        });
        
        services.AddTransient<Mp3Tagger.Steps.Mp3Tagger>();
        services.AddTransient<Cleanup>();
        
        services.AddOptions<Arguments>().Bind(context.Configuration).ValidateDataAnnotations().ValidateOnStart()
            .PostConfigure<ILogger<Program>>(
                (z,logger ) => 
                {
                    if (!Directory.Exists(z.OutputPath))
                    {
                        logger.LogInformation("Creating output directory: {OutputPath}", z.OutputPath);
                        Directory.CreateDirectory(z.OutputPath);
                    }
                });
    })
    .Build();

var app = host.Services.GetRequiredService<App>();
var cancellationTokenSource = new CancellationTokenSource();

Console.CancelKeyPress += (_, _) => cancellationTokenSource.Cancel();

await app.Run(cancellationTokenSource.Token);