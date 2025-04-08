using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;

namespace AudioAnalyzer.Infrastructure;

public static class FfMpegWrapper
{
    public static async Task<bool> ConvertStreamToStream(Stream inputStream, Stream outputStream, string outputFormat)
    {
         
        return await FFMpegArguments
                     .FromPipeInput(new StreamPipeSource(inputStream))
                     .OutputToPipe(new StreamPipeSink(outputStream), options =>
                     {
                         options.ForceFormat(outputFormat);
                         options.WithAudioSamplingRate(16000);
                         options.WithAudioBitrate(256);
                     }).WithLogLevel(FFMpegLogLevel.Info)
                     .ProcessAsynchronously(throwOnError: false);
    }
}
