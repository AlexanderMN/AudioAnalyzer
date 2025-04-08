namespace AudioAnalyzer.Core;

public class AudioFileNameHandler
{
    public AudioFile ParseFileName(string filename)
    {
        string extension = Path.GetExtension(filename)[1..];
        string name = Path.GetFileNameWithoutExtension(filename);
        
        return new AudioFile
        {
            Extension = extension,
            Name = name,
        };
    }
}

public class AudioFile
{
    public string Name { get; set; }
    public string Extension { get; set; }
}
