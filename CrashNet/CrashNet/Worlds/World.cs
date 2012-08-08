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
        /// How far objects should be pushed away from entrances to the room
        /// upon entering the room. 
        /// </summary>
        float ENTRANCE_PADDING = 50f;

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
                    rooms[x, y] = new Room(20, 20);

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
            Direction leavingDirection;
            if (curRoom.ShouldLeave(out leavingDirection))
            {
                // remove the players from the current room, put them in the next room
                // TODO: put them in the proper position in the new room.
                Room nextRoom = GetNextRoom(leavingDirection, out roomCoords);
                foreach (Player player in curRoom.GetPlayers())
                {
                    player.Position = GetNextStartPosition(player, leavingDirection);
                    nextRoom.Add(player);
                }
                curRoom.Leave();

                curRoom = nextRoom;
            }
        }

        /// <summary>
        /// Gets the next start position of an object in a room,
        /// given its entering direction.
        /// </summary>
        /// <param name="obj">An object entering a new room.</param>
        /// <param name="direction">The direction the object is entering the room from.</param>
        /// <returns>The starting position of the object in the room.</returns>
        private Vector2 GetNextStartPosition(GameObject obj, Direction direction)
        {
            float newXPos = obj.Position.X, newYPos = obj.Position.Y;

            if (DirectionOperations.IsHorizontal(direction))
            {
                // start it at the opposite end of the room
                newXPos = MathHelper.Distance(obj.Position.X, curRoom.GetWidthInPixels());
                // pad the object away from entrance to prevent them from exiting immediately
                newXPos += (newXPos <= curRoom.GetWidthInPixels() / 2) ? ENTRANCE_PADDING : -ENTRANCE_PADDING;
            }

            if (DirectionOperations.IsVertical(direction))
            {
                newYPos = MathHelper.Distance(obj.Position.Y, curRoom.GetHeightInPixels());
                newYPos += (newYPos <= curRoom.GetHeightInPixels() / 2) ? ENTRANCE_PADDING : -ENTRANCE_PADDING;
            }

            return new Vector2(newXPos, newYPos);
        }

        internal void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            curRoom.Draw(spriteBatch);

            string roomString = "Room: (" + roomCoords.X.ToString() + ", " + (string)roomCoords.Y.ToString() + ")";
            spriteBatch.DrawString(FontManager.GetFont(FontNames.MAIN_MENU_FONT), roomString, 
                new Vector2(10, 10), Color.White);
        }

        private Room GetNextRoom(Direction direction, out Vector2 nextCoords)
        {
            Vector2 change = DirectionOperations.ToVector(direction);
            nextCoords = Vector2.Add(roomCoords, change);
            return rooms[(int)nextCoords.X, (int)nextCoords.Y];
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