using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CrashNet.Engine;
using CrashNet.Worlds;

namespace CrashNet.GameObjects
{
    class GameObject
    {
        //how much the player should be kept away from any walls
        const float WALL_PADDING = 0.1f;

        //used for calculating rotation angles
        const float DEFAULT_ROTATION_SPEED = (float)(Math.PI / 20);
        const double PI = MathHelper.Pi;
        const double PI_OVER_TWO = MathHelper.PiOver2;
        const double THREE_PI_OVER_TWO = 1.5 * PI;
        const double TWO_PI = MathHelper.TwoPi;
        //diagonals
        const double PI_OVER_FOUR = MathHelper.PiOver4;
        const double THREE_PI_OVER_FOUR = 0.75 * PI;
        const double FIVE_PI_OVER_FOUR = 1.25 * PI;
        const double SEVEN_PI_OVER_FOUR = 1.75 * PI;
        
        Vector2 position;
        internal Vector2 origin;

        /// <summary>
        /// The velocity of the object.
        /// </summary>
        public Vector2 Velocity;
        Vector2 maxVelocity;
        Vector2 acceleration;
        Vector2 deceleration;
         
        /// <summary>
        /// How much the object is rotated, in radians.
        /// A rotation of 0 is treated as a rotation of 2Pi.
        /// </summary>
        float rotation;
        float rotationChange = 0;
        float rotationSpeed;

        Texture2D texture; 
        internal BBox BBox = null;

        /// <summary>
        /// Make a new game object.
        /// </summary>
        /// <param name="position">The starting position of the object.</param>
        /// <param name="initialVelocity">The initial velocity of the object.</param>
        /// <param name="maxSpeed">The maximum velocity the object can obtain.</param>
        /// <param name="acceleration">The acceleration of the object.</param>
        /// <param name="deceleration">The deceleration of the object.</param>
        /// <param name="texture">The texture of the object.</param>
        /// <param name="rotation">How much the object should be rotated by.</param>
        /// <param name="rotationSpeed">The speed at which the object rotates.</param>
        public GameObject(Vector2 position, Vector2 origin, Vector2 initialVelocity, Vector2 maxSpeed, Vector2 acceleration, 
            Vector2 deceleration, Texture2D texture, float rotation=0, float rotationSpeed=DEFAULT_ROTATION_SPEED)
        {
            this.Position = position;
            this.origin = origin;

            this.Velocity = initialVelocity;
            this.maxVelocity = maxSpeed;
            this.acceleration = acceleration;
            this.deceleration = deceleration;

            this.Texture = texture;

            this.rotation = rotation;
            this.rotationSpeed = rotationSpeed;
        }

        internal virtual void Update()
        {
            if (ShouldRotate()) Rotate();
        }

        internal virtual void Update(Room room)
        {
            if (ShouldRotate()) Rotate();
        }

        internal virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, 
                new Rectangle((int)(Position.X + origin.X), (int)(Position.Y + origin.Y), Texture.Width, Texture.Height), 
                null, Color.White, rotation, origin, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Moves the object.
        /// </summary>
        /// <param name="direction">The direction in which to move the object.</param>
        internal virtual void Move(Direction direction)
        {
            float xComponent, yComponent;
            if (direction == Direction.None)
                xComponent = yComponent = 0;
            else
            {
                float angle = (float)DirectionToRadians(direction);
                RotateTo(angle);

                // There's this weird arithmetic bug where these produce very small values during movements along
                // the opposite axes, so round them.
                xComponent = (float)(acceleration.X * Math.Round(Math.Sin(angle), 6));
                // up is negative, down is positive, so multiply by -1. 
                yComponent = (float)(-1 * acceleration.Y * Math.Round(Math.Cos(angle), 6));
                
            }
            ChangeVelocity(new Vector2(xComponent, yComponent));
            Position = Vector2.Add(position, Velocity);
        }

