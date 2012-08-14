using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CrashNet
{
    class MainMenu : Menu
    {
        internal void Update(GameTime gameTime)
        {
        }

        internal void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(FontManager.GetFont(FontNames.MAIN_MENU_FONT), "Main Menu", 
                new Vector2(0, 0), Color.White);
        }
    }
}
