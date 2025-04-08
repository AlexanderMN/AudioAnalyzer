using System.Reflection;

namespace AudioAnalyzer.Core;

public static class SupportedAudioFormats
{
    static SupportedAudioFormats()
    {
        SetOfSupportedFormats = new HashSet<string>();
        
        var formatFieldsInfo = typeof(SupportedAudioFormats).GetFields(BindingFlags.Public |
                                                              BindingFlags.Static | 
                                                              BindingFlags.DeclaredOnly);
        foreach (var fieldInfo in formatFieldsInfo)
        {
            if (fieldInfo is not { IsLiteral: true, IsInitOnly: false }) 
                continue;
            
            var val = fieldInfo.GetRawConstantValue();

            if (val is string format)
            {
                SetOfSupportedFormats.Add(format);
            }
        }
        
    }
    
    public static HashSet<string> SetOfSupportedFormats;
    
    public const string Wav = "wav";
    public const string Mp3 = "mp3";
    public const string Mp4 = "mp4";
    public const string Ogg = "ogg";
    public const string Acc = "acc";
    public const string Wmv = "wmv";
    public const string Avi = "avi";
}
