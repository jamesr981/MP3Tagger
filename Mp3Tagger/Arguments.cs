using System.ComponentModel.DataAnnotations;

namespace Mp3Tagger;

internal sealed class Arguments
{
    [Required]
    public string OutputPath { get; init; } = string.Empty;
    
    [Required]
    public string OutputFileName { get; init; } = string.Empty;
    
    [Required]
    [Url]
    public string Mp3Url { get; init; } = string.Empty;
    
    [Url]
    public string? CoverUrl { get; init; }
    
    public string Artist { get; init; } = string.Empty;

    public string YtdlpLocation { get; init; } = @"Tools\yt-dlp.exe";
    public string FfmpegLocation { get; init; } = @"ffmpeg\ffmpeg.exe";
}