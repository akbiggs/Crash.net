using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CrashNet.Worlds;
using CrashNet.GameObjects;
using CrashNet.Engine;

namespace CrashNet.Engine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private int width;
        private int height;

        GameState state;

        World world;
        Cutscene curCutscene;

        MainMenu mainMenu;
        GameMenu gameMenu;
        UserInterface ui;

        Background background;

        public Game(int Width, int Height, bool IsFullScreen=false)
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = IsFullScreen;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";

            this.Width = Width;
            this.Height = Height;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            state = GameState.Level;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            TextureManager.LoadContents(Content);
            FontManager.LoadContents(Content);

            //initializing here because they are dependent on content managers
            background = new Background(Width, Height, TextureManager.GetTexture(TextureNames.BACKGROUND));

            world = WorldGenerator.Generate(WorldNumber.One);
            world.Add(new Player(PlayerNumber.One, new Vector2(200, 200)));
            world.Add(new Player(PlayerNumber.Two, new Vector2(100, 100)));

            ui = new UserInterface();

            mainMenu = new MainMenu();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (IsActive)
                Input.Update();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Update components based on the mode the game is in.
            switch (state)
            {
                case GameState.Level:
                case GameState.Boss:
                    background.Update(gameTime);
                    world.Update(gameTime);
                    ui.Update(gameTime);
                    break;

                case GameState.Cutscene:
                    curCutscene.Update(gameTime);
                    break;

                case GameState.GameMenu:
                    background.Update(gameTime);
                    gameMenu.Update(gameTime);
                    break;

                case GameState.MainMenu:
                    background.Update(gameTime);
                    mainMenu.Update(gameTime);
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            // Render components based on the mode the game is in.
            switch (state)
            {
                case GameState.Level:
                case GameState.Boss:
                    background.Draw(spriteBatch);
                    world.Draw(spriteBatch);
                    ui.Draw(spriteBatch);
                    break;

                case GameState.Cutscene:
                    curCutscene.Draw(spriteBatch);
                    break;

                case GameState.GameMenu:
                    background.Draw(spriteBatch);
                    world.Draw(spriteBatch);
                    ui.Draw(spriteBatch);
                    gameMenu.Draw(spriteBatch);
                    break;

                case GameState.MainMenu:
                    background.Draw(spriteBatch);
                    mainMenu.Draw(spriteBatch);
                    break;
            }

            base.Draw(gameTime);

            spriteBatch.End();
        }

        public int Width {
            get { return width; }
            set
            {
                width = value;
                if (background != null) background.Width = width;
                graphics.PreferredBackBufferWidth = width;
            }
        }

        public int Height {
            get { return height; }
            set
            {
                height = value;
                if (background != null) background.Height = height;
                graphics.PreferredBackBufferHeight = height;
            }
        }
    }

    enum GameState
    {
        Cutscene,
        Level,
        Boss,
        GameMenu,
        MainMenu
    }
}
