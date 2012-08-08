using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CrashNet
{
    /// <summary>
    /// Used for anything whose texture needs to be rendered before it can be
    /// displayed, but which does not need to be re-rendered on every single
    /// frame.
    /// </summary>
    public interface Renderable
    {
        void Render(GraphicsDevice graphicsDevice);
        bool ShouldRender();
    }
}
