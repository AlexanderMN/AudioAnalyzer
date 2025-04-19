using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AudioAnalyzer.Data.Persistence.Models;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    [NotMapped]
    public string ConnectionId { get; set; }
    
    public IEnumerable<AudioRequest> Requests { get; set; }
    public IEnumerable<UploadedFile> UploadedFiles { get; set; }
}
