using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CrashNet
{
    class Background : Renderable
    {
        public int Width;
        public int Height;

        bool isTiled;
        Texture2D texture;

        Texture2D unrendered = null;

        /// <summary>
        /// Make a new background of the given width, height, and showing
        /// the given texture.
        /// </summary>
        /// <param name="Width">The width of the background.</param>
        /// <param name="Height">The height of the background.</param>
        /// <param name="texture">The texture of the background.</param>
        /// <param name="isTiled">Whether or not the texture of the background
        /// should be tiled.</param>
        public Background(int Width, int Height, Texture2D texture, bool isTiled=false)
        {
            this.Width = Width;
            this.Height = Height;

            this.isTiled = isTiled;
            Texture = texture;
        }

        internal void Update()
        {
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            if (ShouldRender())
            {
                Render(spriteBatch.GraphicsDevice);
            }

            spriteBatch.Draw(texture, new Vector2(0, 0), Color.White);
        }

        /// <summary>
        /// Re-render the background.
        /// For example, if the texture changes, tiles the background based on the
        /// new texture.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device of the game.</param>
        public void Render(GraphicsDevice graphicsDevice)
        {
            Render(graphicsDevice, new Rectangle(0, 0, Width, Height));
        }

        /// <summary>
        /// Re-render a region of the background.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device of the game.</param>
        /// <param name="region">The area of the background to render.</param>
        public void Render(GraphicsDevice graphicsDevice, Rectangle region)
        {
            if (!isTiled) texture = unrendered;
            else {
                //lay out the given texture as tiles
                Color[,] paint = new Color[texture.Width, texture.Height];
                Color[] canvas = new Color[region.Width * region.Height];
                for (int x = 0; x < region.Width; x++)
                    for (int y = 0; y < region.Height; y++)
                        canvas[x + y * region.Width] = paint[(x + region.X) % unrendered.Width,
                            (y + region.Y) % unrendered.Height];

                //store the newly generated background texture
                texture = new Texture2D(graphicsDevice, this.Width, this.Height);
                texture.SetData(0, region, canvas, 0, region.Width * region.Height);
            }

            unrendered = null;
        }

        /// <summary>
        /// Whether or not the background needs to be re-rendered.
        /// </summary>
        /// <returns>True if the background needs to be re-rendered,
        /// false otherwise.</returns>
        public bool ShouldRender()
        {
            return unrendered != null;
        }

        /// <summary>
        /// The texture of the background.
        /// </summary>
        public Texture2D Texture
        {
            get
            {
                return texture;
            }
            set
            {
                unrendered = value;
            }
        }
    }
}
