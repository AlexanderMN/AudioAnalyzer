namespace AudioParserLib.Wav;

public class WavFileSplitterOptions
{
    internal WavFile WavFile;

    private int _timeFrames;

    public int TimeFrames
    {
        get { return _timeFrames; }
        set
        {
            _timeFrames = value;
        }
    }


    public int NumberOfFiles { get; set; }
    public uint BodySize {get; set;}

    public WavFileSplitterOptions(int timeFrames, WavFile wavFile)
    {
        WavFile = wavFile;
        TimeFrames = timeFrames;
        SetSplitParams();
    }
    
    public void ChangeTimeFrames(int timeFrames)
    {
        TimeFrames = timeFrames;
    }

    private void SetSplitParams()
    {
        int numberOfFiles = (int)Math.Ceiling(WavFile.Duration / _timeFrames);

        var ratioToFullSize = (numberOfFiles * _timeFrames) / WavFile.Duration;

        if ((WavFile.WavChunks[0].ChunkSize & 1) != 0)
            Console.WriteLine("file can't be processed, chunk size is an odd number");

        double sizeOfDataChunk = WavFile.WavChunks.Last().ChunkData.Length;

        //if processed file is longer than required time frame
        if (ratioToFullSize < 0)
        {
            sizeOfDataChunk *= ratioToFullSize;
        }
        
        var bodySize = (uint)((sizeOfDataChunk) / numberOfFiles);
        
        if ((bodySize & 1) != 0)
            bodySize--;

        (BodySize, NumberOfFiles) = (bodySize, numberOfFiles);
    }
}