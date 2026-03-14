namespace AudioAnalyzer.Web.Models.ViewModels;

public class ClassificationViewModel
{
    public string TextClass { get; set; }

    public ClassificationViewModel(string textClass)
    {
        TextClass = textClass;
    }
}
