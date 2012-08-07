using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CrashNet.GameObjects;
using CrashNet.Engine;

namespace CrashNet.Worlds
{
    class Room : Renderable
    {
        /// <summary>
        /// The size of all tiles in the room.
        /// </summary>
        const int TILESIZE = 32;

        /// <summary>
        /// How much distance to keep objects away from walls.
        /// </summary>
        const float WALL_PADDING = 0.01f;

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

        Texture2D texture = null;
        Rectangle unrenderedRegion = Rectangle.Empty;

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
                    SetTile(x, y, new Tile(new Vector2(x * TILESIZE, y * TILESIZE), TileType.Ground));
        }

        internal void Update()
        {
            #region LEVEL EDITOR
            if (Input.MouseLeftButtonDown)
            {
                Vector2 mousePos = Input.MousePosition;
                Vector2 tileCoords = GetTileCoordsByPixel(mousePos.X, mousePos.Y);
                SetTile((int)tileCoords.X, (int)tileCoords.Y, new Tile(new Vector2(tileCoords.X, tileCoords.Y), TileType.Wall));
            }

            if (Input.MouseRightButtonDown)
            {
                Vector2 mousePos = Input.MousePosition;
                Add(new Player(PlayerNumber.One, mousePos));
            }
            #endregion

            foreach (Tile tile in tiles)
                tile.Update();

            foreach (GameObject obj in objects)
            {
                obj.Update();
                KeepInBounds(obj);

                //// calculate all the corrections from tile collisions to the object's
                //// position, sum them to get the actual new position.
                //List<Vector2> corrections = new List<Vector2>();
                //foreach (Tile tile in tiles)
                //{
                //    Vector2 correction = Collide(obj, tile);
                //    if (correction != Vector2.Zero) corrections.Add(correction);
                //};
                //foreach (float x in GetDistinctXValues(corrections))
                //    obj.Position = new Vector2(obj.Position.X + x, obj.Position.Y);
                //foreach (float y in GetDistinctYValues(corrections))
                //    obj.Position = new Vector2(obj.Position.X, obj.Position.Y + y);

                foreach (GameObject other in objects.Where(x => x != obj))
                {
                    Collide(obj, other);
                }        
            }
        }

        private IEnumerable<float> GetDistinctXValues(List<Vector2> corrections)
        {
            return corrections.Select(x => x.X).Distinct();
        }

        private IEnumerable<float> GetDistinctYValues(List<Vector2> corrections)
        {
            return corrections.Select(x => x.Y).Distinct();
        }

        /// <summary>
        /// Collides the given game object with the other game object.
        /// </summary>
        /// <param name="obj">The object colliding.</param>
        /// <param name="other">The object being collided against.</param>
        /// <returns>The new position of the colliding object after the collision.</returns>
        private Vector2 Collide(GameObject obj, GameObject other)
        {
            BBox region = obj.GetCollision(other);
            Vector2 newPos = Vector2.Zero;
            if (!region.IsEmpty())
            {
                newPos = obj.ResolveCollision(other, region);
            }

            return newPos;
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            if (ShouldRender()) Render(spriteBatch.GraphicsDevice);

            spriteBatch.Draw(texture, new Vector2(0, 0), Color.White);

            foreach (GameObject obj in objects)
                obj.Draw(spriteBatch);
        }

