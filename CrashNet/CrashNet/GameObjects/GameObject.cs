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
        const double PI = Math.PI;
        const double PI_OVER_TWO = PI / 2;
        const double THREE_PI_OVER_TWO = 1.5 * PI;
        const double TWO_PI = 2 * PI;
        //diagonals
        const double PI_OVER_FOUR = 0.25 * PI;
        const double THREE_PI_OVER_FOUR = 0.75 * PI;
        const double FIVE_PI_OVER_FOUR = 1.25 * PI;
        const double SEVEN_PI_OVER_FOUR = 1.75 * PI;
        
        Vector2 position;

        /// <summary>
        /// The velocity of the object.
        /// </summary>
        Vector2 velocity;
        Vector2 maxVelocity;
        Vector2 acceleration;
         
        /// <summary>
        /// How much the object is rotated, in radians.
        /// A rotation of 0 is treated as a rotation of 2Pi.
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
        bool hasMovedX = false;
        bool hasMovedY = false;

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
            Texture2D texture, float rotation=0, float rotationSpeed=DEFAULT_ROTATION_SPEED)
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
            if (!hasMovedX) DecellerateX();
            if (!hasMovedY) DecellerateY();
            Position = Vector2.Add(Position, velocity);

            hasMovedX = false;
            hasMovedY = false;
        }

        internal virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height), 
                null, Color.White, rotation, new Vector2(Texture.Width/2, Texture.Height/2), SpriteEffects.None, 0);
        }

        /// <summary>
        /// Moves the object.
        /// </summary>
        /// <param name="direction">The direction in which to move the object.</param>
        internal virtual void Move(Direction direction)
        {
            float angle = (float)DirectionToRadians(direction);
            RotateTo(angle);

            // There's this weird arithmetic bug where these produce very small values during movements along
            // the opposite axes, so round them.
            float xComponent = (float)(acceleration.X * Math.Round(Math.Sin(angle), 6));
            // up is negative, down is positive, so multiply by -1. 
            float yComponent = (float)(-1 * acceleration.Y * Math.Round(Math.Cos(angle), 6));
            ChangeVelocity(new Vector2(xComponent, yComponent));
        }

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
            if (amount.X != 0) hasMovedX = true;
            if (amount.Y != 0) 
                hasMovedY = true;
            Vector2 newVelocity = Vector2.Add(velocity, amount);

            // if we've passed the max velocity boundaries, reset velocity to them.
            if (Math.Abs(newVelocity.X) > Math.Abs(maxVelocity.X))
                velocity.X = newVelocity.X >= 0 ? maxVelocity.X : -maxVelocity.X;
            else velocity.X = newVelocity.X;

            if (Math.Abs(newVelocity.Y) > Math.Abs(maxVelocity.Y))
                velocity.Y = newVelocity.Y >= 0 ? maxVelocity.Y : -maxVelocity.Y;
            else velocity.Y = newVelocity.Y;
        }

        /// <summary>
        /// Decellerates the object on the x-axis.
        /// </summary>
        private void DecellerateX()
        {
            if (Math.Abs(velocity.X) <= Math.Abs(acceleration.X))
                velocity.X = 0;
            else
                velocity.X = velocity.X >= 0 ? velocity.X - acceleration.X : velocity.X + acceleration.X;
        }

        /// <summary>
        /// Decellerates the object on the y-axis.
        /// </summary>
        private void DecellerateY()
        {
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
            // two possible directions to rotate; choose the one with less
            // distance. Do so by adjusting values based on the newAngle and
            // rotation so that one of them is the top of the circle, then determine
            // distance from there.
            float adjust = Math.Min(newAngle, rotation);
            float choice1 = (newAngle + adjust) - (rotation + adjust);
            float choice2 = (newAngle + adjust) + (rotation + adjust);
            if (choice1 % TWO_PI < choice2 % TWO_PI)
                rotationChange = (float)(choice1 % TWO_PI);
            else rotationChange = (float)(choice2 % TWO_PI);
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
                rotation = (rotation + rotationChange) % (float)TWO_PI;
                rotationChange = 0;
            }
            else
            {
                if (rotationChange >= 0)
                {
                    rotation = (rotation + rotationSpeed) % (float)TWO_PI;
                    rotationChange -= rotationSpeed;
                }
                else
                {
                    rotation = (rotation - rotationSpeed) % (float)TWO_PI;
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
