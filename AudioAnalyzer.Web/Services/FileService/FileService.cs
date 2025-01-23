using System.Net;
using Microsoft.Extensions.Primitives;

namespace AudioAnalyzer.Web.Services.FileService;

public class FileService : IFileService
{
    public FtpWebResponse UploadFileToFTP(string uri, Stream stream)
    {
        
        WebRequest ftpRequest = WebRequest.Create(uri);
        ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
        
        //TODO encrypt passwords
        ftpRequest.Credentials = new NetworkCredential("alexMN", "3217AlexN");
        
        Stream ftpStream = ftpRequest.GetRequestStream();
        stream.CopyTo(ftpStream);
        ftpStream.Close();
        
        
        using (var response = (FtpWebResponse)ftpRequest.GetResponse())
        {
            return response;
        }
    }
}