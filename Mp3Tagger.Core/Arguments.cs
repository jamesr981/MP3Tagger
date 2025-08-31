using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Mp3Tagger.Core;

public sealed class Arguments
{
    [Required]
    public string OutputPath { get; init; } =
        Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, "Downloads");

    public string YtdlpLocation { get; init; } = @"Tools\yt-dlp.exe";
    public string FfmpegLocation { get; init; } = @"ffmpeg\ffmpeg.exe";
}