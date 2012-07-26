using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CrashNet.Engine;

namespace CrashNet.GameObjects
{
    class GameObject
    {
        const float DEFAULT_ROTATION_SPEED = (float)(Math.PI / 10);
        
        Vector2 position;

        /// <summary>
        /// The velocity of the object.
        /// </summary>
        Vector2 velocity;
        Vector2 maxVelocity;
        Vector2 acceleration;
         
        /// <summary>
        /// How much the object is rotated, in radians.
        /// </summary>
        float rotation;
        float rotationChange = 0;
        float rotationSpeed;

        Texture2D texture; 
        BBox BBox = null;

        /// <summary>
        /// Whether or not the object has moved in the last 
        /// update.
        /// </summary>
        bool hasMoved = false;

        /// <summary>
        /// Make a new game object.
        /// </summary>
        /// <param name="position">The starting position of the object.</param>
        /// <param name="initialVelocity">The initial velocity of the object.</param>
        /// <param name="maxSpeed">The maximum velocity the object can obtain.</param>
        /// <param name="acceleration">The acceleration of the object.</param>
        /// <param name="texture">The texture of the object.</param>
        /// <param name="rotation">How much the object should be rotated by.</param>
        /// <param name="rotationSpeed">The speed at which the object rotates.</param>
        public GameObject(Vector2 position, Vector2 initialVelocity, Vector2 maxSpeed, Vector2 acceleration, 
            Texture2D texture, float rotation=(float)(2*Math.PI), float rotationSpeed=DEFAULT_ROTATION_SPEED)
        {
            this.Position = position;

            this.velocity = initialVelocity;
            this.maxVelocity = maxSpeed;
            this.acceleration = acceleration;

            this.Texture = texture;

            this.rotation = rotation;
            this.rotationSpeed = rotationSpeed;
        }

        internal virtual void Update()
        {
            if (ShouldRotate()) Rotate();
            if (!hasMoved) Decellerate();
            Position = Vector2.Add(Position, velocity);

            hasMoved = false;
        }

        internal virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height), 
                null, Color.White, rotation, Vector2.Zero, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Moves the object.
        /// </summary>
        /// <param name="direction">The direction in which to move the object.</param>
        internal virtual void Move(Direction direction)
        {
            hasMoved = true;
            switch (direction)
            {
                // TODO: throw rads to degrees stuff into a helper method
                // Also, fix bug where moving player in two directions makes them
                // face only one.
                case Direction.North:
                    RotateTo((float)(2 * Math.PI));
                    ChangeVelocity(new Vector2(0, -acceleration.Y));
                    break;
                case Direction.South:
                    RotateTo((float)Math.PI);
                    ChangeVelocity(new Vector2(0, acceleration.Y));
                    break;
                case Direction.West:
                    RotateTo((float)(1.5 * Math.PI));
                    ChangeVelocity(new Vector2(-acceleration.X, 0));
                    break;
                case Direction.East:
                    RotateTo((float)(Math.PI / 2));
                    ChangeVelocity(new Vector2(acceleration.X, 0));
                    break;
            }
        }

        /// <summary>
        /// Changes the object's velocity by the given amount. Restricted by the
        /// maximum speed of the object.
        /// </summary>
        /// <param name="amount">The amount to change the velocity by.</param>
        private void ChangeVelocity(Vector2 amount)
        {
            Vector2 newVelocity = Vector2.Add(velocity, amount);

            // if we've passed the max velocity boundaries, reset velocity to them.
            if (Math.Abs(newVelocity.X) > Math.Abs(maxVelocity.X))
                velocity.X = newVelocity.X >= 0 ? maxVelocity.X : -maxVelocity.X;
            else velocity.X = newVelocity.X;

            if (Math.Abs(newVelocity.Y) > Math.Abs(maxVelocity.Y))
                velocity.Y = newVelocity.Y >= 0 ? maxVelocity.Y : -maxVelocity.Y;
            else velocity.Y = newVelocity.Y;
        }

        private void Decellerate()
        {
            //TODO: decellerate the object by its acceleration
            if (Math.Abs(velocity.X) <= Math.Abs(acceleration.X))
                velocity.X = 0;
            else
                velocity.X = velocity.X >= 0 ? velocity.X - acceleration.X : velocity.X + acceleration.X;

            if (Math.Abs(velocity.Y) <= Math.Abs(acceleration.Y))
                velocity.Y = 0;
            else
                velocity.Y = velocity.Y >= 0 ? velocity.Y - acceleration.Y : velocity.Y + acceleration.Y;
        }

        /// <summary>
        /// Rotates the object to the new angle.
        /// To make the animation less awkward, does not rotate immediately;
        /// instead, eases into new angle gradually.
        /// </summary>
        /// <param name="newAngle">The new angle of the object, in radians.</param>
        private void RotateTo(float newAngle)
        {
            rotationChange = newAngle - rotation;
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
                rotation = rotation + rotationChange;
                rotationChange = 0;
            }
            else
            {
                if (rotationChange >= 0)
                {
                    rotation += rotationSpeed;
                    rotationChange -= rotationSpeed;
                }
                else
                {
                    rotation -= rotationSpeed;
                    rotationChange += rotationSpeed;
                }
            }
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
