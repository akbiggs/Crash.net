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
        // string representations
        const string GROUND = "G";
        const string WALL = "W";

        TileType type;

        /// <summary>
        /// Make a new tile of the given type.
        /// </summary>
        /// <param name="type">The type of the tile.</param>
        public Tile(TileType type) :
            base(Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero,
            Vector2.Zero, TextureManager.GetTexture(type))
        {
            this.type = type;
        }

        /// <summary>
        /// Make a new tile of the given type at the given position.
        /// </summary>
        /// <param name="position">The position of the tile.</param>
        /// <param name="type">The type of the tile.</param>
        public Tile(Vector2 position, TileType type) :
            base(position, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, 
            Vector2.Zero, TextureManager.GetTexture(type))
        {
            this.type = type;
        }

        public Color[,] GetColors()
        {
            return TextureManager.GetColors(type);
        }

        public TileType GetTileType()
        {
            return type;
        }

        public override string ToString()
        {
            switch (type)
            {
                case TileType.Ground:
                default:
                    return GROUND;
                case TileType.Wall:
                    return WALL;
            }
        }

        public static Tile FromString(string s)
        {
            switch (s)
            {
                case GROUND:
                default:
                    return new Tile(TileType.Ground);
                case WALL:
                    return new Tile(TileType.Wall);
            }
        }
    }

    enum TileType
    {
        Ground,
        Wall
    }
}
