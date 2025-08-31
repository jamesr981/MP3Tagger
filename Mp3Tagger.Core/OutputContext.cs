namespace Mp3Tagger.Core;

public sealed class OutputContext
{
    public required DirectoryInfo OutputFolder { get; init; }
    public required FileInfo Mp3File { get; init; }
    public required FileInfo Cover { get; init; }
}