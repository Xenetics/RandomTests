using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;

public class World : MonoBehaviour
{
    public Dictionary<WorldPos, Chunk> chunks = new Dictionary<WorldPos, Chunk>();
    public WorldData data = new WorldData();
    public GameObject chunkPrefab;
    private Thread serializeThread;
    private int currentChunk = 0;
    private string filePath;

    void Start()
    {
        filePath = Application.persistentDataPath;

        for (int x = -2; x < 2; x++)
        {
            for (int y = -1; y < 1; y++)
            {
                for (int z = -1; z < 1; z++)
                {
                    CreateChunk(x * 16, y * 16, z * 16);
                }
            }
        }

        foreach (KeyValuePair<WorldPos, Chunk> chunk in chunks)
        {
            ChunkData cData = new ChunkData();
            cData.blocks = chunk.Value.blocks;
            data.chunks.Add(chunk.Key, cData);
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(new Vector2(30, 15), new Vector2(100, 30)), "Serialize"))
        {
            if (serializeThread == null || !serializeThread.IsAlive)
            {
                if (currentChunk < data.chunks.Count)
                {
                    serializeThread = new Thread(() => SerializeChunk(currentChunk));
                    //serializeThread.Start();
                }
                //IFormatter formatter = new BinaryFormatter();
                //Stream stream = new FileStream(filePath + "/chunk" + currentChunk + ".bin", FileMode.Create, FileAccess.Write);
                //formatter.Serialize(stream, data.chunks.Values.ElementAt(currentChunk));
                //stream.Close();
            }
        }
    }

    private void SerializeChunk(int theChunk)
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(filePath + "/chunk" + theChunk + ".bin", FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, data.chunks.Values.ElementAt(theChunk));
        stream.Close();
        Debug.Log("Chunk " + theChunk + " Serialized");
        currentChunk++;
        //CloseThread();
    }

    private void CloseThread()
    {
        serializeThread.Abort();
    }

    public void DisplayData()
    {
        //for (int i = 0; i < data.chunks.Count; ++i)
        //{
        //    Debug.Log("Chunk " + i);
        //    for (int j = 0; j < data.chunks.Values.ElementAt(i).blocks.Length; ++j)
        //    {
        //        Debug.Log("Block " + j + " in Chunk " + i);
        //    }
        //}
    }

    public void CreateChunk(int x, int y, int z)
    {
        WorldPos worldPos = new WorldPos(x, y, z);

        //Instantiate the chunk at the coordinates using the chunk prefab
        GameObject newChunkObject = Instantiate(chunkPrefab, new Vector3(x, y, z), Quaternion.Euler(Vector3.zero)) as GameObject;

        Chunk newChunk = newChunkObject.GetComponent<Chunk>();

        newChunk.pos = worldPos;
        newChunk.world = this;

        //Add it to the chunks dictionary with the position as the key
        chunks.Add(worldPos, newChunk);

        for (int xi = 0; xi < 16; xi++)
        {
            for (int yi = 0; yi < 16; yi++)
            {
                for (int zi = 0; zi < 16; zi++)
                {
                    if (yi <= 7)
                    {
                        SetBlock(x + xi, y + yi, z + zi, new BlockGrass());
                    }
                    else
                    {
                        SetBlock(x + xi, y + yi, z + zi, new BlockAir());
                    }
                }
            }
        }
    }

    public Chunk GetChunk(int x, int y, int z)
    {
        WorldPos pos = new WorldPos();
        float multiple = Chunk.chunkSize;
        pos.x = Mathf.FloorToInt(x / multiple) * Chunk.chunkSize;
        pos.y = Mathf.FloorToInt(y / multiple) * Chunk.chunkSize;
        pos.z = Mathf.FloorToInt(z / multiple) * Chunk.chunkSize;

        Chunk containerChunk = null;

        chunks.TryGetValue(pos, out containerChunk);

        return containerChunk;
    }

    public Block GetBlock(int x, int y, int z)
    {
        Chunk containerChunk = GetChunk(x, y, z);

        if (containerChunk != null)
        {
            Block block = containerChunk.GetBlock(
                x - containerChunk.pos.x,
                y - containerChunk.pos.y,
                z - containerChunk.pos.z);

            return block;
        }
        else
        {
            return new BlockAir();
        }
    }

    public void SetBlock(int x, int y, int z, Block block)
    {
        Chunk chunk = GetChunk(x, y, z);

        if (chunk != null)
        {
            chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, block);
            chunk.update = true;
        }
    }

    public void DestroyChunk(int x, int y, int z)
    {
        Chunk chunk = null;
        if (chunks.TryGetValue(new WorldPos(x, y, z), out chunk))
        {
            Object.Destroy(chunk.gameObject);
            data.chunks.Remove(new WorldPos(x, y, z));
        }
    }
}
