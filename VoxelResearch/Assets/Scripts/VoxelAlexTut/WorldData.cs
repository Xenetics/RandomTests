using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WorldData
{
    public Dictionary<WorldPos, ChunkData> chunks = new Dictionary<WorldPos, ChunkData>();

}