        /// <summary>
        /// Moves the object in the given direction in the context of
        /// being in the given room, meaning the object will collide against
        /// any walls in the room.
        /// </summary>
        /// <param name="direction">The direction to move the object.</param>
        /// <param name="room">The room in which the object is moving around.</param>
        internal virtual void Move(Direction direction, Room room)
        {
            float xComponent, yComponent;
            if (direction == Direction.None)
                xComponent = yComponent = 0;
            else
            {
                float angle = (float)DirectionToRadians(direction);
                RotateTo(angle);

                // There's this weird arithmetic bug where these produce very small values during movements along
                // the opposite axes, so round them.
                xComponent = (float)(acceleration.X * Math.Round(Math.Sin(angle), 6));
                // up is negative, down is positive, so multiply by -1. 
                yComponent = (float)(-1 * acceleration.Y * Math.Round(Math.Cos(angle), 6));

            }
            ChangeVelocity(new Vector2(xComponent, yComponent));
            if (Velocity.X != 0)
                ChangeXPosition(Velocity.X, room);
            if (Velocity.Y != 0)
                ChangeYPosition(Velocity.Y, room);
        }

        private void ChangeYPosition(float amount, Room room)
        {
            // 1. change the y-pos.
            // 2. round to prevent jittering in some situations
            // 3. go through each intersection, and fix it.
            // Thanks to David Gouveia http://gamedev.stackexchange.com/users/11686/david-gouveia
            Position = new Vector2(Position.X, (float)Math.Round(Position.Y + amount));

            List<Tile> collidingWalls = room.GetIntersectingWalls(this);

            foreach (Tile tile in collidingWalls)
            {
                float depth = BBox.GetVerticalIntersectionDepth(BBox, tile.BBox);
                if (depth != 0)
                {
                    Velocity.Y = 0;
                    Position = new Vector2(Position.X, Position.Y + depth + (WALL_PADDING * Math.Sign(depth)));
                }
            }
        }

        private void ChangeXPosition(float amount, Room room)
        {
            // 1. change the x-pos.
            // 2. round to prevent jittering in some situations
            // 3. go through each intersection, and fix it.
            // Thanks to David Gouveia http://gamedev.stackexchange.com/users/11686/david-gouveia
            Position = new Vector2((float)Math.Round(Position.X + amount), Position.Y);
            List<Tile> collidingWalls = room.GetIntersectingWalls(this);

            foreach (Tile tile in collidingWalls)
            {
                float depth = BBox.GetHorizontalIntersectionDepth(BBox, tile.BBox);
                if (depth != 0)
                {
                    Velocity.X = 0;
                    Position = new Vector2(Position.X + depth + (WALL_PADDING * Math.Sign(depth)), Position.Y);
                }
            }
        }

        /// <summary>
        /// Converts the given direction into radians.
        /// </summary>
        /// <param name="direction">A direction.</param>
        /// <returns>The angle of rotation of the direction, in radians.</returns>
        private double DirectionToRadians(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                default:
                    return 0;
                case Direction.NorthWest:
                    return SEVEN_PI_OVER_FOUR;
                case Direction.West:
                    return THREE_PI_OVER_TWO;
                case Direction.SouthWest:
                    return FIVE_PI_OVER_FOUR;
                case Direction.South:
                    return PI;
                case Direction.SouthEast:
                    return THREE_PI_OVER_FOUR;
                case Direction.East:
                    return PI_OVER_TWO;
                case Direction.NorthEast:
                    return PI_OVER_FOUR;
            }
        }

        /// <summary>
        /// Changes the object's velocity by the given amount. Restricted by the
        /// maximum speed of the object.
        /// </summary>
        /// <param name="amount">The amount to change the velocity by.</param>
        private void ChangeVelocity(Vector2 amount)
        {
            if (amount.X == 0) DecellerateX();
            if (amount.Y == 0) DecellerateY();
            Vector2 newVelocity = Vector2.Add(Velocity, amount);

            // restrain the velocity to the maximums
            Velocity = Vector2.Clamp(newVelocity, new Vector2(-maxVelocity.X, -maxVelocity.Y),
                new Vector2(maxVelocity.X, maxVelocity.Y));
        }

        /// <summary>
        /// Decellerates the object on the x-axis.
        /// </summary>
        private void DecellerateX()
        {
            if (Math.Abs(Velocity.X) <= Math.Abs(acceleration.X))
                Velocity.X = 0;
            else
                Velocity.X = Velocity.X >= 0 ? Velocity.X - deceleration.X : Velocity.X + deceleration.X;
        }

        /// <summary>
        /// Decellerates the object on the y-axis.
        /// </summary>
        private void DecellerateY()
        {
            if (Math.Abs(Velocity.Y) <= Math.Abs(acceleration.Y))
                Velocity.Y = 0;
            else
                Velocity.Y = Velocity.Y >= 0 ? Velocity.Y - deceleration.Y : Velocity.Y + deceleration.Y;
        }

