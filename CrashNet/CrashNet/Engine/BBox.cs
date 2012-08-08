using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CrashNet.Engine
{
    class BBox
    {
        /// from http://pastie.org/3152377
        /// <summary>
        /// Get the depth of the horizontal interesection between two boxes.
        /// </summary>
        /// <param name="boxA">A box.</param>
        /// <param name="boxB">Another box.</param>
        /// <returns>The depth of the horizontal intersection, negative if left, 
        /// positive if right.</returns>
        public static float GetHorizontalIntersectionDepth(BBox boxA, BBox boxB)
        {
            // Calculate half sizes.
            float halfWidthA = boxA.Width / 2.0f;
            float halfWidthB = boxB.Width / 2.0f;

            // Calculate centers.
            float centerA = boxA.Left + halfWidthA;
            float centerB = boxB.Left + halfWidthB;

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA - centerB;
            float minDistanceX = halfWidthA + halfWidthB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX)
                return 0f;

            // Calculate and return intersection depths.
            return distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
        }

        /// from http://pastie.org/3152377
        /// <summary>
        /// Get the depth of the vertical intersection between the two boxes.
        /// </summary>
        /// <param name="boxA">A box.</param>
        /// <param name="boxB">Another box.</param>
        /// <returns>The depth of the vertical intersection, negative if up,
        /// positive if down.</returns>
        public static float GetVerticalIntersectionDepth(BBox boxA, BBox boxB)
        {
            // Calculate half sizes.
            float halfHeightA = boxA.Height / 2.0f;
            float halfHeightB = boxB.Height / 2.0f;

            // Calculate centers.
            float centerA = boxA.Top + halfHeightA;
            float centerB = boxB.Top + halfHeightB;

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceY = centerA - centerB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceY) >= minDistanceY)
                return 0f;

            // Calculate and return intersection depths.
            return distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
        }

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

        public BBox Offset(Vector2 offset)
        {
            return Offset((int)offset.X, (int)offset.Y);
        }

        public BBox Offset(int x, int y)
        {
            return new BBox(new Vector2(Position.X + x, Position.Y + y), Width, Height);
        }

        public BBox Intersect(BBox other)
        {
            return new BBox(Rectangle.Intersect(box, other.box));
        }

        public int Top { get { return box.Top; } }

        public int Bottom { get { return box.Bottom; } }

        public int Left { get { return box.Left; } }

        public int Right { get { return box.Right; } }

        public Point Center { get { return box.Center; } }

        public Vector2 Position {
            get { return new Vector2(box.X, box.Y); }
            set
            {
                box.X = (int)value.X;
                box.Y = (int)value.Y;
            }
        }

        public float X { get { return box.X; } }
        public float Y { get { return box.Y; } }

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
