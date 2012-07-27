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
                    SetTile(x, y, new Tile(new Vector2(x * TILESIZE, y * TILESIZE), Tiletype.Ground));
        }

        /// <summary>
        /// Sets the tile at the given coordinates to the given tile.
        /// </summary>
        /// <param name="x">The x-coordinate of the new tile, relative to other tiles.</param>
        /// <param name="y">The y-coordinate of the new tile, relative to other tiles.</param>
        /// <param name="tile">The new tile.</param>
        private void SetTile(int x, int y, Tile tile)
        {
            if (tiles == null) tiles = new Tile[Width, Height];
            tiles[x, y] = tile;

            // determine the part of the map that has not been rendered yet,
            // based on this new unrendered tile and the other tiles that have not been rendered.
            float pixelX = x * TILESIZE;
            float pixelY = y * TILESIZE;
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

        /// <summary>
        /// Adds the given object to the room.
        /// </summary>
        /// <param name="obj">The object to be put in the room.</param>
        public void Add(GameObject obj)
        {
            objects.Add(obj);
        }

        internal void Update()
        {

            if (Input.MouseLeftButtonDown)
            {
                Vector2 mousePos = Input.MousePosition;
                Vector2 tileCoords = GetTileCoordsByPixel(mousePos.X, mousePos.Y);
                SetTile((int)tileCoords.X, (int)tileCoords.Y, new Tile(new Vector2(tileCoords.X, tileCoords.Y), Tiletype.Wall));
            }
                
            foreach (Tile tile in tiles)
                tile.Update();

            foreach (GameObject obj in objects)
            {
                obj.Update();
                KeepInBounds(obj);
            }
        }

        private void KeepInBounds(GameObject obj)
        {
            if (obj.Position.X < 0)
                obj.Position = new Vector2(0, obj.Position.Y);
            else if (obj.Position.X + obj.Texture.Width >= GetWidthInPixels())
                obj.Position = new Vector2(GetWidthInPixels() - obj.Texture.Width, obj.Position.Y);

            if (obj.Position.Y < 0)
                obj.Position = new Vector2(obj.Position.X, 0);
            else if (obj.Position.Y + obj.Texture.Height >= GetHeightInPixels())
                obj.Position = new Vector2(obj.Position.X, GetHeightInPixels() - obj.Texture.Height);
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            if (ShouldRender()) Render(spriteBatch.GraphicsDevice);

            spriteBatch.Draw(texture, new Vector2(0, 0), Color.White);

            foreach (GameObject obj in objects)
                obj.Draw(spriteBatch);
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
            get
            {
                return texture;
            }
            set
            {
                texture = value;
            }
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
