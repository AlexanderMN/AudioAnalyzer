using System.Text;

namespace AudioParserLib.Wav;

public class WavFileParser
{
    public List<WavChunk> CopyWavFileChunksWithoutData(WavFile wavFile, uint bodySizeOfDataChunk = 0)
    {
        int index = 0;
        
        List<WavChunk> wavChunks = new List<WavChunk>();
        
        while (wavFile.WavChunks.Count != index &&
               wavFile.WavChunks[index].ChunkId != "data")
        {
            var templateChunk = wavFile.WavChunks[index];
            var copyChunk = new WavChunk(templateChunk.ChunkId, templateChunk.ChunkSize, templateChunk.Position, new byte[templateChunk.ChunkData.Length]);
            Array.Copy(templateChunk.ChunkData, copyChunk.ChunkData, templateChunk.ChunkData.Length);
            
            wavChunks.Add(copyChunk);
            index++;
        }

        if (wavFile.WavChunks[index].ChunkId == "data")
        {
            var templateChunk = wavFile.WavChunks[index];
            
            wavChunks.Add(new WavChunk
            (
                templateChunk.ChunkId,
                bodySizeOfDataChunk + WavChunk.DefaultChunkIdLength + WavChunk.DefaultChunkSizeLength,
                templateChunk.Position,
                new byte[bodySizeOfDataChunk]
            ));

        }
        
        MakeShallowCopyOfRiffChunk(wavChunks);
        AdjustRiffChunk(wavChunks);
        
        
        return wavChunks;
    }

    /// <summary>
    /// makes a copy of riff chunk, because later
    /// the size of this chunk will be adjusted
    /// </summary>
    /// <param name="wavChunks"> full list of chunks </param>
    /// <exception cref="Exception"> throws if RIFF chunk was not found </exception>
    private void MakeShallowCopyOfRiffChunk(List<WavChunk> wavChunks)
    {
        if (wavChunks[0].ChunkId == "RIFF")
        {
            var riffChunk = wavChunks[0];
            wavChunks[0] = new WavChunk(riffChunk.ChunkId, 0, riffChunk.Position, riffChunk.ChunkData);
        }
        else
        {
            throw new Exception("RIFF chunk is missing in WavChunkParser.CopyWavFileHeaderChunks()");
        }
    }

    private void AdjustRiffChunk(List<WavChunk> wavChunks)
    {
        foreach (var wavChunk in wavChunks)
        {
            if (wavChunk.ChunkId != "RIFF")
            {
                wavChunks[0].ChunkSize += wavChunk.ChunkSize;   
            }
        }
        
        wavChunks[0].ChunkSize += (uint)wavChunks[0].ChunkData.Length;
    }

    public async Task<List<WavChunk>> GetWavFileChunksAsync(string path)
    {
        using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            return await GetWavStreamChunksAsync(fileStream);    
        }
        
    }

    public async Task<List<WavChunk>> GetWavStreamChunksAsync(Stream fileStream)
    {
        List<WavChunk> wavChuncks = new List<WavChunk>();
        
        if (fileStream.Position != 0)
            throw new Exception("fileStream.Position != 0");
        
        byte[] byteChucnkId = new byte[WavChunk.DefaultChunkIdLength];
        byte[] byteChunkSize = new byte[WavChunk.DefaultChunkSizeLength];

        var chunckId = "";

        if (!IsFileWav(fileStream, byteChucnkId, byteChunkSize, wavChuncks))
            throw new Exception("Not wav file");

        while (fileStream.Position != fileStream.Length)
        {
            while (!WavChunk.ChunkTypesDict.ContainsKey(chunckId)
                   && fileStream.Position != fileStream.Length)
            {
                await fileStream.ReadExactlyAsync(byteChucnkId, 0, WavChunk.DefaultChunkIdLength);
                chunckId = Encoding.ASCII.GetString(byteChucnkId);
            }

            var position = fileStream.Position - WavChunk.DefaultChunkIdLength;
            await fileStream.ReadExactlyAsync(byteChunkSize, 0, WavChunk.DefaultChunkSizeLength);
            var chunkSize = BitConverter.ToUInt32(byteChunkSize);
            
            byte[] chunckContents = new byte[chunkSize];
            await fileStream.ReadExactlyAsync(chunckContents, 0, (int)chunkSize);

            wavChuncks.Add(new WavChunk(chunckId, GetAdjustedChunkSize(chunkSize), position,
                                        chunckContents));

            chunckId = "";
        }
        
        return wavChuncks;
    }
    
    private uint GetAdjustedChunkSize(uint chunkSize)
    {
        return chunkSize + WavChunk.DefaultChunkIdLength + WavChunk.DefaultChunkSizeLength;
    }
    
    private bool IsFileWav(Stream fileStream, byte[] byteChucnkId,
                            byte[] byteChunkSize, List<WavChunk> wavChuncks)
    {
        var position = fileStream.Position;
        
        fileStream.ReadExactly(byteChucnkId, 0, WavChunk.DefaultChunkIdLength);
        string chunckId = Encoding.ASCII.GetString(byteChucnkId);
        
        if (chunckId != "RIFF")
            return false;
        
        fileStream.ReadExactly(byteChunkSize, 0, WavChunk.DefaultChunkSizeLength);
        var chunkSize = BitConverter.ToUInt32(byteChunkSize);
        
        var waveFileFlag = new byte[WavChunk.DefaultChunkIdLength];
        fileStream.ReadExactly(waveFileFlag, 0, WavChunk.DefaultChunkIdLength);

        if (Encoding.ASCII.GetString(waveFileFlag) != "WAVE")
            return false;
        
        wavChuncks.Add(new WavChunk(chunckId, chunkSize, position, waveFileFlag));

        return true;
    }
}