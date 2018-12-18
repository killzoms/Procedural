using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Procedural.Terrain
{
    [RequireComponent(typeof(MeshCollider))]
    public class TerrainController : MonoBehaviour {

        public Vector3 chunkSize;
        public static Matrix4x4 id = Matrix4x4.identity;
        public Material material;

        public int radius = 2;
        public int height = 8;
        public string seed;

        Dictionary<Vector3Int, Chunk> chunkPosMap;

        public static TerrainController instance;

        Thread chunkBlockGen;

        Mesh mesh;

        private MeshCollider meshCollider;

        private void Awake()
        {
            chunkBlockGen = new Thread(GenerateChunkBlocks);
            instance = this;
            chunkPosMap = new Dictionary<Vector3Int, Chunk>();
            meshCollider = GetComponent<MeshCollider>();
            mesh = new Mesh();
            mesh.name = "Terrain";
        }


        // Use this for initialization
        void Start() {
            Chunk.size = chunkSize;
            GenerateChunks(seed);
        }

        public void GenerateChunks(object seed)
        {
            this.seed = (string)seed;
            if (this.seed == null || this.seed == "")
            {
                this.seed = (Environment.TickCount * 5).ToString();
            }
            for (int x = -radius; x < radius + 1; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = -radius; z < radius + 1; z++)
                    {
                        GameObject chunk = new GameObject();
                        chunk.transform.parent = transform;
                        chunk.transform.position = new Vector3();
                        Chunk ch = chunk.AddComponent<Chunk>();
                        ch.position = new Vector3Int((int)transform.position.x + x * Chunk.size.x, (int)transform.position.y + y * Chunk.size.y, (int)transform.position.z + z * Chunk.size.z);
                        chunk.name = string.Format("Chunk: {0}, {1}, {2}", ch.position.x, ch.position.y, ch.position.z);

                        chunkPosMap.Add(ch.position, ch);
                    }
                }
            }
            GenerateChunkBlocks();
            //chunkBlockGen.Start();
        }

        void GenerateChunkBlocks()
        {
            foreach (Chunk ch in chunkPosMap.Values)
            {
                ch.GenerateBlockArray();
            }
        }

            private void OnValidate()
        {
            if (chunkSize.x < 1)
            {
                chunkSize.x = 16;
            }
            if (chunkSize.y < 1)
            {
                chunkSize.y = 16;
            }
            if (chunkSize.z < 1)
            {
                chunkSize.z = 16;
            }
        }

        public bool GetChunkAt(int x, int y, int z, out Chunk chunk)
        {
            Vector3Int key = WorldCoordinatesToChunkCoorinates(x, y, z);
            return chunkPosMap.TryGetValue(key, out chunk);
        }

        // Update is called once per frame
        void Update() {
            foreach (Chunk ch in chunkPosMap.Values)
            {
                if (ch.blocksDone == true && !ch.started)
                {
                    StartCoroutine(ch.GenerateMesh());
                    ch.started = true;
                }
                if (ch.ready)
                {
                    Graphics.DrawMesh(ch.mesh, id, material, 0);
                }
            }
        }

        public static Vector3Int WorldCoordinatesToChunkCoorinates(int x, int y, int z)
        {
            return new Vector3Int(
                Mathf.FloorToInt(x / (float)Chunk.size.x) * Chunk.size.x,
                Mathf.FloorToInt(y / (float)Chunk.size.y) * Chunk.size.y,
                Mathf.FloorToInt(z / (float)Chunk.size.z) * Chunk.size.z);
        }
    }
}