using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CrashNet.GameObjects;

namespace CrashNet.Worlds
{
    class Tile : GameObject
    {
        Tiletype type;

        /// <summary>
        /// Make a new tile of the given type at the given position.
        /// </summary>
        /// <param name="position">The position of the tile.</param>
        /// <param name="type">The type of the tile.</param>
        public Tile(Vector2 position, Tiletype type) :
            base(position, Vector2.Zero, Vector2.Zero, Vector2.Zero, TextureManager.GetTexture(type))
        {
            this.type = type;
        }

        public Color[,] GetColors()
        {
            return TextureManager.GetColors(type);
        }
    }

    enum Tiletype
    {
        Ground,
        Wall
    }
}
