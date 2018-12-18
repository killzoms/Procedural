using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Procedural.Terrain
{
    public enum BlockType
    {
        Air,
        Solid
    }

    public static class BlockType_Extensions
    {
        public static bool IsTransparent(this BlockType block)
        {
            return block == BlockType.Air;
        }
    }
}
