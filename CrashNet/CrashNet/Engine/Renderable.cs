using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CrashNet
{
    /// <summary>
    /// Used for anything whose texture does not need to be redrawn every frame. 
    /// </summary>
    public interface Renderable
    {
        Texture2D Texture { get; set; }

        void Render(GraphicsDevice graphicsDevice);
        bool ShouldRender();
    }
}
