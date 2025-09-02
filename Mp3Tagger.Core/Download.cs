using System.ComponentModel.DataAnnotations;
using OneOf;
using OneOf.Types;

namespace Mp3Tagger.Core;

public sealed class Download
{
    [Required]
    public string SongTitle { get; set; } = string.Empty;
    
    [Required]
    [Url]
    public string Mp3Url { get; set; } = string.Empty;
    
    [Url]
    public string? CoverUrl { get; set; }
    
    public string Artist { get; set; } = string.Empty;

    public OneOf<OneOf.Types.Success, OneOf.Types.Error<IEnumerable<string>>> Validate()
    {
        List<string> errors = [];
        if (string.IsNullOrWhiteSpace(SongTitle))
        {
            errors.Add("Song Title is required.");
        }

        if (string.IsNullOrWhiteSpace(Mp3Url))
        {
            errors.Add("Mp3 Url is required.");
        }
        else if (!IsUrlValid(Mp3Url))
        {
            errors.Add("Mp3 Url is not a valid url.");
        }

        if (!IsUrlValid(CoverUrl))
        {
            errors.Add("Cover Url is not a valid url.");
        }

        if (string.IsNullOrWhiteSpace(Artist))
        {
            errors.Add("Artist is required.");
        }

        if (errors.Any())
        {
            return new Error<IEnumerable<string>>(errors);
        }

        return new Success();
    }

    public static bool IsUrlValid(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return true;
        
        return url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
               || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
    }
}