        /// <summary>
        /// Sets the tile at the given coordinates to the given tile.
        /// </summary>
        /// <param name="col">The x-coordinate of the new tile, relative to other tiles.</param>
        /// <param name="row">The y-coordinate of the new tile, relative to other tiles.</param>
        /// <param name="tile">The new tile.</param>
        private void SetTile(int col, int row, Tile tile)
        {
            if (ValidRow(row) && ValidCol(col))
            {
                if (tiles == null) tiles = new Tile[Width, Height];
                tile.Position = new Vector2(col * TILESIZE, row * TILESIZE);
                tiles[col, row] = tile;

                // determine the part of the map that has not been rendered yet,
                // based on this new unrendered tile and the other tiles that have not been rendered.
                float pixelX = col * TILESIZE;
                float pixelY = row * TILESIZE;
                if (unrenderedRegion == Rectangle.Empty)
                    unrenderedRegion = new Rectangle((int)pixelX, (int)pixelY, TILESIZE, TILESIZE);
                else
                {
                    float smallerX = Math.Min(pixelX, unrenderedRegion.X);
                    float largerX = Math.Max(pixelX, unrenderedRegion.X);
                    float smallerY = Math.Min(pixelY, unrenderedRegion.Y);
                    float largerY = Math.Max(pixelY, unrenderedRegion.Y);
                    unrenderedRegion = new Rectangle((int)smallerX, (int)smallerY,
                        (int)(largerX - smallerX + TILESIZE), (int)(largerY - smallerY + TILESIZE));
                }
            }
        }


        private bool ValidCol(int col)
        {
            return col >= 0 && col < Width;
        }

        private bool ValidRow(int row)
        {
            return row >= 0 && row < Height;
        }

        /// <summary>
        /// Adds the given object to the room.
        /// </summary>
        /// <param name="obj">The object to be put in the room.</param>
        public void Add(GameObject obj)
        {
            objects.Add(obj);
        }

        private void KeepInBounds(GameObject obj)
        {
            // figure out the change to force in the object's x-position to keep it in bounds.
            List<float> xBounds = GetXBounds(obj);
            float boundedX = MathHelper.Clamp(obj.Position.X, xBounds.Min(), xBounds.Max());
            float changeX = Math.Abs(boundedX - obj.Position.X);

            // figure out the change to force in the object's y-position to keep it in bounds.
            List<float> yBounds = GetYBounds(obj);
            float boundedY = MathHelper.Clamp(obj.Position.Y, yBounds.Min(), yBounds.Max());
            float changeY = MathHelper.Distance(boundedY, obj.Position.Y);
            
            // make the smaller of the two changes, then recalculate the other one.
            // if they're equal, make the change that pushes the object outside the wall first.
            // TODO: make this algorithm not suck.
            if (changeY > changeX || (WallAtPixel(obj.Position.X, boundedY)))
            {
                obj.Position = new Vector2(boundedX, obj.Position.Y);

                yBounds = GetYBounds(obj);
                boundedY = MathHelper.Clamp(obj.Position.Y, yBounds.Min(), yBounds.Max());
                obj.Position = new Vector2(boundedX, boundedY);
            }
            else
            {
                obj.Position = new Vector2(obj.Position.X, boundedY);

                xBounds = GetXBounds(obj);
                boundedX = MathHelper.Clamp(obj.Position.X, xBounds.Min(), xBounds.Max());
                obj.Position = new Vector2(boundedX, boundedY);
            }
        }

        private List<float> GetXBounds(GameObject obj)
        {
            Vector2 objCoords = GetTileCoordsByPixel(obj.Position);
            float lowerBoundX = 0, upperBoundX = GetWidthInPixels();

            // determine how much the object spans
            int colSpan = GetColSpan(obj);
            int rowSpan = GetRowSpan(obj);
            
            // get lower bound of x-movement. We only need to check about two tiles around to see if there's anything to be
            // collided with.
            bool hit = false;
            for (int x = (int)objCoords.X; x >= Math.Max(0, (int)(objCoords.X - 2)); x--)
            {
                //check along all the tiles that the object spans
                for (int y = (int)objCoords.Y; y <= objCoords.Y + colSpan; y++)
                    if (WallAt(x, y))
                    {
                        // since we're colliding with the right side of the tile, add another
                        // tile's size to it.
                        lowerBoundX = x * TILESIZE + TILESIZE + WALL_PADDING;
                        hit = true;
                        break;
                    }
                if (hit) break;
            }

            //get upper bound of x-movement. Again, only need to check two tiles to the right.
            hit = false;
            for (int x = (int)objCoords.X + rowSpan; x < Math.Min(Width, (int)(objCoords.X + rowSpan + 2)); x++)
            {
                for (int y = (int)objCoords.Y; y <= objCoords.Y + colSpan; y++)
                    if (WallAt(x, y))
                    {
                        // because we're getting the bounds of the object's position, which is at the
                        // top-left corner of the object, subtract the width of the object's texture.
                        upperBoundX = x * TILESIZE - obj.Texture.Width - WALL_PADDING;
                        hit = true;
                        break;
                    }
                if (hit) break;
            }

            // if the upper bound is less than the lower bound, the object is
            // inside a corner and should be pushed out of it.
            if (upperBoundX <= lowerBoundX)
            {
                // collision against left corner? push to the left
                if (LeftCornerCollision(obj))
                {
                    upperBoundX -= WALL_PADDING;
                    lowerBoundX = 0;
                }
                // collision against right corner? push to the right.
                else
                {
                    lowerBoundX += WALL_PADDING;
                    upperBoundX = GetWidthInPixels();
                }
            }

            return new List<float> { lowerBoundX, upperBoundX };
        }

