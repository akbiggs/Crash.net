using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using CrashNet.Engine;

namespace CrashNet.GameObjects
{
    class Player : GameObject
    {
        const float MAX_VELOCITY_X = 10;
        const float MAX_VELOCITY_Y = 10;
        const float ACCELERATION_X = 2;
        const float ACCELERATION_Y = 2;

        PlayerNumber playerNumber;
        
        // TODO: Figure out how to move constants for acceleration and max velocity
        // here into actual constants. Maybe read them in from file? Have a property
        // manager?
        public Player(PlayerNumber playerNumber, Vector2 position)
            : base(position, Vector2.Zero, new Vector2(MAX_VELOCITY_X, MAX_VELOCITY_Y), 
            new Vector2(ACCELERATION_X, ACCELERATION_Y),
            TextureManager.GetTexture(TextureNames.PLAYER_ONE_IDLE))
        {
            this.PlayerNumber = playerNumber;
        }

        internal override void Update()
        {
            if (Input.IsKeyDown(Keys.Up) && Input.IsKeyDown(Keys.Left))
                Move(Direction.NorthWest);
            else if (Input.IsKeyDown(Keys.Up) && Input.IsKeyDown(Keys.Right))
                Move(Direction.NorthEast);
            else if (Input.IsKeyDown(Keys.Down) && Input.IsKeyDown(Keys.Left))
                Move(Direction.SouthWest);
            else if (Input.IsKeyDown(Keys.Down) && Input.IsKeyDown(Keys.Right))
                Move(Direction.SouthEast);
            else if (Input.IsKeyDown(Keys.Left))
                Move(Direction.West);
            else if (Input.IsKeyDown(Keys.Right))
                Move(Direction.East);
            else if (Input.IsKeyDown(Keys.Up))
                Move(Direction.North);
            else if (Input.IsKeyDown(Keys.Down))
                Move(Direction.South);

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
