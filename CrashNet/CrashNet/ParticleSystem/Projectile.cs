using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CrashNet.GameObjects;
using CrashNet.Engine;
using CrashNet.Worlds;

namespace CrashNet.ParticleSystem
{
    /**
     * Projectile class used for bullets, missiles, and other animated gameobjects
     * which require physics, collisions, and particle system effects.
     */
    class Projectile : GameObject
    {
        /**
         * Members
         **/
        internal const String DIR = "Content\\EffectsXMLs\\";

        // Player who fired this thing:
        GameObject owner;
        /**
         * Special effects emitters used on launch, during flight,
         * and upon collision with other gameobject.
         **/
        internal XNAEmitter OnFireFXEmitter;
        internal XNAEmitter InFlightFXEmitter;
        internal XNAEmitter OnCollisionFXEmitter;

        private bool HasCollided = false;

        /// <summary>
        /// Creates a new projectile.
        /// </summary>
        /// <param name="owner">The object that fired the projectile.</param>
        /// <param name="position">The initial position of the projectile.</param>
        /// <param name="origin">The origin point of the projectile.</param>
        /// <param name="initialVelocity">The initial velocity of the projectile.</param>
        /// <param name="maxSpeed">The maximum speed of the projectile.</param>
        /// <param name="acceleration">The acceleration of the projectile.</param>
        /// <param name="deceleration">The deccceleration of the projectile.</param>
        /// <param name="texture">The texture of the projectile.</param>
        /// <param name="OnFireFX_XML">The relative path to the XML file from which the 
        /// effects for firing a projectile are loaded.</param>
        /// <param name="InFlightFX_XML">The relative path to the XML file from which the
        /// effects used for an in-flight particle are loaded.</param>
        /// <param name="OnCollisionFX_XML">The relative path to the XML file from which the
        /// effects used for a projectile that has collided with a surface are loaded.</param>
        /// <param name="rotation">The initial rotation of the projectile.</param>
        /// <param name="rotationVelocity">The speed at which the projectile spins.
        /// Negative for counter-clockwise, positive for clockwise, 0 for no spinning.</param>
        /// <param name="particleLevel"></param>
        /// <param name="particleScaling"></param>
        public Projectile(
            GameObject owner,
            Vector2 position,
            Vector2 origin,
            Vector2 initialVelocity,
            Vector2 maxSpeed,
            Vector2 acceleration,
            Vector2 deceleration,
            Texture2D texture,
            String OnFireFX_XML,
            String InFlightFX_XML,
            String OnCollisionFX_XML,
            float rotation,  //TODO: Create global default values for rotation, 
            float rotationVelocity, // rotationSpeed and particleLevel (possible from GameObject constants)
            double particleLevel = 1.0,
            double particleScaling = 1.0) 
            : base(position, origin, initialVelocity, maxSpeed, acceleration, deceleration, texture, rotation, rotationVelocity)
        {
            this.owner = owner;
            OnFireFXEmitter = new XNAEmitter(/**parent,**/ position, OnFireFX_XML, particleLevel, particleScaling);
            InFlightFXEmitter = new XNAEmitter(/**parent,**/ position, InFlightFX_XML, particleLevel, particleScaling);
            OnCollisionFXEmitter = new XNAEmitter(/**parent,**/ position, OnCollisionFX_XML, particleLevel, particleScaling);
        }

        /// <summary>
        /// Callback for projectile fired event.
        /// Triggers special in-flight effects.
        /// </summary>
        internal void OnFire()
        {
            OnFireFXEmitter.Start();
            InFlightFXEmitter.Start();
        }

        /// <summary>
        /// Callback for collision event.
        /// Triggers special collision effects.
        /// </summary>
        internal void OnCollision()
        {
            HasCollided = true;
            InFlightFXEmitter.Stop();
            OnCollisionFXEmitter.Start();
        }

        internal override bool IsAlive()
        {
            return (OnFireFXEmitter.IsAlive()
                || InFlightFXEmitter.IsAlive()
                || OnCollisionFXEmitter.IsAlive()
                || (!HasCollided)
                );
        }

        internal override void Update(GameTime gameTime, Room room)
        {
            // remove if expired
            if (!IsAlive())
                room.RemoveAfterUpdate(this);
            
            // move the projectile
            if (!HasCollided)
                Move(room);
         
            // update the special effects            
            OnFireFXEmitter.Update(gameTime.ElapsedGameTime.Milliseconds);
            InFlightFXEmitter.SetLocation(Position);
            InFlightFXEmitter.Update(gameTime.ElapsedGameTime.Milliseconds);
            OnCollisionFXEmitter.SetLocation(Position);
            OnCollisionFXEmitter.Update(gameTime.ElapsedGameTime.Milliseconds);
            base.Update(gameTime, room);
        }

        /**
         * Handle collision events.
         **/
        public override void Collide(GameObject other, BBox region)
        {
            if (!HasCollided && (!other.Equals(owner)))
            {
                OnCollision();
                /**base.Collide(other, region);**/
            }
        }

        /**
         * Draw special effects then call GameObject draw.
         * Note: May need to reverse order of these calls to make
         * projectile not appear on top of effects.
         **/
        internal override void Draw(SpriteBatch spriteBatch)
        {

            if (!HasCollided)
                base.Draw(spriteBatch);
            OnFireFXEmitter.Draw(spriteBatch);
            InFlightFXEmitter.Draw(spriteBatch);
            OnCollisionFXEmitter.Draw(spriteBatch);
            
        }
        
    }
}
