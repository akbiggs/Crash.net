﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CrashNet.Engine;

namespace CrashNet.Worlds
{
    class GameObject
    {
        Vector2 position;

        /// <summary>
        /// The velocity of the object.
        /// </summary>
        Vector2 velocity;

        /// <summary>
        /// How much the object is rotated, in radians.
        /// </summary>
        float rotation;

        Texture2D texture; 
        BBox BBox = null;

        /// <summary>
        /// Make a new game object.
        /// </summary>
        /// <param name="position">The starting position of the object.</param>
        /// <param name="velocity">The initial velocity of the object.</param>
        /// <param name="texture">The texture of the object.</param>
        /// <param name="rotation">How much the object should be rotated by.</param>
        public GameObject(Vector2 position, Vector2 velocity, Texture2D texture, float rotation=0)
        {
            this.Position = position;
            this.velocity = velocity;
            this.Texture = texture;
            this.rotation = rotation;
        }

        internal virtual void Update()
        {
            Position = Vector2.Add(Position, velocity);
        }

        internal virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height), 
                null, Color.White, rotation, Vector2.Zero, SpriteEffects.None, 0);
        }

        /// <summary>
        /// The current position of the object.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return position;
            }
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
            get
            {
                return texture;
            }
            set
            {
                texture = value;
                BBox = new BBox(Position, value.Width, value.Height);
            }
        }
    }
}
