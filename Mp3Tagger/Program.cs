using Id3;
using Id3.Frames;

const string musicDirectory = @"C:\temp\Music";

var mp3Files = Directory.GetFiles(musicDirectory, "*.mp3", SearchOption.AllDirectories);

foreach (var mp3File in mp3Files)
{
    CreateTagForFile(mp3File);
}

return;

void CreateTagForFile(string mp3Path)
{
    using var mp3 = new Mp3(mp3Path, Mp3Permissions.ReadWrite);
    var tagToCreate = CreateTag(mp3Path);
    mp3.WriteTag(tagToCreate);
}

Id3Tag CreateTag(string filePath)
{
    var title = Path.GetFileNameWithoutExtension(filePath);
    var artistFrame = new ArtistsFrame();
    var artist = GetArtistFromFile(filePath);
    if (artist is not null)
    {
        artistFrame.Value.Add(artist);
    }

    var tag = new Id3Tag
    {
        Title = title,
        Album = title,
        Artists = artistFrame,
    };

    return tag.ConvertTo(Id3Version.V23);
}

string? GetArtistFromFile(string mp3Path)
{
    var directory = Path.GetDirectoryName(mp3Path);
    if (directory is null) return null;
    var artistFilePath = Path.Combine(directory, "artist.txt");
    if (!File.Exists(artistFilePath)) return null;
    
    using var stream = File.OpenRead(artistFilePath);
    using var streamReader = new StreamReader(stream);
    var artist = streamReader.ReadLine();
    return artist;
}

