using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Mp3Tagger.Core;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddMp3Tagger(context.Configuration);
        services.AddOptions<Download>().Bind(context.Configuration).ValidateDataAnnotations().ValidateOnStart();
    })
    .Build();

var app = host.Services.GetRequiredService<MusicDownloader>();
var download = host.Services.GetRequiredService<IOptions<Download>>().Value;
var cancellationTokenSource = new CancellationTokenSource();

Console.CancelKeyPress += (_, _) => cancellationTokenSource.Cancel();

await app.Download(download, cancellationTokenSource.Token);