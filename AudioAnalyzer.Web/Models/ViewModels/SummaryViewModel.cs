namespace AudioAnalyzer.Web.Models.ViewModels;

public class SummaryViewModel
{
    public string SummaryText { get; set; }

    public SummaryViewModel(string summaryText)
    {
        SummaryText = summaryText;
    }
}