        private bool LeftCornerCollision(GameObject obj)
        {
            // TODO: use XOR to clean this up once I figure out what XOR is in XNA.
            return (GetTileByPixel(obj.Position).GetTileType() == TileType.Wall
                && GetTileByPixel(new Vector2(obj.BBox.Left(), obj.BBox.Bottom())).GetTileType() != TileType.Wall)
                || (GetTileByPixel(obj.Position).GetTileType() != TileType.Wall
                && GetTileByPixel(new Vector2(obj.BBox.Left(), obj.BBox.Bottom())).GetTileType() == TileType.Wall);
        }

        private List<float> GetYBounds(GameObject obj)
        {
            Vector2 objCoords = GetTileCoordsByPixel(obj.Position);
            float lowerBoundY = 0, upperBoundY = GetHeightInPixels();

            // determine the number of tiles the object can collide with height-wise and width-wise
            int colSpan = GetColSpan(obj);
            int rowSpan = GetRowSpan(obj);

            // get lower bound of y-movement
            bool hit = false;
            for (int y = (int)objCoords.Y; y >= Math.Max(0, (int)(objCoords.Y - 2)); y--)
            {
                for (int x = (int)objCoords.X; x <= objCoords.X + rowSpan; x++)
                    if (WallAt(x, y))
                    {
                        // since we're colliding with the bottom part of the tile, add
                        // another tile's size to the bound
                        lowerBoundY = y * TILESIZE + TILESIZE + WALL_PADDING;
                        hit = true;
                        break;
                    }
                if (hit) break;
            }

            //get upper bound of y-movement
            hit = false;
            for (int y = (int)objCoords.Y + colSpan; y < Math.Min(Height, (int)(objCoords.Y + colSpan + 2)); y++)
            {
                for (int x = (int)objCoords.X; x <= objCoords.X + rowSpan; x++)
                    if (WallAt(x, y))
                    {
                        // because we're getting the bounds of the object's position, which is at the
                        // top-left corner of the object, subtract the height of the texture.
                        upperBoundY = y * TILESIZE - obj.Texture.Height - WALL_PADDING;
                        hit = true;
                        break;
                    }
                if (hit) break;
            }

            // if the upper bound is less than the lower bound, the object is
            // currently inside another tile and should be pushed out.
            if (upperBoundY < lowerBoundY)
            {
                // collision against bottom corner? push to the left
                if (BottomCornerCollision(obj))
                {
                    upperBoundY -= WALL_PADDING;
                    lowerBoundY = 0;
                }
                // collision against right corner? push to the right
                else
                {
                    lowerBoundY += WALL_PADDING;
                    upperBoundY = GetHeightInPixels();
                }
            }

            return new List<float> { lowerBoundY, upperBoundY };
        }

        /// <summary>
        /// Get the number of columns that the given object spans over.
        /// </summary>
        /// <param name="obj">An object in the room.</param>
        /// <returns>The number of columns that the object spans over, starting at 0 if the object is
        /// confined to 1 tile.</returns>
        private int GetColSpan(GameObject obj)
        {
            return (int)(((obj.Position.Y % TILESIZE) + obj.Texture.Height) / TILESIZE);
        }

