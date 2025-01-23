namespace AudioAnalyzer.Web.Models.Persistence.Repositories.AudioExtensions;

public class LocalAudioExtensionRepository : IAudioExtensionRepository
{
    private IConfiguration _configuration;

    public LocalAudioExtensionRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public List<string> GetAllAudioExtensions()
    {
        var audioExtensions = new List<string>();
        
        var audioExtensionsSection = GetAudioExtensionSection().GetChildren();
        
        foreach (var audioExtensionSection in audioExtensionsSection)
        {
            if (string.IsNullOrEmpty(audioExtensionSection.Value))
                continue;
            audioExtensions.Add(audioExtensionSection.Value);
        }

        return audioExtensions;
    }

    public string GetAudioExtensionById(string id)
    {
        throw new NotImplementedException();
    }

    public string GetAudioExtensionsSectionName()
    {
        var audioExtensionsSectionName = GetAudioExtensionSection().Key;

        return audioExtensionsSectionName;
    }

    private IConfigurationSection GetAudioExtensionSection()
    {
        return _configuration.GetSection("audio-ext");
    }
}