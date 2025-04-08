namespace AudioParserLib.Wav;

public class WavStreamProcesser
{
    private int _offset = 0;
    private WavFileSplitter _wavFileSplitter;
    private Stream _stream;
    
    public WavStreamProcesser(WavFileSplitter wavFileSplitter, Stream stream)
    {
        _wavFileSplitter = wavFileSplitter;
        _stream = stream;
    }
    
    public bool TrySendNextToMemoryStream()
    {
        var dataChunk = _wavFileSplitter.WavFileSplitterOptions.WavFile.WavChunks.Last();

        if (_stream.CanWrite)
        {
            _stream.Write(dataChunk.ChunkData, _offset, (int)_wavFileSplitter.WavFileSplitterOptions.BodySize);
            _offset += (int)_wavFileSplitter.WavFileSplitterOptions.BodySize;
            return true;
        }
        else
        {
            return false;
        }

    }
}