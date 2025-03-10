using System.Net;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using Microsoft.Extensions.Options;

namespace AudioAnalyzer.Infrastructure.FileService;

public class FtpClient : IFtpClient
{
    private readonly FtpSettings _settings;

    public FtpClient(IOptions<FtpSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<FtpWebResponse?> UploadFileToFTPServer(string uri, Stream stream)
    {
        
        WebRequest ftpRequest = WebRequest.Create(uri);
        ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

        //TODO encrypt passwords
        ftpRequest.Credentials = new NetworkCredential(_settings.UserName, _settings.Password);

        Stream ftpStream = await ftpRequest.GetRequestStreamAsync();

        if (!await ConvertStreamToStream(
                inputStream: stream,
                outputStream: ftpStream,
                outputFormat: SupportedAudioFormats.Wav))
        {
            return null;
        }
        
        var response = (FtpWebResponse)await ftpRequest.GetResponseAsync();
        await ftpStream.DisposeAsync();
        return response;
    }

    private async Task<bool> ConvertStreamToStream(Stream inputStream, Stream outputStream, string outputFormat)
    {
        return await FFMpegArguments
              .FromPipeInput(new StreamPipeSource(inputStream))
              .OutputToPipe(new StreamPipeSink(outputStream), options =>
                                options.ForceFormat(outputFormat))
              .ProcessAsynchronously(throwOnError: false);
    }

    private async Task DeleteFile(string path)
    {
        
    }
}

public class FtpSettings
{
    public string? IpAddress { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
}