        /// <summary>
        /// Rotates the object to the new angle.
        /// To make the animation less awkward, does not rotate immediately;
        /// instead, eases into new angle gradually.
        /// </summary>
        /// <param name="newAngle">The new angle of the object, in radians.</param>
        private void RotateTo(float newAngle)
        {
            // two possible directions to rotate; choose the one with less
            // distance. Do so by adjusting values based on the newAngle and
            // rotation so that one of them is the top of the circle, then determine
            // distance from there.
            float choice1 = MathHelper.WrapAngle((newAngle - rotation) - (float)TWO_PI);
            float choice2 = MathHelper.WrapAngle((newAngle - rotation));
            if (Math.Abs(choice1) < Math.Abs(choice2))
                rotationChange = (float)(choice1);
            else rotationChange = (float)(choice2);
        }

        /// <summary>
        /// Whether or not the object needs to be rotated.
        /// In other words, if the object is in the middle of transitioning to a new angle
        /// of rotation.
        /// </summary>
        /// <returns>True if the object still needs to be rotated, false otherwise.</returns>
        private bool ShouldRotate()
        {
            return rotationChange != 0;
        }

        /// <summary>
        /// Rotates the object, if the object needs to be rotated.
        /// Otherwise, does nothing.
        /// </summary>
        private void Rotate()
        {
            // finish rotating if the speed of rotation is greater
            // than the amount left to be rotated by. Otherwise,
            // keep rotating.
            if (rotationSpeed > Math.Abs(rotationChange))
            {
                rotation = MathHelper.WrapAngle(rotation + rotationChange);
                rotationChange = 0;
            }
            else
            {
                // reduce the distance to rotate by the rotation speed
                if (rotationChange >= 0)
                {
                    rotation = MathHelper.WrapAngle(rotation + rotationSpeed);
                    rotationChange -= rotationSpeed;
                }
                else
                {
                    rotation = MathHelper.WrapAngle(rotation - rotationSpeed);
                    rotationChange += rotationSpeed;
                }
            }
        }

        /// <summary>
        /// Get the collision between this and another object, as another bounded box.
        /// </summary>
        /// <param name="other">The object being collided with.</param>
        /// <returns>The collision between the two objects, or BBox.Empty if there is no
        /// collision.</returns>
        public BBox GetCollision(GameObject other)
        {
            return BBox.Intersect(other.BBox);
        }

        /// <summary>
        /// Resolves the collision in a region between this object
        /// and another.
        /// </summary>
        /// <param name="other">The other object being collided with.</param>
        /// <param name="region">The region of collision.</param>
        /// <returns>The correction to this object's position.</returns>
        public virtual Vector2 ResolveCollision(GameObject other, BBox region) {
            if (other is Tile) return ResolveCollision((Tile)other, region);
            return Vector2.Zero;
        }

        /// <summary>
        /// Resolves the collision in a region between this object
        /// and a tile.
        /// </summary>
        /// <param name="tile">The tile being collided with.</param>
        /// <param name="region">The region of collision.</param>
        /// <returns>The correction to this object's position.</returns>
        public virtual Vector2 ResolveCollision(Tile tile, BBox region) {
            Vector2 correction = Vector2.Zero;
            if (tile.GetTileType() == TileType.Wall)
            {
                // push the object outside the boundaries of the wall
                if (region.Position.X >= tile.Position.X + tile.Texture.Width / 2)
                    correction = new Vector2(region.Width, correction.Y);
                else correction = new Vector2(-region.Width, correction.Y);

                if (region.Position.Y >= tile.Position.Y + tile.Texture.Height / 2)
                    correction = new Vector2(correction.X, region.Height);
                else correction = new Vector2(correction.X, -region.Height);

                // push the object to the greater of the two coordinates, if they're equal push to both
                //if () correction = new Vector2(correction.X, 0);
                //else if () correction = new Vector2(0, correction.Y);
            }
            return correction;
        }

        /// <summary>
        /// The current position of the object.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                if (BBox != null) BBox.Position = value;
            }
        }

        /// <summary>
        /// The texture of the object.
        /// </summary>
        public Texture2D Texture
        {
            get { return texture; }
            set
            {
                texture = value;
                BBox = new BBox(Position, value.Width, value.Height);
            }
        }
    }
}
