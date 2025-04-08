namespace AudioAnalyzer.Infrastructure;

public class FtpSettings
{
    public string? IpAddress { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }

    public const string DefaultFileUploadFolder = "uploadedFiles";
    public const string DefaultFileDownloadFolder = "downloadFiles";
}
