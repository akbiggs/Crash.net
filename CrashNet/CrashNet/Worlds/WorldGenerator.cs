using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CrashNet.Engine;

namespace CrashNet.Worlds
{
    static class WorldGenerator
    {
        const int ROOM_WIDTH = 19;
        const int ROOM_HEIGHT = 19;
        const string ROOM_FOLDER = "\\..\\..\\..\\Worlds\\Rooms\\";
        public static World Generate(WorldNumber number)
        {
            switch (number)
            {
                case WorldNumber.One:
                default:
                    return new World(5, 5, ROOM_WIDTH, ROOM_HEIGHT);
                case WorldNumber.Two:
                    return new World(7, 7, ROOM_WIDTH, ROOM_HEIGHT);
                case WorldNumber.Three:
                    return new World(9, 9, ROOM_WIDTH, ROOM_HEIGHT);
                case WorldNumber.Four:
                    return new World(11, 11, ROOM_WIDTH, ROOM_HEIGHT);
            }
        }

        /// <summary>
        /// Generate a tilemap from the given file.
        /// </summary>
        /// <param name="filepath">The path to a text file.</param>
        /// <returns>A tilemap generated from the given filepath.</returns>
        public static Tile[,] TilesFromFile(string filepath)
        {
            StreamReader reader = new StreamReader(filepath);
            String line = reader.ReadLine();
            Tile[,] tiles = new Tile[ROOM_WIDTH, ROOM_HEIGHT];
            int x = 0, y = 0;

            while (line != null)
            {
                line.Trim().Replace(" ", "");
                foreach (string tile in line.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    tiles[x, y] = Tile.FromString(tile);
                    ++x;
                }
                ++y;
            }

            return tiles;
        }
    }
}
