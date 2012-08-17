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

        public Projectile(
            GameObject owner,
            Vector2 position,
            Vector2 origin,
            Vector2 initialVelocity,
            Vector2 maxSpeed,
            Vector2 acceleration,
            Vector2 deceleration,
            Texture2D texture,
            /**CrashNet.Engine.Game parent,**/ 
            String XMLFile1, // OnFireFXEmitter xml file
            String XMLFile2, // InFlightFXEmitter xml file
            String XMLFile3, // OnCollisionFXEmitter xml file
            float rotation,  //TODO: Create global default values for rotation, 
            float rotationSpeed, // rotationSpeed and particleLevel (possible from GameObject constants)
            double particleLevel = 1.0,
            double particleScaling = 1.0) 
            : base(position, origin, initialVelocity, maxSpeed, acceleration, deceleration, texture, rotation, rotationSpeed)
        {
            this.owner = owner;
            OnFireFXEmitter = new XNAEmitter(/**parent,**/ position, XMLFile1, particleLevel, particleScaling);
            InFlightFXEmitter = new XNAEmitter(/**parent,**/ position, XMLFile2, particleLevel, particleScaling);
            OnCollisionFXEmitter = new XNAEmitter(/**parent,**/ position, XMLFile3, particleLevel, particleScaling);
        }

        /**
         * Used when projectile is first fired to trigger
         * special effects.
         **/
        internal void OnFire()
        {
            OnFireFXEmitter.Start();
            InFlightFXEmitter.Start();
        }

        /**
         * Triggers special effects when projectile
         * collides with another gameObject.
         **/
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

        /**
         * Update override.
         **/
        internal override void Update(GameTime gameTime, Room room)
        {
            if (!IsAlive())
                room.RemoveAfterUpdate(this);
            /**
             * Move the projectile:
             **/
            if (!HasCollided)
                Move(room);

            /**
             * Update the special effects:
             **/
            
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
                base.Collide(other, region);
            }
        }

        /**
         * Draw special effects then call GameObject draw.
         * Note: May need to reverse order of these calls to make
         * projectile not appear on top of effects.
         **/
        internal override void Draw(SpriteBatch spriteBatch)
        {
          
            OnFireFXEmitter.Draw(spriteBatch);
            InFlightFXEmitter.Draw(spriteBatch);
            OnCollisionFXEmitter.Draw(spriteBatch);
            if (HasCollided)
                base.Draw(spriteBatch);
        }
        
    }
}
