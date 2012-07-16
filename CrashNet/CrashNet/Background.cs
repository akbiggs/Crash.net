using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CrashNet
{
    class Background
    {
        public int Width;
        public int Height;

        Texture2D texture;
        bool isTiled;
        bool shouldReRender;

        public Background(int Width, int Height, Texture2D bgTexture, bool IsTiled=false)
        {
            this.Width = Width;
            this.Height = Height;
            
            isTiled = IsTiled;
        }

        internal void Update()
        {
            throw new NotImplementedException();
        }

        /**
         * Renders the background based on the given texture.
         * For example, if the background is to be tiled based on the given texture,
         * returns a tiled texture the width and height of the whole background.
         */
        /*internal void Render(GraphicsDevice graphicsDevice)
        {
            
            if (!isTiled) {
                //lay out the given texture as tiles
                Color[,] paint = new Color[texture.Width, texture.Height];
                Color[] canvas = new Color[this.Width * this.Height];
                for (int x = 0; x < this.Width; x++)
                    for (int y = 0; y < this.Height; y++)
                        canvas[x + y * this.Width] = paint[x % texture.Width, y % texture.Height];

                Texture2D result = new Texture2D(graphicsDevice, this.Width, this.Height);
                result.SetData(canvas);
            }
        }*/

        internal void Draw(SpriteBatch spriteBatch)
        {
            
        }
    }
}
