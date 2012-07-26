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
        Keys Up, Down, Left, Right;

        /// <summary>
        /// Make a new player.
        /// </summary>
        /// <param name="playerNumber">The player's number(i.e. player one, or player two)</param>
        /// <param name="position">The starting position of the player.</param>
        public Player(PlayerNumber playerNumber, Vector2 position)
            : base(position, Vector2.Zero, new Vector2(MAX_VELOCITY_X, MAX_VELOCITY_Y), 
            new Vector2(ACCELERATION_X, ACCELERATION_Y),
            playerNumber == PlayerNumber.One ? 
                TextureManager.GetTexture(TextureNames.PLAYER_ONE_IDLE) :
                TextureManager.GetTexture(TextureNames.PLAYER_TWO_IDLE))
        {
            this.PlayerNumber = playerNumber;
            if (PlayerNumber == PlayerNumber.One)
            {
                Up = Keys.Up;
                Down = Keys.Down;
                Right = Keys.Right;
                Left = Keys.Left;
            }
            else
            {
                Up = Keys.W;
                Down = Keys.S;
                Right = Keys.D;
                Left = Keys.A;
            }
        }

        internal override void Update()
        {
            if (Input.IsKeyDown(Up) && Input.IsKeyDown(Left))
                Move(Direction.NorthWest);
            else if (Input.IsKeyDown(Up) && Input.IsKeyDown(Right))
                Move(Direction.NorthEast);
            else if (Input.IsKeyDown(Down) && Input.IsKeyDown(Left))
                Move(Direction.SouthWest);
            else if (Input.IsKeyDown(Down) && Input.IsKeyDown(Right))
                Move(Direction.SouthEast);
            else if (Input.IsKeyDown(Left))
                Move(Direction.West);
            else if (Input.IsKeyDown(Right))
                Move(Direction.East);
            else if (Input.IsKeyDown(Up))
                Move(Direction.North);
            else if (Input.IsKeyDown(Down))
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
