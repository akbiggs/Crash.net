using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CrashNet
{
    static class FontManager
    {
        private static Dictionary<String, SpriteFont> fontDic = new Dictionary<string, SpriteFont>();

        // the directory in which the fonts are stored.
        static string dir = "Fonts\\";

        private static List<String> fontNames = new List<String> {
            //Menus
            FontNames.MAIN_MENU_FONT
        };

        /// <summary>
        /// Load in all the fonts the game will use.
        /// </summary>
        /// <param name="content">Content manager for the game.</param>
        public static void LoadContents(ContentManager content)
        {
            foreach (String name in fontNames)
            {
                fontDic[name] = content.Load<SpriteFont>(dir + name);
            }
        }

        /// <summary>
        /// Gets a font.
        /// </summary>
        /// <param name="name">The name of the font.</param>
        /// <returns>The specified font.</returns>
        public static SpriteFont GetFont(String name)
        {
            return fontDic[name];
        }
    }

    static class FontNames
    {
        public const String MAIN_MENU_FONT = "main_menu_font";
    }
}
