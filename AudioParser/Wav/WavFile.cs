namespace AudioParserLib.Wav;

public class WavFile: IDisposable
{
    
    //public WavHeader Header;
    //public byte[] FullData;
    public List<WavChunk> WavChunks;
    
    private double _duration;

    public double Duration
    {
        get
        {
            if (_duration == 0)
            {
                var fmtChunk = this.WavChunks.FirstOrDefault(wc => wc.ChunkId == "fmt ");

                if (fmtChunk == null)
                    throw new Exception("Wav doesn't contain fmt chunk");

                ReadOnlySpan<byte> spanNumChannels = fmtChunk.ChunkData.AsSpan().Slice(2, 2);
                ReadOnlySpan<byte> spanSampleRate = fmtChunk.ChunkData.AsSpan().Slice(4, 4);
                ReadOnlySpan<byte> spanBitsPerSample = fmtChunk.ChunkData.AsSpan().Slice(14, 2);

                ushort numChannels = BitConverter.ToUInt16(spanNumChannels);
                uint sampleRate = BitConverter.ToUInt32(spanSampleRate);
                ushort bitsPerSample = BitConverter.ToUInt16(spanBitsPerSample);



                return WavChunks.Last().ChunkData.Length /
                       (bitsPerSample / 8.0) /
                       numChannels /
                       sampleRate;
            }
            else
            {
                return _duration;
            }
        }
        set
        {
            _duration = value;
        }
    }
    
    public WavFile(List<WavChunk> headerWavChunks)
    {
        WavChunks = headerWavChunks;
    }

    /// <summary>
    /// Empty file only consisting of one wavChunk of data
    /// </summary>
    /// <param name="bodySize"></param>
    public WavFile(uint bodySize)
    {
        WavChunks = new List<WavChunk>
        {
            new WavChunk(null, bodySize, 0, new byte[bodySize])
        };
    }
    
    public void Dispose()
    {
        WavChunks = null;
    }
}