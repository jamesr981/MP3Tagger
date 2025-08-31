namespace Mp3Tagger;

public class OutputContext
{
    public required DirectoryInfo OutputFolder { get; init; }
    public required FileInfo Mp3File { get; init; }
    public required FileInfo Cover { get; init; }
}