using System.Reflection;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
var ffmpegLocation = GetFullPath(builder.Configuration, "FfmpegLocation", assemblyLocation);
var ytdlpLocation = GetFullPath(builder.Configuration, "YtdlpLocation", assemblyLocation);

if (string.IsNullOrWhiteSpace(ffmpegLocation))
{
    throw new FileNotFoundException("ffmpeg location not found");
}

if (string.IsNullOrWhiteSpace(ytdlpLocation))
{
    throw new FileNotFoundException("ytdlp location not found");
}

builder.AddProject<Projects.Mp3Tagger_Web>("web")
    .WithEnvironment("FfmpegLocation", ffmpegLocation)
    .WithEnvironment("YtdlpLocation", ytdlpLocation);

builder.Build().Run();

return;

string? GetFullPath(IConfiguration configuration, string configName, string asmLocation)
{
    var configValue = configuration[configName];
    if (string.IsNullOrWhiteSpace(configValue)) return null;
    
    return Path.Combine(asmLocation, configValue);
}