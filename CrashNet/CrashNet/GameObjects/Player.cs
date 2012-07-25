using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CrashNet.GameObjects
{
    class Player : GameObject
    {
        PlayerNumber playerNumber;

        public Player(PlayerNumber playerNumber, Vector2 position)
            : base(position, Vector2.Zero, TextureManager.GetTexture(TextureNames.PLAYER_ONE_IDLE))
        {
            this.PlayerNumber = playerNumber;
        }

        internal override void Update()
        {
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Left))
                Position = Vector2.Add(Position, new Vector2(-10, 0));
                //Move(Direction.West);
            if (keyState.IsKeyDown(Keys.Right))
                Position = Vector2.Add(Position, new Vector2(10, 0)); 
                //Move(Direction.East);

            if (keyState.IsKeyDown(Keys.Up))
                Position = Vector2.Add(Position, new Vector2(0, -10));
                //Move(Direction.North);
            if (keyState.IsKeyDown(Keys.Down))
                Position = Vector2.Add(Position, new Vector2(0, 10));
                //Move(Direction.South);

            base.Update();
        }

        public PlayerNumber PlayerNumber
        {
            get { return playerNumber; }
            private set { playerNumber = value; }
        }
    }

    public enum PlayerNumber
    {
        One,
        Two
    }
}
