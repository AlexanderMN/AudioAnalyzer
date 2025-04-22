using AudioAnalyzer.Data.Models;

namespace AudioAnalyzer.Web.Models.ViewModels;

public class UserViewModel
{
    public string UserName { get; set; }
    
    public List<AudioRequest> Requests { get; set; }
    public List<UploadedFile> UploadedFiles { get; set; }
}
