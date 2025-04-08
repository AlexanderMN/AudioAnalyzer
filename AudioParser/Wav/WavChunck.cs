namespace AudioParserLib.Wav;

public class WavChunk
{
    public const int DefaultChunkIdLength = 4;
    public const int DefaultChunkSizeLength = 4;
    public WavChunk(string chunkId, uint chunkSize, long position, byte[] data)
    {
        ChunkId = chunkId;
        ChunkSize = chunkSize;
        Position = position;
        ChunkData = data;
    }

    public readonly string ChunkId;
    
    /// <summary>
    /// Size of the entire chunk, including chunkId and chunkSize fields
    /// </summary>
    
    public uint ChunkSize;
    
    
    public readonly long Position;

    /// <summary>
    /// Contains all actual data of the chunk
    /// Does not include chunkId and chunkSize,
    /// If its a RIFF chunk, contains format (WAVE in ascii)
    /// </summary>
    

    public readonly byte[] ChunkData; 
    
    public static readonly Dictionary<string, byte[]> ChunkTypesDict = new Dictionary<string, byte[]> 
    {
        {"RIFF", "RIFF"u8.ToArray()}, 
        {"fmt ", "fmt "u8.ToArray()},
        {"data", "data"u8.ToArray()},
        {"INFO", "INFO"u8.ToArray()},  
        {"LIST", "LIST"u8.ToArray()},
        //{"WAVE", new byte[] { 87, 65, 86, 69 }}
    };
}