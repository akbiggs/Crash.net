using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CrashNet.GameObjects;

namespace CrashNet.Worlds
{
    class Room
    {
        /// <summary>
        /// The size of all tiles in the room.
        /// </summary>
        const int TILESIZE = 32;

        /// <summary>
        /// All the objects in the room.
        /// </summary>
        List<GameObject> objects;

        /// <summary>
        /// All the tiles in the room.
        /// </summary>
        Tile[,] tiles;

        /// <summary>
        /// The width of the room, in tiles.
        /// </summary>
        public int Width;

        /// <summary>
        /// The height of the room, in tiles.
        /// </summary>
        public int Height;

        /// <summary>
        /// Makes a new room.
        /// </summary>
        /// <param name="width">The width of the room in tiles.</param>
        /// <param name="height">The height of the room in tiles.</param>
        public Room(int width, int height)
        {
            this.Width = width;
            this.Height = height;

            this.objects = new List<GameObject>();

            tiles = new Tile[Width, Height];
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    tiles[x, y] = new Tile(new Vector2(x * TILESIZE, y * TILESIZE), Tiletype.Ground);
        }

        /// <summary>
        /// Adds the given object to the room.
        /// </summary>
        /// <param name="obj">The object to be put in the room.</param>
        public void Add(GameObject obj)
        {
            objects.Add(obj);
        }

        internal void Update()
        {
            foreach (Tile tile in tiles)
                tile.Update();

            foreach (GameObject obj in objects)
                obj.Update();
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            foreach (Tile tile in tiles)
                tile.Draw(spriteBatch);

            foreach (GameObject obj in objects)
                obj.Draw(spriteBatch);
        }
    }
}
