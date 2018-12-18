using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procedural.Terrain
{
    public enum Direction : byte
    {
        North = 1,
        East = 2,
        South = 4,
        West = 8,
        Up = 16,
        Down = 32,
    }
}
