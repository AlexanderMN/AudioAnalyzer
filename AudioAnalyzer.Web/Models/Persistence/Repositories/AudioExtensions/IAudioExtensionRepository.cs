namespace AudioAnalyzer.Web.Models.Persistence.Repositories.AudioExtensions;

public interface IAudioExtensionRepository 
{
    
    public List<string> GetAllAudioExtensions();
    public string GetAudioExtensionById(string id);
    public string GetAudioExtensionsSectionName();
}