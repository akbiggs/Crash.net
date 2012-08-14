using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrashNet.GRNG;


namespace CrashNet.ParticleSystem
{
    public class Particle
    {
        
        /**
         * Members
         **/

        /**
         * The parent emitter.
         * ie. "the thing that done launchered it"
         **/
        public ParticleEmitter parentEmitter { get; set; }

        /**
         * Properties
         **/

        /**
         * Position on screen.
         * */
        public Vector3D position;

        /**
         * Velocity vector.
         **/
        private Vector3D velocity;

        /**
         * Acceleration vector.
         **/
        private Vector3D acceleration;

        /**
         * Rotation angle
         **/
        public double angle { get; set; }

        /**
         * How fast particle rotates.
         **/
        private double angularVelocity;

        /**
         * RGB color vector.
         **/
        public Vector3D color { get; set; }

        /**
         * Alpha value 0 - 1.0
         **/
        public double transparency { get; set; }

        /**
         * Amount of alpha change at every update.
         **/
        public double transparencyDelta { get; set; }

        /**
         * Scale at which to draw particle.
         **/
        public double size { get; set; }

        /**
         * Amount by which size changes every update.
         * (multiplicative, not additive)
         **/
        private double sizeDelta;

        /**
         * Number of updates this particle is active for. 
         **/
        public double TTL { get; set; }

        /**
         * Index of sprite/Texture used by this particle.
         * Used for emitters with multiple textures.
         **/
        public int TextureIndex { get; set; }


       


        public Particle(
            ParticleEmitter parent,
            Vector3D position,
            Vector3D velocity,
            Vector3D acceleration,
            double angle,
            double angVel,
            Vector3D col,
            double trans,
            double transDelta,
            double size,
            double sizeDelta,
            double ttl,
            int txtIndex
            )
        {
            this.parentEmitter = parent;
            this.position = position;
            this.velocity = velocity;
            this.acceleration = acceleration;
            this.angle = angle;
            this.angularVelocity = angVel;
            this.color = col;
            this.transparency = trans;
            this.transparencyDelta = transDelta;
            this.size = size;
            this.sizeDelta = sizeDelta;
            this.TTL = ttl;
            this.TextureIndex = txtIndex;
        }




        /**
         * Updated every tick.
         **/
        public void Update(double MsSinceLastUpdate)
        {
            TTL-=MsSinceLastUpdate;
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

            /**
             * Kill particle if it's no longer visible
             **/
            if (transparency < 0.0) TTL = 0;

        }    
        
    }
}
