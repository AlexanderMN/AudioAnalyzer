namespace AudioAnalyzer.Core;

public class AudioFileNameHandler
{
    public AudioFile GenerateFileName(string filename)
    {
        var audiofile = ParseFileName(filename);
        
        Guid audioId = Guid.NewGuid();
        audiofile.Name = audioId.ToString();
        
        return audiofile;
    }

    private AudioFile ParseFileName(string filename)
    {
        string extension = Path.GetExtension(filename);
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
