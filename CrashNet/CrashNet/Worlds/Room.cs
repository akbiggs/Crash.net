using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrashNet.Worlds
{
    class Room
    {
        List<GameObject> objects;

        internal void Update()
        {
            foreach (GameObject obj in objects)
                obj.Update();
        }

        internal void Draw()
        {
            foreach (GameObject obj in objects)
                obj.Draw();
        }
    }
}