        /// <summary>
        /// Get the number of rows that the given object spans over.
        /// </summary>
        /// <param name="obj">An object in the room.</param>
        /// <returns>The number of rows that the object spans over, starting at 0 if the object is
        /// confined to 1 tile.</returns>
        private int GetRowSpan(GameObject obj)
        {
            return (int)(((obj.Position.X % TILESIZE) + obj.Texture.Width) / TILESIZE);
        }

        private bool BottomCornerCollision(GameObject obj)
        {
            // TODO: simplify with xor
            return (WallAtPixel(obj.Position) && !WallAtPixel(new Vector2(obj.BBox.Right(), obj.BBox.Top()))) ||
                (!WallAtPixel(obj.Position) && WallAtPixel(new Vector2(obj.BBox.Right(), obj.BBox.Top())));
        }

        /// <summary>
        /// Return whether or not there is a wall at the given pixel.
        /// </summary>
        /// <param name="pixel">The pixel to check.</param>
        /// <returns>True if there is a wall at the given position, false otherwise.</returns>
        private bool WallAtPixel(Vector2 pixel)
        {
            return WallAtPixel(pixel.X, pixel.Y);
        }

        /// <summary>
        /// Return whether or not there is a wall at the given pixel.
        /// </summary>
        /// <param name="x">The x-coord of the pixel.</param>
        /// <param name="y">The y-coord of the pixel.</param>
        /// <returns>True if there is a wall at the given position, false otherwise.</returns>
        private bool WallAtPixel(float x, float y)
        {
            return GetTileByPixel(x, y).GetTileType() == TileType.Wall;
        }

        /// <summary>
        /// Returns whether or not there is a wall at the given row-column.
        /// </summary>
        /// <param name="row">The row of the tile.</param>
        /// <param name="col">The column of the tile.</param>
        /// <returns>True if there is a wall at the given row/column, false otherwise.</returns>
        private bool WallAt(int row, int col)
        {
            return tiles[row, col].GetTileType() == TileType.Wall;
        }

        private Vector2 GetTileCoordsByPixel(Vector2 pixel)
        {
            return GetTileCoordsByPixel(pixel.X, pixel.Y);
        }

        private int GetWidthInPixels()
        {
            return Width * TILESIZE;
        }

        private int GetHeightInPixels()
        {
            return Height * TILESIZE;
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public void Render(GraphicsDevice graphicsDevice)
        {
            if (texture == null)
                texture = new Texture2D(graphicsDevice, Width * TILESIZE, Height * TILESIZE);
            Render(graphicsDevice, unrenderedRegion);
        }

        public void Render(GraphicsDevice graphicsDevice, Rectangle region)
        {
            Color[] canvas = new Color[region.Width * region.Height];
            Color[,] paint = new Color[TILESIZE, TILESIZE];

            // go through each pixel on the region to be rendered and paint it the color
            // of the tile at that pixel.
            for (int x = 0; x < region.Width; x++)
                for (int y = 0; y < region.Height; y++)
                {
                    if (y % TILESIZE == 0)
                        paint = GetTileByPixel(x + region.X, y + region.Y).GetColors();

                    canvas[x + y * region.Width] = paint[x % TILESIZE, y % TILESIZE];
                }

            texture.SetData(0, region, canvas, 0, canvas.Length);

            unrenderedRegion = Rectangle.Empty;
        }

        private Tile GetTileByPixel(Vector2 pixel)
        {
            return GetTileByPixel(pixel.X, pixel.Y);
        }

        private Tile GetTileByPixel(float x, float y)
        {
            Vector2 tileCoords = GetTileCoordsByPixel(x, y);
            return tiles[(int)tileCoords.X, (int)tileCoords.Y];
        }

        private Vector2 GetTileCoordsByPixel(float x, float y)
        {
            return new Vector2((int)(x / TILESIZE), (int)(y / TILESIZE));
        }

        public bool ShouldRender()
        {
            return unrenderedRegion != Rectangle.Empty;
        }
    }
}
