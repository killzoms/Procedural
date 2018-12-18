using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using Random = System.Random;

namespace Procedural.Terrain
{
    [RequireComponent(typeof(MeshCollider))]
    public class Chunk : MonoBehaviour
    {
        public static Vector3Int size = new Vector3Int(16, 32, 16);
        public Mesh mesh;
        public bool ready = false;
        public Vector3Int position;
        public Vector3Int offset;
        public string seed;
        public bool blocksDone = false;
        Random prng;
        public bool started = false;
        new MeshCollider collider;
        int noiseX;
        int noiseZ;
        int noiseY;
        float noiseScale = 0.02f;

        BlockType[] blocks;

        private void Awake()
        {
            collider = GetComponent<MeshCollider>();
            seed = gameObject.transform.parent.GetComponent<TerrainController>().seed;
        }

        public void Start()
        {
            prng = new Random(seed.GetHashCode());
            noiseX = prng.Next(-100000, 100000);
            noiseZ = prng.Next(-100000, 100000);
            noiseY = prng.Next(-100000, 100000);
        }

        public void GenerateBlockArray()
        {
            blocks = new BlockType[size.x * size.y * size.z];
            int index = 0;

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        float caves = Perlin.Noise3D(-((x + position.x) * noiseScale + noiseX), (y + position.y) * noiseScale + noiseY, (z + position.z) * noiseScale + noiseZ);

                        if (caves >= 0.3f)
                        {
                            blocks[index] = BlockType.Solid;
                        }
                        if (y + position.y >= 32)
                        {
                            if (caves <= (y + position.y) * noiseScale / 4.5f)
                            {
                                blocks[index] = BlockType.Air;
                            }
                        }
                        index++;
                    }
                }
            }
            blocksDone = true;
        }

        public IEnumerator GenerateMesh()
        {
            MeshBuilder builder = new MeshBuilder(position, blocks);
            builder.Start();
            yield return new WaitUntil(() => builder.Update());
            mesh = builder.GetMesh(ref mesh);
            collider.sharedMesh = mesh;
            ready = true;
            builder = null;
        }

        public BlockType GetBlockAt(int x, int y, int z)
        {
            x -= position.x;
            y -= position.y;
            z -= position.z;
            
            if (IsPointwithinBounds(x, y, z))
            {
                return blocks[x * size.y * size.z + y * size.z + z];
            }
            return BlockType.Air;
        }

        bool IsPointwithinBounds(int x, int y, int z)
        {
            return x >= 0 && y >= 0 && z >= 0 && z < size.z && y < size.y && x < size.x;
        }

    }
}
