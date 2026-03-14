using System.Net;
using AudioAnalyzer.Infrastructure;
using Microsoft.Extensions.Options;

namespace RabbitMqInfrastructure.Ftp;

public class FtpClient : IFtpClient
{   
    private WebRequest CreateFtpRequest(string uri, string ftpMethod)
    {
        WebRequest ftpRequest = WebRequest.Create(uri);
        ftpRequest.Method = ftpMethod;
        return ftpRequest;
    }
    public async Task<(FtpWebResponse?, long)> UploadFileToFTPServer(string uri, Stream stream)
    {
        var ftpRequest = CreateFtpRequest(uri, WebRequestMethods.Ftp.UploadFile);
        var ftpStream = await ftpRequest.GetRequestStreamAsync();
        var result = await FileConvertor.CopyStreamToStreamAsync(stream, ftpStream);
        if (!result.Item1)
            return (null, 0);
        
        var response = (FtpWebResponse)await ftpRequest.GetResponseAsync();
        await ftpStream.DisposeAsync();
        return (response, result.Item2);
    }

    public async Task<Stream> DownloadFileFromFTPServer(string uri)
    {
        var ftpRequest = CreateFtpRequest(uri, WebRequestMethods.Ftp.DownloadFile);
        var ftpResponse = await ftpRequest.GetResponseAsync();
        var responseStream = ftpResponse.GetResponseStream();
        return responseStream;
    }

    public async Task<FtpWebResponse> CreateFtpDirectory(string uri)
    {
        var ftpRequest = CreateFtpRequest(uri, WebRequestMethods.Ftp.MakeDirectory);
        var response = await ftpRequest.GetResponseAsync();
        return (FtpWebResponse)response;
    }

    public async Task<WebResponse> DeleteFile(string uri)
    {
        var request = CreateFtpRequest(uri, WebRequestMethods.Ftp.DeleteFile);
        var response = await request.GetResponseAsync();
        return (FtpWebResponse)response;
    }

    public async Task<FtpWebResponse> DeleteDirectory(string uri)
    {
        var request = CreateFtpRequest(uri, WebRequestMethods.Ftp.RemoveDirectory);
        var response = await request.GetResponseAsync();
        return (FtpWebResponse)response;
    }

    public async Task<FtpWebResponse> CheckIfFileExists(string uri)
    {
        var request = CreateFtpRequest(uri, WebRequestMethods.Ftp.GetFileSize);
        var response = await request.GetResponseAsync();
        return (FtpWebResponse)response;
    }

    public async Task<bool> CheckIfDirectoryExists(string uri)
    {
        string
            parentUri = uri.Substring(0, uri.TrimEnd('/').LastIndexOf('/') + 1),
            child = uri.Substring(uri.TrimEnd('/').LastIndexOf('/') + 1).TrimEnd('/');
        
        var request = CreateFtpRequest(parentUri, WebRequestMethods.Ftp.ListDirectory);
        using var response = await request.GetResponseAsync();
        
        string data = await new StreamReader(
            stream: response.GetResponseStream(),
            detectEncodingFromByteOrderMarks: true).ReadToEndAsync();
        
        return data.Contains(child, StringComparison.InvariantCultureIgnoreCase);
    }

    public async Task<long> GetFileSize(string uri)
    {
        var request = CreateFtpRequest(uri, WebRequestMethods.Ftp.GetFileSize);
        using var response = await request.GetResponseAsync();
        return response.ContentLength;
    }

    public async Task<Stream> GetFolderContents(string uri)
    {
        var request = CreateFtpRequest(uri, WebRequestMethods.Ftp.ListDirectory);
        var response = await request.GetResponseAsync();
        return response.GetResponseStream();
    }
}