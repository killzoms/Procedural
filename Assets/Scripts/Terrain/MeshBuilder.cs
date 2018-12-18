using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Procedural.Terrain
{
    public class MeshBuilder : ThreadedProcess
    {
        byte[] faces = new byte[Chunk.size.x * Chunk.size.y * Chunk.size.z];

        Vector3[] verts;
        Vector2[] uvs;
        int[] tris;

        Vector3Int position;
        BlockType[] blocks;

        int sizeEstimate = 0;
        int vertexIndex = 0, trianglesIndex = 0;
        bool isVisible = false;

        public MeshBuilder(Vector3Int pos, BlockType[] blocks)
        {
            position = pos;
            this.blocks = blocks;
        }

        public override void ThreadFunction()
        {
            int index = 0;

            Chunk[] neighbors = new Chunk[6];
            bool[] exists = new bool[6];

            exists[0] = TerrainController.instance.GetChunkAt(position.x, position.y, position.z + Chunk.size.z, out neighbors[0]);
            exists[1] = TerrainController.instance.GetChunkAt(position.x + Chunk.size.x, position.y, position.z, out neighbors[1]);
            exists[2] = TerrainController.instance.GetChunkAt(position.x, position.y, position.z - Chunk.size.z, out neighbors[2]);
            exists[3] = TerrainController.instance.GetChunkAt(position.x - Chunk.size.x, position.y, position.z, out neighbors[3]);
            exists[4] = TerrainController.instance.GetChunkAt(position.x, position.y + Chunk.size.y, position.z, out neighbors[4]);
            exists[5] = TerrainController.instance.GetChunkAt(position.x, position.y - Chunk.size.y, position.z, out neighbors[5]);

            for (int x = 0; x < Chunk.size.x; x++)
            {
                for (int y = 0; y < Chunk.size.y; y++)
                {
                    for (int z = 0; z < Chunk.size.z; z++)
                    {
                        if (blocks[index].IsTransparent())
                        {
                            faces[index] = 0;
                            index++;
                            continue;
                        }

                        if (z == 0 && (exists[2] == false || neighbors[2].GetBlockAt(position.x + x, position.y + y, position.z + z - 1).IsTransparent()))
                        {
                            faces[index] |= (byte)Direction.South;
                            sizeEstimate += 4;
                        }
                        else if (z > 0 && blocks[index - 1] == BlockType.Air)
                        {
                            faces[index] |= (byte)Direction.South;
                            sizeEstimate += 4;
                        }

                        if (z == Chunk.size.z - 1 && (exists[0] == false || neighbors[0].GetBlockAt(position.x + x, position.y + y, position.z + z + 1).IsTransparent()))
                        {
                            faces[index] |= (byte)Direction.North;
                            sizeEstimate += 4;
                        }
                        else if (z < Chunk.size.z - 1 && blocks[index + 1] == BlockType.Air)
                        {
                            faces[index] |= (byte)Direction.North;
                            sizeEstimate += 4;
                        }
                        if (y == 0 && (exists[5] == false || neighbors[5].GetBlockAt(position.x + x, position.y + y - 1, position.z + z).IsTransparent()))
                        {
                            faces[index] |= (byte)Direction.Down;
                            sizeEstimate += 4;
                        }
                        else if (y > 0 && blocks[index - Chunk.size.z] == BlockType.Air)
                        {
                            faces[index] |= (byte)Direction.Down;
                            sizeEstimate += 4;
                        }
                        if (y == Chunk.size.y - 1 && (exists[4] == false || neighbors[4].GetBlockAt(position.x + x, position.y + y + 1, position.z + z).IsTransparent()))
                        {
                            faces[index] |= (byte)Direction.Up;
                            sizeEstimate += 4;
                        }
                        else if (y < Chunk.size.y - 1 && blocks[index + Chunk.size.z] == BlockType.Air)
                        {
                            faces[index] |= (byte)Direction.Up;
                            sizeEstimate += 4;
                        }
                        if (x == 0 && (exists[3] == false || neighbors[3].GetBlockAt(position.x + x - 1, position.y + y, position.z + z).IsTransparent()))
                        {
                            faces[index] |= (byte)Direction.West;
                            sizeEstimate += 4;
                        }
                        else if (x > 0 && blocks[index - Chunk.size.z * Chunk.size.y] == BlockType.Air)
                        {
                            faces[index] |= (byte)Direction.West;
                            sizeEstimate += 4;
                        }
                        if (x == Chunk.size.x - 1 && (exists[1] == false || neighbors[1].GetBlockAt(position.x + x + 1, position.y + y, position.z + z).IsTransparent()))
                        {
                            faces[index] |= (byte)Direction.East;
                            sizeEstimate += 4;
                        }
                        else if (x < Chunk.size.x - 1 && blocks[index + Chunk.size.z * Chunk.size.y] == BlockType.Air)
                        {
                            faces[index] |= (byte)Direction.East;
                            sizeEstimate += 4;
                        }

                        isVisible = true;

                        index++;
                    }
                }
            }

            if (isVisible == false)
            {
                return;
            }

            index = 0;
            verts = new Vector3[sizeEstimate];
            uvs = new Vector2[sizeEstimate];
            tris = new int[(int)(sizeEstimate * 1.5)];

            for (int x = 0; x < Chunk.size.x; x++)
            {
                for (int y = 0; y < Chunk.size.y; y++)
                {
                    for (int z = 0; z < Chunk.size.z; z++)
                    {
                        if (faces[index] == 0)
                        {
                            index++;
                            continue;
                        }

                        if ((faces[index] & (byte)Direction.North) != 0)
                        {
                            verts[vertexIndex] = new Vector3(x + position.x, y + position.y, z + position.z + 1);
                            verts[vertexIndex + 1] = new Vector3(x + position.x + 1, y + position.y, z + position.z + 1);
                            verts[vertexIndex + 2] = new Vector3(x + position.x, y + position.y + 1, z + position.z + 1);
                            verts[vertexIndex + 3] = new Vector3(x + position.x + 1, y + position.y + 1, z + position.z + 1);

                            tris[trianglesIndex] = vertexIndex + 1;
                            tris[trianglesIndex + 1] = vertexIndex + 2;
                            tris[trianglesIndex + 2] = vertexIndex;

                            tris[trianglesIndex + 3] = vertexIndex + 1;
                            tris[trianglesIndex + 4] = vertexIndex + 3;
                            tris[trianglesIndex + 5] = vertexIndex + 2;
                            vertexIndex += 4;
                            trianglesIndex += 6;
                        }
                        if ((faces[index] & (byte)Direction.East) != 0)
                        {
                            verts[vertexIndex] = new Vector3(x + position.x + 1, y + position.y, z + position.z);
                            verts[vertexIndex + 1] = new Vector3(x + position.x + 1, y + position.y + 1, z + position.z);
                            verts[vertexIndex + 2] = new Vector3(x + position.x + 1, y + position.y, z + position.z + 1);
                            verts[vertexIndex + 3] = new Vector3(x + position.x + 1, y + position.y + 1, z + position.z + 1);

                            tris[trianglesIndex] = vertexIndex + 1;
                            tris[trianglesIndex + 1] = vertexIndex + 2;
                            tris[trianglesIndex + 2] = vertexIndex;

                            tris[trianglesIndex + 3] = vertexIndex + 1;
                            tris[trianglesIndex + 4] = vertexIndex + 3;
                            tris[trianglesIndex + 5] = vertexIndex + 2;
                            vertexIndex += 4;
                            trianglesIndex += 6;
                        }
                        if ((faces[index] & (byte)Direction.South) != 0)
                        {
                            verts[vertexIndex] = new Vector3(x + position.x, y + position.y, z + position.z);
                            verts[vertexIndex + 1] = new Vector3(x + position.x, y + position.y + 1, z + position.z);
                            verts[vertexIndex + 2] = new Vector3(x + position.x + 1, y + position.y, z + position.z);
                            verts[vertexIndex + 3] = new Vector3(x + position.x + 1, y + position.y + 1, z + position.z);

                            tris[trianglesIndex] = vertexIndex + 1;
                            tris[trianglesIndex + 1] = vertexIndex + 2;
                            tris[trianglesIndex + 2] = vertexIndex;

                            tris[trianglesIndex + 3] = vertexIndex + 1;
                            tris[trianglesIndex + 4] = vertexIndex + 3;
                            tris[trianglesIndex + 5] = vertexIndex + 2;
                            vertexIndex += 4;
                            trianglesIndex += 6;
                        }
                        if ((faces[index] & (byte)Direction.West) != 0)
                        {
                            verts[vertexIndex] = new Vector3(x + position.x, y + position.y, z + position.z);
                            verts[vertexIndex + 1] = new Vector3(x + position.x, y + position.y, z + position.z + 1);
                            verts[vertexIndex + 2] = new Vector3(x + position.x, y + position.y + 1, z + position.z);
                            verts[vertexIndex + 3] = new Vector3(x + position.x, y + position.y + 1, z + position.z + 1);

                            tris[trianglesIndex] = vertexIndex + 1;
                            tris[trianglesIndex + 1] = vertexIndex + 2;
                            tris[trianglesIndex + 2] = vertexIndex;

                            tris[trianglesIndex + 3] = vertexIndex + 1;
                            tris[trianglesIndex + 4] = vertexIndex + 3;
                            tris[trianglesIndex + 5] = vertexIndex + 2;
                            vertexIndex += 4;
                            trianglesIndex += 6;
                        }
                        if ((faces[index] & (byte)Direction.Up) != 0)
                        {
                            verts[vertexIndex] = new Vector3(x + position.x, y + position.y + 1, z + position.z );
                            verts[vertexIndex + 1] = new Vector3(x + position.x, y + position.y + 1, z + position.z + 1);
                            verts[vertexIndex + 2] = new Vector3(x + position.x + 1, y + position.y + 1, z + position.z);
                            verts[vertexIndex + 3] = new Vector3(x + position.x + 1, y + position.y + 1, z + position.z + 1);

                            tris[trianglesIndex] = vertexIndex + 1;
                            tris[trianglesIndex + 1] = vertexIndex + 2;
                            tris[trianglesIndex + 2] = vertexIndex;

                            tris[trianglesIndex + 3] = vertexIndex + 1;
                            tris[trianglesIndex + 4] = vertexIndex + 3;
                            tris[trianglesIndex + 5] = vertexIndex + 2;
                            vertexIndex += 4;
                            trianglesIndex += 6;
                        }
                        if ((faces[index] & (byte)Direction.Down) != 0)
                        {
                            verts[vertexIndex] = new Vector3(x + position.x, y + position.y, z + position.z);
                            verts[vertexIndex + 1] = new Vector3(x + position.x + 1, y + position.y, z + position.z);
                            verts[vertexIndex + 2] = new Vector3(x + position.x, y + position.y, z + position.z + 1);
                            verts[vertexIndex + 3] = new Vector3(x + position.x + 1, y + position.y, z + position.z + 1);

                            tris[trianglesIndex] = vertexIndex + 1;
                            tris[trianglesIndex + 1] = vertexIndex + 2;
                            tris[trianglesIndex + 2] = vertexIndex;

                            tris[trianglesIndex + 3] = vertexIndex + 1;
                            tris[trianglesIndex + 4] = vertexIndex + 3;
                            tris[trianglesIndex + 5] = vertexIndex + 2;
                            vertexIndex += 4;
                            trianglesIndex += 6;
                        }


                        index++;
                    }
                }
            }
        }

        public Mesh GetMesh(ref Mesh copy)
        {
            if (copy == null)
            {
                copy = new Mesh();
            }
            else
            {
                copy.Clear();
            }
            if (isVisible == false || vertexIndex == 0)
            {
                return copy;
            }
            if (vertexIndex > 65535)
            {
                //copy.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            }

            copy.vertices = verts;
            copy.uv = uvs;
            copy.triangles = tris;

            copy.RecalculateNormals();
            return copy;
        }
    }
}
