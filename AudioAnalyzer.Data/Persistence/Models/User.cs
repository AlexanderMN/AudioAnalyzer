namespace AudioAnalyzer.Data.Persistence.Models;

public class User
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string ConnectionId { get; set; }
    public string UploadedFileName { get; set; }
}
