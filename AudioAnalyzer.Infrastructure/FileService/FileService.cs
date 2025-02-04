using System.Net;

namespace AudioAnalyzer.Infrastructure.FileService;

public class FileService : IFileService
{
    public async Task<FtpWebResponse> UploadFileToFTP(string uri, Stream stream)
    {
        return await Task.Run(() =>
        {
            WebRequest ftpRequest = WebRequest.Create(uri);
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
        
            //TODO encrypt passwords
            ftpRequest.Credentials = new NetworkCredential("alexMN", "3217AlexN");
        
            Stream ftpStream = ftpRequest.GetRequestStream();
            stream.CopyTo(ftpStream);
            ftpStream.Close();


            var response = (FtpWebResponse)ftpRequest.GetResponse();
            return response;
        });
        
    }
}