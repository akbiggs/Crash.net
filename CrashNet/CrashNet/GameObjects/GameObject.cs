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

        /// <summary>
        /// Updates the object in the context of the given room.
        /// </summary>
        /// <param name="gameTime">The game's timer.</param>
        /// <param name="room">The room in which the object is located.</param>
        internal virtual void Update(GameTime gameTime, Room room)
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
        /// Moves the object in the given direction in the context of
        /// being in the given room, meaning the object will collide against
        /// any walls in the room.
        /// </summary>
        /// <param name="direction">The direction to move the object.</param>
        /// <param name="gameTime">The game's timer.</param>
        /// <param name="room">The room in which the object is moving around.</param>
        internal virtual void Move(Direction direction, GameTime gameTime, Room room)
        {
            float xComponent, yComponent;
            if (direction == Direction.None)
                xComponent = yComponent = 0;
            else
            {
                float angle = (float)DirectionOperations.ToRadians(direction);
                RotateTo(angle);

                // There's this arithmetic bug where sin and cos produce very small values during movements along
                // the opposite axes, so round them.
                xComponent = (float)(acceleration.X * gameTime.ElapsedGameTime.Milliseconds * Math.Round(Math.Sin(angle), 6));
                // up is negative, down is positive, so multiply by -1. 
                yComponent = (float)(-1 * acceleration.Y * gameTime.ElapsedGameTime.Milliseconds * Math.Round(Math.Cos(angle), 6));

            }
            ChangeVelocity(new Vector2(xComponent, yComponent));
            Move(room);
                
        }

        /**
         * Returns x,y component vector from the specified direction and speed.
         * NOTE: Can use this Move() method above.
         **/
        internal Vector2 GetXYComponents(Direction dir, float magnitudex, float magnitudey)
        {
            float x, y, theta;
            theta = (float)DirectionOperations.ToRadians(dir);

            x = (float)(magnitudex * Math.Round(Math.Sin(theta), 6));
            y = (float)(-1 * magnitudey * Math.Round(Math.Cos(theta), 6));
            return new Vector2(x, y);
        }

        /**
         * Moves GameObject based on its current velocity.
         **/ 
        internal virtual void Move(Room room)
        {
            if (Velocity.X != 0)
                ChangeXPosition(Velocity.X, room);
            if (Velocity.Y != 0)
                ChangeYPosition(Velocity.Y, room);
        }

        /// <summary>
        /// Changes the x-position of the object. Collides with any walls
        /// in the given room.
        /// </summary>
        /// <param name="amount">The amount to change the position by. Negative means left,
        /// positive means right.</param>
        /// <param name="room">The room in which the object is located. Movement will collide against
        /// walls.</param>
        private void ChangeXPosition(float amount, Room room)
        {
            // 1. change the x-pos.
            // 2. round to prevent jittering in some situations
            // 3. go through each intersection, and fix it.
            // Thanks to David Gouveia http://gamedev.stackexchange.com/users/11686/david-gouveia
            Position = new Vector2((float)Math.Round(Position.X + amount), Position.Y);
            room.KeepInBounds(this);
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
        /// Changes the y-position of the object. Collides with any walls
        /// in the given room.
        /// </summary>
        /// <param name="amount">The amount to change the position by. Negative means up,
        /// positive means down.</param>
        /// <param name="room">The room in which the object is located. Movement will collide against
        /// walls.</param>
        private void ChangeYPosition(float amount, Room room)
        {
            // 1. change the y-pos.
            // 2. round to prevent jittering in some situations
            // 3. go through each intersection, and fix it.
            // Thanks to David Gouveia http://gamedev.stackexchange.com/users/11686/david-gouveia
            Position = new Vector2(Position.X, (float)Math.Round(Position.Y + amount));
            room.KeepInBounds(this);
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
            float choice1 = MathHelper.WrapAngle((newAngle - rotation) - (float)MathHelper.TwoPi);
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
        /// Whether or not this object should collide with another object.
        /// </summary>
        /// <param name="other">The other object to check for a collision 
        /// against.</param>
        /// <param name="region">The region of collision. An empty bounding box
        /// if the objects do not collide.</param>
        /// <returns>True if the objects should collide, false otherwise.</returns>
        public bool ShouldCollide(GameObject other, out BBox region)
        {
            region = BBox.Intersect(other.BBox);
            return !region.IsEmpty();
        }

        /// <summary>
        /// Used during room update method to determine
        /// if this GameObject should be removed from
        /// room's GameObject list.
        /// Assumes objects are permanent.
        /// Descendant child-classes should override
        /// if they are not permanent.
        /// </summary>
        /// <returns></returns>
        internal virtual bool IsAlive()
        {
            return true;
        }

        /// <summary>
        /// Resolves the collision in a region between this object
        /// and another.
        /// </summary>
        /// <param name="other">The other object being collided with.</param>
        /// <param name="region">The region of collision.</param>
        public virtual void Collide(GameObject other, BBox region) {

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
