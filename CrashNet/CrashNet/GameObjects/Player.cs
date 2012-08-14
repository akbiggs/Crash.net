using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using CrashNet.Engine;
using CrashNet.Worlds;
using CrashNet.ParticleSystem;

namespace CrashNet.GameObjects
{
    class Player : GameObject
    {
        // movement constants
        const float MAX_VELOCITY_X = 10;
        const float MAX_VELOCITY_Y = 10;
        const float ACCELERATION_X = 2;
        const float ACCELERATION_Y = 2;
        const float DECELERATION_X = 4;
        const float DECELERATION_Y = 4;

        PlayerNumber playerNumber;
        Keys Up, Down, Left, Right, Fire;
        /**
         * Added a direction to keep track of player facing for
         * the purpose of projectile firin'
         **/
        Direction currentDirection;

        bool IsAlive = true;

        /// <summary>
        /// Make a new player.
        /// </summary>
        /// <param name="playerNumber">The player's number(i.e. player one, or player two)</param>
        /// <param name="position">The starting position of the player.</param>
        public Player(PlayerNumber playerNumber, Vector2 position)
            : base(position, Vector2.Zero, Vector2.Zero, new Vector2(MAX_VELOCITY_X, MAX_VELOCITY_Y), 
            new Vector2(ACCELERATION_X, ACCELERATION_Y), new Vector2(DECELERATION_X, DECELERATION_Y),
            playerNumber == PlayerNumber.One ? 
                TextureManager.GetTexture(TextureNames.PLAYER_ONE_IDLE) :
                TextureManager.GetTexture(TextureNames.PLAYER_TWO_IDLE))
        {
            this.PlayerNumber = playerNumber;
            this.origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            if (PlayerNumber == PlayerNumber.One)
            {
                Up = Keys.Up;
                Down = Keys.Down;
                Right = Keys.Right;
                Left = Keys.Left;
                Fire = Keys.RightControl;
            }
            else
            {
                Up = Keys.W;
                Down = Keys.S;
                Right = Keys.D;
                Left = Keys.A;
                Fire = Keys.LeftControl;
            }
        }

        internal override void Update(GameTime gameTime, Room room)
        {
            if (IsAlive)
            {
                List<Direction> directions = new List<Direction>();
                if (Input.IsKeyDown(Up))
                    directions.Add(Direction.North);
                if (Input.IsKeyDown(Down))
                    directions.Add(Direction.South);
                if (Input.IsKeyDown(Left))
                    directions.Add(Direction.West);
                if (Input.IsKeyDown(Right))
                    directions.Add(Direction.East);

                Direction direction = DirectionOperations.Combine(directions);
                Move(direction, gameTime, room);
                if (direction != Direction.None)
                    currentDirection = direction;
                
                if (Input.KeyboardTapped(Fire))
                    FireProjectile(room);
                
                base.Update(gameTime, room);
            }
        }

        /**
         * Adds a projectile to the room's GameObject queue if
         * fire conditions are met. Ie: Not firing too frequently,
         * player isn't dead, etc.
         **/
        private void FireProjectile(Room room)
        {
            /** 
             * Projectiles will be added from pre-defined classes 
             * which have all of their parameters already set 
             * except for position and an initial velocity. These will
             * be taken from the player's position and facing.
             **/
         
        }

        /// <summary>
        /// Kills the player.
        /// </summary>
        private void Die()
        {
            IsAlive = false;
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
