using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CrashNet.GameObjects;
using CrashNet.Engine;
using CrashNet.Worlds;
using CrashNet.ParticleSystem;

namespace CrashNet.ParticleSystem.Weapons
{
    class BoomStick : Projectile
    {
        /**
         * Constant weapon projectile parameters
         **/
        internal const float ORIGIN_X = 0.0f;
        internal const float ORIGIN_Y = 0.0f;
        internal const float INITIAL_VELOCITY_X = 10.0f;
        internal const float INITIAL_VELOCITY_Y = 10.0f;
        internal const float ACCELERATION_X = 0.0f;
        internal const float ACCELERATION_Y = 0.0f;
        internal const float MAX_SPEED_X = 20.0f;
        internal const float MAX_SPEED_Y = 20.0f;
        internal const double PARTICLE_SCALING = 0.5f;
        internal const double PARTICLE_LEVELS = 1.0f;

        const String ON_FIRE_XML = "BoomOnFire.xml";
        const String IN_FLIGHT_XML = "BoomInFlight.xml";
        const String ON_COLLISION_XML = "BoomOnCollision.xml";
        const float ROTATION = 0.0f;
        const float ROTATION_SPEED = 0.0f;

        public BoomStick(GameObject owner, Vector2 position, Vector2 initialVelocity, Texture2D Texture) : 
            base(owner, 
            position, 
            new Vector2(ORIGIN_X, ORIGIN_Y), // Origin
            initialVelocity,
            new Vector2(MAX_SPEED_X, MAX_SPEED_Y),
            new Vector2(ACCELERATION_X, ACCELERATION_Y),
            new Vector2(ACCELERATION_X, ACCELERATION_Y),
            Texture,
            DIR + ON_FIRE_XML,
            DIR + IN_FLIGHT_XML,
            DIR + ON_COLLISION_XML,
            ROTATION,
            ROTATION_SPEED,
            1.0,
            PARTICLE_SCALING
            )
        {            
            OnFire();
        }
    }
}
