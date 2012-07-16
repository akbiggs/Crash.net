using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CrashNet
{
    static class TextureManager
    {
        // Stores textures
        static Dictionary<String, Texture2D> texDic = new Dictionary<String, Texture2D>();
        // Stores representations of textures as colors(useful for rendering)
        static Dictionary<String, Color[,]> colDic = new Dictionary<String, Color[,]>();

        static List<string> textureNames = new List<String>
        {
            // Players
            TextureNames.PLAYER_ONE_IDLE,
            TextureNames.PLAYER_TWO_IDLE,

            // Enemies
            TextureNames.ENEMY,

            // Levels
            TextureNames.GROUND,
            TextureNames.WALL,

            // Misc.
            TextureNames.BLANK
        };
                                               

        public static void LoadContents(ContentManager content)
        {
            foreach (string name in textureNames)
            {
                Texture2D texture = content.Load<Texture2D>(name);
                texDic[name] = texture;
                colDic[name] = TextureToColor2D(texture);
            }
        }

        public static Texture2D GetTexture(string name)
        {
            return texDic[name];
        }

        public static Color[,] GetColors(string name)
        {
            return colDic[name];
        }

        private static Color[,] TextureToColor2D(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];

            return colors2D;
        }
    }

    //stores constants for texture names
    static class TextureNames
    {
        public const string PLAYER_ONE_IDLE = "player_one";
        public const string PLAYER_TWO_IDLE = "player_two";
        public const string ENEMY = "enemy";
        public const string GROUND = "ground";
        public const string WALL = "wall";
        public const string BACKGROUND = "background";
        public const string BLANK = "blank";
    }
}
