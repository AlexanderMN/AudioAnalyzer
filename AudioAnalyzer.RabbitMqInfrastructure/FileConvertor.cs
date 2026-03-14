namespace AudioAnalyzer.Infrastructure;

public static class FileConvertor
{
    public static async Task<(bool, long)> CopyStreamToStreamAsync(Stream inputStream, Stream outputStream)
    {
        inputStream.Position = 0;
        await inputStream.CopyToAsync(outputStream);
        long streamLength = inputStream.Length;
        await inputStream.DisposeAsync();
        return (true, streamLength);
    }
}
