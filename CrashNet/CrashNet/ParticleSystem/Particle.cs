using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrashNet.GRNG;


namespace CrashNet.ParticleSystem
{
    public class Particle
    {

        #region Members
        /// <summary>
        /// The parent emitter.
        /// i.e. the thing that "done launchered it".
        /// </summary>
        public ParticleEmitter parentEmitter { get; set; }
        #endregion

        #region Properties
        /// <summary>
        /// Position on screen.
        /// </summary>
        public Vector3D position;

        /// <summary>
        /// Velocity of the particle.
        /// </summary>
        private Vector3D velocity;

        /// <summary>
        /// Acceleration of the particle.
        /// </summary>
        private Vector3D acceleration;

        /// <summary>
        /// How much the particle is rotated by.
        /// </summary>
        public double angle { get; set; }

        /// <summary>
        /// How fast the particle rotates.
        /// </summary>
        private double angularVelocity;

        /// <summary>
        /// RGB color vector.
        /// </summary>
        public Vector3D color { get; set; }

        /// <summary>
        /// The transparency of the particle.
        /// Value from 0.0 to 1.0.
        /// </summary>
        public double transparency { get; set; }

        /// <summary>
        /// How much the transparency changes by during each update.
        /// </summary>
        public double transparencyDelta { get; set; }

        /// <summary>
        /// The scale at which to draw the particle.
        /// </summary>
        public double size { get; set; }

        /// <summary>
        /// Amount by which the size changes during each update.
        /// Multiplicative, not additive.
        /// </summary>
        private double sizeDelta;

        /// <summary>
        /// How much longer this particle is left alive for.
        /// </summary>
        public double lifeLeft { get; set; }

        /// <summary>
        /// Index of sprite/texture used by this particle.
        /// Used in combination with emitters with multiple textures.
        /// </summary>
        public int TextureIndex { get; set; }
        #endregion

        /// <summary>
        /// Makes a new particle.
        /// </summary>
        /// <param name="parent">The emitter of the particle.</param>
        /// <param name="position">The starting position of the particle.</param>
        /// <param name="velocity">The initial velocity of the particle.</param>
        /// <param name="acceleration">The acceleration of the particle</param>
        /// <param name="angle">The angle of the particle.</param>
        /// <param name="angVel">The speed at which the particle's angle changes.</param>
        /// <param name="color">The color of the particle.</param>
        /// <param name="transparency">The transparency of the particle.</param>
        /// <param name="transDelta">The change in transperency of the particle.</param>
        /// <param name="size">The size of the particle.</param>
        /// <param name="sizeDelta">The change in size of the particle.</param>
        /// <param name="lifespan">How long the particle should remain alive for, in milliseconds.</param>
        /// <param name="textureIndex">The index of the texture this particle should have.</param>
        public Particle(
            ParticleEmitter parent,
            Vector3D position,
            Vector3D velocity,
            Vector3D acceleration,
            double angle,
            double angVel,
            Vector3D color,
            double transparency,
            double transDelta,
            double size,
            double sizeDelta,
            double lifespan,
            int txtIndex
            )
        {
            this.parentEmitter = parent;
            this.position = position;
            this.velocity = velocity;
            this.acceleration = acceleration;
            this.angle = angle;
            this.angularVelocity = angVel;
            this.color = color;
            this.transparency = transparency;
            this.transparencyDelta = transDelta;
            this.size = size;
            this.sizeDelta = sizeDelta;
            this.lifeLeft = lifespan;
            this.TextureIndex = txtIndex;
        }

        /// <summary>
        /// Updates the particle. Called every tick.
        /// </summary>
        /// <param name="MsSinceLastUpdate">Milliseconds since the last update was called.</param>
        public void Update(double MsSinceLastUpdate)
        {
            lifeLeft-=MsSinceLastUpdate;
            double SecSinceLastUpdate = MsSinceLastUpdate / 1000.0;
            //MsSinceLastUpdate = 1;

            position.X += velocity.X * SecSinceLastUpdate;
            position.Y += velocity.Y * SecSinceLastUpdate;
            position.Z += velocity.Z * SecSinceLastUpdate;

            velocity.X += acceleration.X * SecSinceLastUpdate;
            velocity.Y += acceleration.Y * SecSinceLastUpdate;
            velocity.Z += acceleration.Z * SecSinceLastUpdate;

            angle += angularVelocity * SecSinceLastUpdate / 360.0;
            size *= (1.0 + sizeDelta * SecSinceLastUpdate);
            transparency += transparencyDelta * SecSinceLastUpdate;

            // kill the particle if it's no longer visible
            if (transparency < 0.0) lifeLeft = 0;
        }    
    }
}
