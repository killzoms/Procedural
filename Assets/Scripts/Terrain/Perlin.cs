using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Procedural.Terrain
{
    public static class Perlin
    {
        public static float Noise3D(float x, float y, float z)
        {
            float ab = Mathf.PerlinNoise(x, y);
            float bc = Mathf.PerlinNoise(y, z);
            float ac = Mathf.PerlinNoise(x, z);

            float ba = Mathf.PerlinNoise(y, x);
            float cb = Mathf.PerlinNoise(z, y);
            float ca = Mathf.PerlinNoise(z, x);

            float abc = ab + bc + ac + ba + cb + ca;
            return abc / 6f;
        }

        public static float Noise(float x, float y)
        {
            return Mathf.PerlinNoise(x, y);
        }
    }
}
