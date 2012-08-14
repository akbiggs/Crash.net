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

        /**
         * Special effects emitters used on launch, during flight,
         * and upon collision with other gameobject.
         **/
        XNAEmitter OnFireFXEmitter;
        XNAEmitter InFlightFXEmitter;
        XNAEmitter OnCollisionFXEmitter;

        public Projectile(
            Vector2 position,
            Vector2 origin,
            Vector2 initialVelocity,
            Vector2 maxSpeed,
            Vector2 acceleration,
            Vector2 deceleration,
            Texture2D texture,
            CrashNet.Engine.Game parent, 
            String XMLFile1, // OnFireFXEmitter xml file
            String XMLFile2, // InFlightFXEmitter xml file
            String XMLFile3, // OnCollisionFXEmitter xml file
            float rotation,  //TODO: Create global default values for rotation, 
            float rotationSpeed, // rotationSpeed and particleLevel (possible from GameObject constants)
            double particleLevel = 1.0,
            double particleScaling = 1.0) 
            : base(position, origin, initialVelocity, maxSpeed, acceleration, deceleration, texture, rotation, rotationSpeed)
        {
            OnFireFXEmitter = new XNAEmitter(parent, position, XMLFile1, particleLevel, particleScaling);
            InFlightFXEmitter = new XNAEmitter(parent, position, XMLFile2, particleLevel, particleScaling);
            OnCollisionFXEmitter = new XNAEmitter(parent, position, XMLFile3, particleLevel, particleScaling);
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
            InFlightFXEmitter.Stop();
            OnCollisionFXEmitter.Start();
        }

        /**
         * Update override.
         **/
        internal void Update(GameTime gameTime, Room room)
        {
            /**
             * Move the projectile:
             **/
            Move(room);

            /**
             * Update the special effects:
             **/
            OnFireFXEmitter.SetLocation(Position);
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
            OnCollision();
            base.Collide(other, region);
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
            base.Draw(spriteBatch);
        }
        
    }
}
