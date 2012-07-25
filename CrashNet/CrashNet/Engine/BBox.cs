using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CrashNet.Engine
{
    class BBox
    {
        Rectangle box;

        public BBox(Vector2 position, int width, int height)
        {
            box = new Rectangle();

            this.Position = position;
            this.Width = width;
            this.Height = height;
        }


        public Vector2 Position {
            get { return new Vector2(box.X, box.Y); }
            set
            {
                box.X = (int)value.X;
                box.Y = (int)value.Y;
            }
        }

        public int Width {
            get { return box.Width; }
            set { box.Width = value; }
        }

        public int Height
        {
            get { return box.Height; }
            set { box.Height = value; }
        }
    }
}
