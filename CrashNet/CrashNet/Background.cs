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

        bool shouldRender;
        Texture2D unrendered;

        public Background(int Width, int Height, Texture2D bgTexture, bool isTiled=false)
        {
            this.Width = Width;
            this.Height = Height;

            this.isTiled = isTiled;
            Texture = bgTexture;
        }

        internal void Update()
        {
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            if (shouldRender)
            {
                Render(spriteBatch.GraphicsDevice);
                shouldRender = false;
            }

            spriteBatch.Draw(texture, new Vector2(0, 0), Color.White);
        }

        public void Render(GraphicsDevice graphicsDevice)
        {
            Render(graphicsDevice, new Rectangle(0, 0, Width, Height));
        }
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

                texture = new Texture2D(graphicsDevice, this.Width, this.Height);
                texture.SetData(0, region, canvas, 0, region.Width * region.Height);
            }
        }

        public Texture2D Texture
        {
            get
            {
                return texture;
            }
            set
            {
                unrendered = value;
                shouldRender = true;
            }
        }
    }
}
