using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrashNet.Worlds;
using CrashNet.GameObjects;
using Microsoft.Xna.Framework;
using CrashNet.Engine;

namespace CrashNet
{
    class World
    {
        /// <summary>
        /// All the rooms in the world.
        /// </summary>
        Room[,] rooms;

        /// <summary>
        /// The room that is currently being accessed in the world.
        /// </summary>
        Room curRoom;
        Vector2 roomCoords;

        /// <summary>
        /// The width of the world in rooms.
        /// </summary>
        public int Width;

        /// <summary>
        /// The height of the world in rooms.
        /// </summary>
        public int Height;

        /// <summary>
        /// The world we're currently in.
        /// </summary>
        WorldNumber worldNumber;

        public World(WorldNumber worldNumber)
        {
            // TODO: Get the properties that this world should have
            // based on the world number.
            this.Width = 7;
            this.Height = 5;

            rooms = new Room[Width, Height];
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    rooms[x, y] = new Room(32, 32);

            roomCoords = GetStartRoomCoords();
            curRoom = rooms[(int)roomCoords.X, (int)roomCoords.Y];
        }

        private Vector2 GetStartRoomCoords()
        {
 	        return new Vector2(Width / 2, Height / 2);
        }

        /// <summary>
        /// Adds the given object to the current room of the world.
        /// </summary>
        /// <param name="obj">The object to be added to the current room.</param>
        public void Add(GameObject obj)
        {
            Add(obj, curRoom);
        }

        /// <summary>
        /// Adds the given object to the given room of the world.
        /// </summary>
        /// <param name="obj">The object to be put in the room.</param>
        /// <param name="room">The room for the object to be put in.</param>
        private void Add(GameObject obj, Room room)
        {
            room.Add(obj);
        }

        internal void Update()
        {
            curRoom.Update();
        }

        internal void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            curRoom.Draw(spriteBatch);

            string roomString = "Room: (" + roomCoords.X.ToString() + ", " + (string)roomCoords.Y.ToString() + ")";
            spriteBatch.DrawString(FontManager.GetFont(FontNames.MAIN_MENU_FONT), roomString, 
                new Vector2(10, 10), Color.White);
        }

        private Room GetNextRoom(Direction direction)
        {
            switch (direction)
            {
                case Direction.None:
                default:
                    return curRoom;
                case Direction.North:
                    return rooms[(int)roomCoords.X, (int)roomCoords.Y - 1];
                case Direction.NorthWest:
                    return rooms[(int)roomCoords.X - 1, (int)roomCoords.Y - 1];
                case Direction.West:
                    return rooms[(int)roomCoords.X - 1, (int)roomCoords.Y];
                case Direction.SouthWest:
                    return rooms[(int)roomCoords.X - 1, (int)roomCoords.Y + 1];
                case Direction.South:
                    return rooms[(int)roomCoords.X, (int)roomCoords.Y + 1];
                case Direction.SouthEast:
                    return rooms[(int)roomCoords.X + 1, (int)roomCoords.Y + 1];
                case Direction.East:
                    return rooms[(int)roomCoords.X + 1, (int)roomCoords.Y];
                case Direction.NorthEast:
                    return rooms[(int)roomCoords.X + 1, (int)roomCoords.Y - 1];
            }
        }
    }

    enum WorldNumber
    {
        World1,
        World2,
        World3,
        World4
    }
}