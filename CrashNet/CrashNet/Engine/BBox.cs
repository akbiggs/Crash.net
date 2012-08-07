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

        public BBox(Rectangle area) 
        {
            box = area;

            this.Position = new Vector2(area.X, area.Y);
            this.Width = area.Width;
            this.Height = area.Height;
        }

        public bool IsEmpty()
        {
            return GetArea() == 0;
        }

        public int GetArea()
        {
            return box.Width * box.Height;
        }

        public BBox Intersect(BBox other)
        {
            return new BBox(Rectangle.Intersect(box, other.box));
        }

        public int Top()
        {
            return box.Top;
        }

        public int Bottom()
        {
            return box.Bottom;
        }

        public int Left()
        {
            return box.Left;
        }

        public int Right()
        {
            return box.Right;
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
