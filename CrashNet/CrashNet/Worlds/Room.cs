﻿using System;
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

        /// <summary>
        /// Gets the width of the room in pixels.
        /// </summary>
        /// <returns>The width of the room in pixels.</returns>
        private int GetWidthInPixels()
        {
            return Width * TILESIZE;
        }

        /// <summary>
        /// Gets the height of the room in pixels.
        /// </summary>
        /// <returns>The height of the room in pixels.</returns>
        private int GetHeightInPixels()
        {
            return Height * TILESIZE;
        }

        /// <summary>
        /// Updates the room.
        /// </summary>
        internal virtual void Update()
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
                tile.Update(this);

            foreach (GameObject obj in objects)
            {
                obj.Update(this);
                KeepInBounds(obj);

                foreach (GameObject other in objects)
                {
                    BBox region;
                    if (obj != other && obj.ShouldCollide(other, out region))
                        obj.Collide(other, region);
                }        
            }
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            if (ShouldRender()) Render(spriteBatch.GraphicsDevice);

            spriteBatch.Draw(texture, new Vector2(0, 0), Color.White);

            foreach (GameObject obj in objects)
                obj.Draw(spriteBatch);
        }

        /// <summary>
        /// Check whether or not the given column is in bounds.
        /// </summary>
        /// <param name="col">The column to check, 0-indexed.</param>
        /// <returns>True if the column is in bounds, false otherwise.</returns>
        private bool ValidCol(int col)
        {
            return col >= 0 && col < Width;
        }

        /// <summary>
        /// Check whether or not the given row is in bounds.
        /// </summary>
        /// <param name="row">The row to check, 0-indexed.</param>
        /// <returns>True if the row is in bounds, false otherwise.</returns>
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

        /// <summary>
        /// Keeps the given object within the boundaries of the room.
        /// </summary>
        /// <param name="obj">The object to keep in bounds.</param>
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

        #region Tile Operations

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

        /// <summary>
        /// Gets the coordinates of a tile by a pixel.
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns></returns>
        private Vector2 GetTileCoordsByPixel(Vector2 pixel)
        {
            return GetTileCoordsByPixel(pixel.X, pixel.Y);
        }

        /// <summary>
        /// Gets a tile by a pixel.
        /// </summary>
        /// <param name="pixel">A coordinate on the game.</param>
        /// <returns>The tile at that coordinate.</returns>
        private Tile GetTileByPixel(Vector2 pixel)
        {
            return GetTileByPixel(pixel.X, pixel.Y);
        }

        /// <summary>
        /// Gets a tile by a pixel.
        /// </summary>
        /// <param name="x">The x-coordinate of the pixel.</param>
        /// <param name="y">The y-coordinate of the pixel.</param>
        /// <returns>The tile at the given coordinates.</returns>
        private Tile GetTileByPixel(float x, float y)
        {
            Vector2 tileCoords = GetTileCoordsByPixel(x, y);
            return tiles[(int)tileCoords.X, (int)tileCoords.Y];
        }

        /// <summary>
        /// Gets the column-row coordinates of a tile by a pixel.
        /// </summary>
        /// <param name="x">The x-coordinate of the pixel.</param>
        /// <param name="y">The y-coordinate of the pixel.</param>
        /// <returns>The column-row coordinates of the tile at the 
        /// given coordinates.</returns>
        private Vector2 GetTileCoordsByPixel(float x, float y)
        {
            return new Vector2((int)(x / TILESIZE), (int)(y / TILESIZE));
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
        /// Returns whether or not there is a wall at the given column and row.
        /// </summary>
        /// <param name="row">The row of the tile, 0-indexed.</param>
        /// <param name="col">The column of the tile, 0-indexed.</param>
        /// <returns>True if there is a wall at the given row/column, false otherwise.</returns>
        private bool WallAt(int col, int row)
        {
            return tiles[col, row].GetTileType() == TileType.Wall;
        }

        /// <summary>
        /// Return whether or not a wall overlaps the given object.
        /// </summary>
        /// <param name="box">The bounding box that might be overlapping a wall.</param>
        /// <returns>True if the box overlaps a wall, false otherwise.</returns>
        private bool WallIntersects(GameObject obj)
        {
            // TODO: check every tile that the box overlaps, not just the corners.
            return WallAtPixel(obj.BBox.Left, obj.BBox.Top) || WallAtPixel(obj.BBox.Right, obj.BBox.Top) ||
                WallAtPixel(obj.BBox.Left, obj.BBox.Bottom) || WallAtPixel(obj.BBox.Right, obj.BBox.Bottom);
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
        #endregion

        /// <summary>
        /// Renders the texture of the room's foreground.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device managing the game's graphics.</param>
        public void Render(GraphicsDevice graphicsDevice)
        {
            if (texture == null)
                texture = new Texture2D(graphicsDevice, Width * TILESIZE, Height * TILESIZE);
            Render(graphicsDevice, unrenderedRegion);
        }

        /// <summary>
        /// Renders a region of the texture of the room's foreground.
        /// Assumes the texture is not null.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device managing the game's graphics.</param>
        /// <param name="region">The region to render.</param>
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

        /// <summary>
        /// Whether or not the room's foreground texture needs to be re-rendered.
        /// </summary>
        /// <returns>True if the foreground needs to be re-rendered, 
        /// false otherwise.</returns>
        public bool ShouldRender()
        {
            return unrenderedRegion != Rectangle.Empty;
        }
    }
}
