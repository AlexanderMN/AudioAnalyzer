using System.Text;

namespace AudioParserLib.Wav;

public class WavFileSplitter
{
    public readonly WavFileSplitterOptions WavFileSplitterOptions;
    public readonly WavFileParser WavFileParser;

    public WavFileSplitter(WavFileSplitterOptions wavFileSplitterOptions, WavFileParser wavFileParser)
    {
        WavFileSplitterOptions = wavFileSplitterOptions;
        WavFileParser = wavFileParser;
    }

    public async Task<WavFile[]> SplitWavFileAsync(bool createHeaderForEachFile)
    {
        var bigDataChunk = WavFileSplitterOptions.WavFile.WavChunks.Last();
        
        return await Task.Run(() =>
        {
            var files = CreateWavFiles(createHeaderForEachFile);
            FillWavFilesWithData(files, bigDataChunk);
            return files;
        });
    }

    private void FillWavFilesWithData(WavFile[] files, WavChunk bigDataChunk)
    {
        uint singleFileBodySize = WavFileSplitterOptions.BodySize;
        
        for(int i = 0; i < files.Length; i++)
        {
            var requiredSize = singleFileBodySize * (i + 1);

            if (bigDataChunk.ChunkData.Length < requiredSize)
            {
                singleFileBodySize = (uint)bigDataChunk.ChunkData.Length % singleFileBodySize;
            }

            var smallDataChunk = files[i].WavChunks.Last();

            Array.Copy(sourceArray: bigDataChunk.ChunkData, sourceIndex: WavFileSplitterOptions.BodySize * i,
                destinationArray: smallDataChunk.ChunkData, destinationIndex: 0, length: singleFileBodySize);
        }
    }
    
    private WavFile[] CreateWavFiles(bool createHeader)
    {
        var files = new WavFile[WavFileSplitterOptions.NumberOfFiles];
        
        if (createHeader)
        {
            for (int i = 0; i < files.Length; i++)
            {
                var smallWavFileChunks = WavFileParser.CopyWavFileChunksWithoutData(
                    wavFile: WavFileSplitterOptions.WavFile,
                    bodySizeOfDataChunk: WavFileSplitterOptions.BodySize);
                files[i] = new WavFile(smallWavFileChunks);
            }
        }
        else
        {
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = new WavFile(WavFileSplitterOptions.BodySize);
            }    
        }
        
        return files;
    }
    

    //TODO: simplify method
    //TODO: fix division problems (loosing bytes)
    
    public void SaveWavFiles(string path, string fileNameBase, params WavFile[] wavFiles)
    {
        if (wavFiles.Length == 0)
            return;

        for (int i = 0; i < wavFiles.Length; i++)
        {
            using (var fileStream = File.OpenWrite(path + "\\" + fileNameBase + i + ".wav"))
            {
                WriteWavFileToStream(wavFiles[i], fileStream);
            }
        }
    }


    public void WriteWavFileToStream(WavFile wavFile, Stream stream)
    {
        foreach (var wavChunk in wavFile.WavChunks)
        {
            stream.Write(Encoding.ASCII.GetBytes(wavChunk.ChunkId));
            stream.Write(BitConverter.GetBytes(wavChunk.ChunkSize -
                                                   WavChunk.DefaultChunkIdLength -
                                                   WavChunk.DefaultChunkSizeLength));
            stream.Write(wavChunk.ChunkData);
        }
    }
    
}