using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrashNet.Worlds;

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

            curRoom = GetStartRoom();
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

        /// <summary>
        /// Gets the starting room of the world.
        /// </summary>
        /// <returns>The starting room of the world.</returns>
        private Room GetStartRoom()
        {
            return rooms[Width / 2, Height / 2];
        }

        internal void Update()
        {
            curRoom.Update();
        }

        internal void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            curRoom.Draw(spriteBatch);
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