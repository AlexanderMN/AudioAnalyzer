namespace AudioAnalyzer.Web.Models.ViewModels;

public class ViewModelBase
{
    public string Name { get; set; }

    public ViewModelBase(string name)
    {
        Name = name;
    }
}
