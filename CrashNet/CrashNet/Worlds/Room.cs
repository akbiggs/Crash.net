﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CrashNet.GameObjects;
using CrashNet.Engine;
using System.Timers;
using Microsoft.Xna.Framework.Input;
using System.IO;


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
        const float WALL_PADDING = 0.1f;

        /// <summary>
        /// How many tiles should be free next to each exit.
        /// </summary>
        const int EXIT_PADDING = 2;

        /// <summary>
        /// The folder for rooms to be saved in.
        /// </summary>
        const string ROOM_FOLDER = "..\\..\\..\\Worlds\\Rooms\\";

        /// <summary>
        /// The savename of the room file.
        /// </summary>
        const string SAVE_NAME = "Room.csv";

        /// <summary>
        /// All the objects in the room.
        /// </summary>
        List<GameObject> objects;

        /// <summary>
        /// Objects being added to room after update cycle. 
        /// </summary>
        List<GameObject> addedObjects;

        /// <summary>
        /// Objects to be removed from room after
        /// update cycle.
        /// </summary>
        List<GameObject> removedObjects;

        /// <summary>
        /// All of the objects that want to leave the room.
        /// </summary>
        Dictionary<Direction, List<GameObject>> wantingToLeave;

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

        bool editMode = false;

        /// <summary>
        /// Makes a new room.
        /// </summary>
        /// <param name="width">The width of the room in tiles.</param>
        /// <param name="height">The height of the room in tiles.</param>
        /// <param name="exits">The directions in which one can exit from the room.</param>
        public Room(int width, int height, List<Direction> exits)
        {
            this.Width = width;
            this.Height = height;

            this.objects = new List<GameObject>();
            this.addedObjects = new List<GameObject>();
            this.removedObjects = new List<GameObject>();
            ClearLeavingObjects();
            
            tiles = new Tile[Width, Height];
            for (int col = 0; col < Width; col++)
                for (int row = 0; row < Height; row++)
                    SetTile(col, row, TileType.Ground);

            SetBorder(exits);
        }

        public Room(Tile[,] tiles) :
            this(tiles.GetLength(0), tiles.GetLength(1), new List<Direction>())
        {
            this.tiles = tiles;
        }


        #region Initialization
        /// <summary>
        /// Initializes the list of objects wanting to leave the room.
        /// </summary>
        private void ClearLeavingObjects()
        {
            this.wantingToLeave = new Dictionary<Direction, List<GameObject>>();
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                wantingToLeave[direction] = new List<GameObject>();
        }

        /// <summary>
        /// Generates the border of the room based on the given
        /// list of exits.
        /// </summary>
        /// <param name="exits">The exits from the room.</param>
        private void SetBorder(List<Direction> exits)
        {
            int x, y;

            // TODO: generalize these into a method.
            // generate north border
            y = 0;
            for (x = 0; x < Width; x++)
                if (MathHelper.Distance(x, 0) < EXIT_PADDING && exits.Contains(Direction.NorthWest))
                    SetTile(x, y, TileType.Ground);
                else if (MathHelper.Distance(x, Width / 2) < EXIT_PADDING && exits.Contains(Direction.North))
                    SetTile(x, y, TileType.Ground);
                else if (MathHelper.Distance(x, Width - 1) < EXIT_PADDING && exits.Contains(Direction.NorthEast))
                    SetTile(x, y, TileType.Ground);
                else
                    SetTile(x, y, TileType.Wall);

            // generate south border
            y = Height - 1;
            for (x = 0; x < Width; x++)
                if (MathHelper.Distance(x, 0) < EXIT_PADDING && exits.Contains(Direction.SouthWest))
                    SetTile(x, y, TileType.Ground);
                else if (MathHelper.Distance(x, Width / 2) < EXIT_PADDING && exits.Contains(Direction.South))
                    SetTile(x, y, TileType.Ground);
                else if (MathHelper.Distance(x, Width - 1) < EXIT_PADDING && exits.Contains(Direction.SouthEast))
                    SetTile(x, y, TileType.Ground);
                else
                    SetTile(x, y, TileType.Wall);

            // generate west border
            x = 0;
            for (y = 0; y < Height; y++)
                if (MathHelper.Distance(y, 0) < EXIT_PADDING && exits.Contains(Direction.NorthWest))
                    SetTile(x, y, TileType.Ground);
                else if (MathHelper.Distance(y, Height / 2) < EXIT_PADDING && exits.Contains(Direction.West))
                    SetTile(x, y, TileType.Ground);
                else if (MathHelper.Distance(y, Height - 1) < EXIT_PADDING && exits.Contains(Direction.SouthWest))
                    SetTile(x, y, TileType.Ground);
                else
                    SetTile(x, y, TileType.Wall);
 
            // generate east border
            x = Width - 1;
            for (y = 0; y < Height; y++)
                if (MathHelper.Distance(y, 0) < EXIT_PADDING && exits.Contains(Direction.NorthEast))
                    SetTile(x, y, TileType.Ground);
                else if (MathHelper.Distance(y, Height / 2) < EXIT_PADDING && exits.Contains(Direction.East))
                    SetTile(x, y, TileType.Ground);
                else if (MathHelper.Distance(y, Height - 1) < EXIT_PADDING && exits.Contains(Direction.SouthEast))
                    SetTile(x, y, TileType.Ground);
                else
                    SetTile(x, y, TileType.Wall);
        }
        #endregion

        /// <summary>
        /// Gets the width of the room in pixels.
        /// </summary>
        /// <returns>The width of the room in pixels.</returns>
        public int GetWidthInPixels()
        {
            return Width * TILESIZE;
        }

        /// <summary>
        /// Gets the height of the room in pixels.
        /// </summary>
        /// <returns>The height of the room in pixels.</returns>
        public int GetHeightInPixels()
        {
            return Height * TILESIZE;
        }

        /// <summary>
        /// Updates the room.
        /// </summary>
        internal virtual void Update(GameTime gameTime)
        {
            #region LEVEL EDITOR
            
#if DEBUG 
            if (Input.KeyboardTapped(Keys.F1))
                Save();

            if (Input.KeyboardTapped(Keys.F2))
                editMode = !editMode;

            if (editMode)
            {
                if (Input.MouseLeftButtonDown)
                {
                    Vector2 mousePos = Input.MousePosition;
                    Vector2 tileCoords = GetTileCoordsByPixel(mousePos.X, mousePos.Y);
                    SetTile((int)tileCoords.X, (int)tileCoords.Y, TileType.Wall);
                }

                if (Input.MouseRightButtonDown)
                {
                    Vector2 mousePos = Input.MousePosition;
                    Vector2 tileCoords = GetTileCoordsByPixel(mousePos.X, mousePos.Y);
                    SetTile((int)tileCoords.X, (int)tileCoords.Y, TileType.Ground);
                }
            }
#endif
            #endregion

            foreach (Tile tile in tiles)
                tile.Update(gameTime, this);

            for (int i = 0; i < objects.Count; i++)
            {
                /**
                 * If object is alive, update it.
                 * Otherwise, add to the remove queue.
                 **/
                if (objects[i].IsAlive())
                {
                    objects[i].Update(gameTime, this);
                    KeepInBounds(objects[i]);

                    // check if the object wants to leave the room,
                    // update our leaving tracker if it does.
                    Direction leavingDirection = Direction.None;
                    if (AtEdge(objects[i], out leavingDirection))
                        wantingToLeave[leavingDirection].Add(objects[i]);

                    // it does not want to leave in more than one direction,
                    // and if it no longer wants to leave the room remove it from the
                    // tracker.
                    foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                        if (direction != leavingDirection)
                            wantingToLeave[direction].Remove(objects[i]);

                    // collide with any other objects in the room
                    foreach (GameObject other in objects)
                    {
                        BBox region;
                        if (objects[i] != other && objects[i].ShouldCollide(other, out region))
                            objects[i].Collide(other, region);
                    }
                }
                else // If object is not alive:
                {
                    RemoveAfterUpdate(objects[i]);
                }
            }

            // add all objects queued to be added
            foreach (GameObject obj in addedObjects)
            {
                objects.Add(obj);
            }
            // Clear queued objects:
            addedObjects.Clear();

            // remove all objects queued to be removed
            foreach (GameObject obj in removedObjects)
            {
                objects.Remove(obj);
            }
            // Clear queued objects:
            removedObjects.Clear();
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            if (ShouldRender())
                Render(spriteBatch.GraphicsDevice);

            spriteBatch.Draw(texture, new Vector2(0, 0), Color.White);

            foreach (GameObject obj in objects)
                obj.Draw(spriteBatch);

            if (editMode)
                spriteBatch.DrawString(FontManager.GetFont(FontNames.MAIN_MENU_FONT),
                    "EDIT MODE", new Vector2((Width - 4) * TILESIZE, (Height - 1) * TILESIZE),
                    Color.White);
        }

        /// <summary>
        /// Save this room to a file.
        /// </summary>
        private void Save()
        {
            // go through and write each tile to the comma-separated file
            StreamWriter writer = new StreamWriter(ROOM_FOLDER + SAVE_NAME);
            for (int y = 0; y < Height; y++)
            {
                List<string> row = new List<string>();
                for (int x = 0; x < Width; x++)
                    row.Add(tiles[x, y].ToString());
                writer.WriteLine(String.Join(", ", row) + ((y != Height - 1) ? "," : ""));
            }

            writer.Close();
        }

        /// <summary>
        /// Get the filename under which this room should be saved.
        /// </summary>
        /// <param name="saveDir">The directory into which the room is being 
        /// saved.</param>
        /// <returns>The filename to save the room as.</returns>
        private string GetSaveName(string saveDir)
        {
            String nextName = "Room.csv";
            while (Directory.EnumerateFiles(saveDir).Contains(nextName))
                nextName = String.Concat("Room", nextName);
            return nextName;
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
        /// Whether or not the room should be exited.
        /// </summary>
        /// <param name="direction">The direction in which to leave the room, or
        /// Direction.None if the room should not be left in any direction.</param>
        /// <returns>True if the room should be exited, false otherwise.</returns>
        public bool ShouldLeave(out Direction direction)
        {
            foreach (KeyValuePair<Direction, List<GameObject>> pair in wantingToLeave)
            {
                // if the players are both trying to leave in the same direction,
                // leave in that direction.
                // TODO: make this subset checking thing into a helper method in a separate
                // class.
                if (!GetPlayers().Cast<GameObject>().Except(pair.Value).Any())
                {
                    direction = pair.Key;
                    return true;
                }
            }

            direction = Direction.None;
            return false;
        }

        /// <summary>
        /// Cleans up the room behind the players as they leave.
        /// </summary>
        public void Leave()
        {
            ClearLeavingObjects();
            RemovePlayers();
        }

        #region Object Operations
        /// <summary>
        /// Adds the given object to the objects list.
        /// </summary>
        /// <param name="obj">The object to be put in the room.</param>
        public void Add(GameObject obj)
        {
            objects.Add(obj);
        }

        /// <summary>
        /// Adds given object to the room after the update has finished.
        /// Safer than Add to call during an update.
        /// </summary>
        public void AddAfterUpdate(GameObject obj)
        {
            addedObjects.Add(obj);
        }

        /// <summary>
        /// Removes the given object from the room.
        /// </summary>
        /// <param name="obj">The object to be removed from the room.</param>
        public void Remove(GameObject obj)
        {
            objects.Remove(obj);
        }

        /// <summary>
        /// Removes the given object from the room after the update has finished.
        /// Safer than Remove to call during an update.
        /// </summary>
        /// <param name="obj">The object to be removed.</param>
        public void RemoveAfterUpdate(GameObject obj)
        {
            removedObjects.Add(obj);
        }

        /// <summary>
        /// Return whether or not the given object is out of bounds.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <param name="violationDirection">The direction in which the object is out of bounds,
        /// or Direction.None if the object is not out of bounds.</param>
        /// <returns>True if the object is out of bounds, false otherwise.</returns>
        public bool OutOfBounds(GameObject obj, out Direction violationDirection)
        {
            violationDirection = Direction.None;

            // get the direction in which the object is out of bounds, if any at all
            // priority: West > North > East > South
            if (obj.Position.X < 0)
                violationDirection = Direction.West;
            else if (obj.Position.Y < 0)
                violationDirection = Direction.North;
            else if (obj.Position.X + obj.BBox.Width >= GetWidthInPixels())
                violationDirection = Direction.East;
            else if (obj.Position.Y + obj.BBox.Height >= GetHeightInPixels())
                violationDirection = Direction.South;

            return violationDirection == Direction.None;
        }

        /// <summary>
        /// Keeps the given object within the boundaries of the room.
        /// </summary>
        /// <param name="obj">The object to keep in bounds.</param>
        public void KeepInBounds(GameObject obj)
        {
            // if the object went outside the boundaries of the room, collide it with a wall and push
            // it back in.
            Vector2 oldPosition = obj.Position;
            obj.Position = Vector2.Clamp(obj.Position, Vector2.Zero,
                new Vector2(GetWidthInPixels() - obj.BBox.Width - 1, GetHeightInPixels() - obj.BBox.Height - 1));
            if (oldPosition != obj.Position) 
            {
                // TODO: properly create a region of collision corresponding to the amount of the exit overlapped
                Vector2 change = new Vector2(Math.Abs(oldPosition.X - obj.Position.X),
                    Math.Abs(oldPosition.Y - obj.Position.Y));
                obj.Collide(new Tile(TileType.Wall), new BBox(oldPosition, (int)change.X, (int)change.Y));
            }
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
        /// Check if the given object is at the edge of the room.
        /// </summary>
        /// <param name="obj">The object in the room to check.</param>
        /// <param name="direction">The edge of the room that the object is at, or
        /// Direction.None if the object is not at the edge of the room.</param>
        /// <returns>True if the object is at the edge of the room, false otherwise.</returns>
        private bool AtEdge(GameObject obj, out Direction direction)
        {
            // use the center of the object's bounding box as the telltale point of whether or not
            // the object is at the edge.
            Vector2 coords = GetTileCoordsByPixel(new Vector2(obj.BBox.Center.X, obj.BBox.Center.Y));
            direction = Direction.None;

            // check diagonal edges first
            if (coords.X == 0 && coords.Y == 0)
                direction = Direction.NorthWest;
            else if (coords.X == Width - 1 && coords.Y == 0)
                direction = Direction.NorthEast;
            else if (coords.X == 0 && coords.Y == Height - 1)
                direction = Direction.SouthWest;
            else if (coords.X == Width - 1 && coords.Y == Height - 1)
                direction = Direction.SouthEast;

            // check the other edges
            else if (coords.X == 0)
                direction = Direction.West;
            else if (coords.X == Width - 1)
                direction = Direction.East;
            else if (coords.Y == 0)
                direction = Direction.North;
            else if (coords.Y == Height - 1)
                direction = Direction.South;

            return direction != Direction.None;
        }

        /// <summary>
        /// Get the players in the room.
        /// </summary>
        /// <returns>A list of the player characters in the room.</returns>
        public List<Player> GetPlayers()
        {
            return objects.Where(x => x is Player).Cast<Player>().ToList();
        }

        /// <summary>
        /// Remove the players from the room.
        /// </summary>
        private void RemovePlayers()
        {
            objects = objects.Where(x => !(x is Player)).ToList();
        }
        #endregion

        #region Tile Operations

        /// <summary>
        /// Sets the tile at the given coordinates to the given tile.
        /// </summary>
        /// <param name="col">The x-coordinate of the new tile, relative to other tiles.</param>
        /// <param name="row">The y-coordinate of the new tile, relative to other tiles.</param>
        /// <param name="tiletype">The new tile.</param>
        private void SetTile(int col, int row, TileType tiletype)
        {
            Tile tile = new Tile(new Vector2(col * TILESIZE, row * TILESIZE), tiletype);
            if (ValidRow(row) && ValidCol(col))
            {
                if (tiles == null) tiles = new Tile[Width, Height];
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

        #region Foreground Rendering
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
        #endregion
    }
}
