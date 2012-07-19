using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrashNet.Worlds;

namespace CrashNet
{
    class World
    {
        Room[,] rooms;
        Room curRoom;

        internal void Update()
        {
            curRoom.Update();
        }

        internal void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            curRoom.Draw();
        }
    }
}