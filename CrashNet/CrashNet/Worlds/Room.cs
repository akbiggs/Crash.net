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
            for (int col = 0; col < Width; col++)
                for (int row = 0; row < Height; row++)
                    SetTile(col, row, new Tile(new Vector2(col * TILESIZE, row * TILESIZE), TileType.Ground));
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
                obj.Update(this);
                KeepInBounds(obj);

                foreach (GameObject other in objects)
                {
                    if (obj != other) 
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
            obj.Position = Vector2.Clamp(obj.Position, Vector2.Zero, 
                new Vector2(GetWidthInPixels(), GetHeightInPixels()));
        }

        /// <summary>
        /// Get the number of columns that the given object spans over.
        /// </summary>
        /// <param name="obj">An object in the room.</param>
        /// <returns>The number of columns that the object spans over, starting at 0 if the object is
        /// confined to 1 tile.</returns>
        private int GetColSpan(GameObject obj)
        {
            return (int)(((obj.BBox.Y % TILESIZE) + obj.BBox.Height) / TILESIZE);
        }

        /// <summary>
        /// Get the number of rows that the given object spans over.
        /// </summary>
        /// <param name="obj">An object in the room.</param>
        /// <returns>The number of rows that the object spans over, starting at 0 if the object is
        /// confined to 1 tile.</returns>
        private int GetRowSpan(GameObject obj)
        {
            return (int)(((obj.BBox.X % TILESIZE) + obj.BBox.Width) / TILESIZE);
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
        private bool WallAt(int col, int row)
        {
            return tiles[col, row].GetTileType() == TileType.Wall;
        }

        /// <summary>
        /// Return whether or not a wall overlaps the given box.
        /// </summary>
        /// <param name="box">The bounding box that might be overlapping a wall.</param>
        /// <returns>True if the box overlaps a wall, false otherwise.</returns>
        private bool WallIntersects(BBox box)
        {
            // TODO: check every tile that the box overlaps, not just the corners.
            return (WallAtPixel(box.Left, box.Top) || WallAtPixel(box.Right, box.Top) ||
                WallAtPixel(box.Left, box.Bottom) || WallAtPixel(box.Right, box.Bottom));
        }

        /// <summary>
        /// Get all the walls that intersect with the given object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>All the walls that collide with the object.</returns>
        public List<Tile> GetIntersectingWalls(GameObject obj)
        {
            List<Tile> intersects = new List<Tile>();
            Vector2 tileCoords = GetTileCoordsByPixel(obj.BBox.Position);
            float colSpan = GetColSpan(obj);
            float rowSpan = GetRowSpan(obj);

            for (int x = (int)tileCoords.X; x <= tileCoords.X + rowSpan; x++)
                for (int y = (int)tileCoords.Y; y <= tileCoords.Y + colSpan; y++)
                    if (WallAt(x, y))
                        intersects.Add(tiles[x, y]);

            return intersects;
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

        /// <summary>
        /// Gets all the coordinates that the object spans.
        /// </summary>
        /// <param name="obj">An object in the room.</param>
        /// <returns>A list of all the tile coordinates that the object spans.</returns>
        public List<Vector2> GetAllTileCoords(GameObject obj)
        {
            List<Vector2> coords = new List<Vector2>();
            Vector2 start = GetTileCoordsByPixel(obj.Position);
            Vector2 end = GetTileCoordsByPixel(new Vector2(obj.BBox.Right, obj.BBox.Bottom));
            for (int x = (int)start.X; x <= end.X; x++)
                for (int y = (int)start.Y; y <= end.Y; y++)
                    coords.Add(new Vector2(x, y));

            return coords;
        }

        public bool ShouldRender()
        {
            return unrenderedRegion != Rectangle.Empty;
        }
    }
}
