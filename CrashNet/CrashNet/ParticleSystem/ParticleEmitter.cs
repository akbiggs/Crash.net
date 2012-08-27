using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CrashNet.GRNG;


namespace CrashNet.ParticleSystem
{
    public class ParticleEmitter
    {
        #region Members

        /// <summary>
        /// Global FX levels for the particle emitter.
        /// i.e. high for extreme amounts of particles, low for low amounts of particles.
        /// </summary>
        private double GlobalParticleLevels;

        /// <summary>
        /// Size scaling of the particles.
        /// </summary>
        private double GlobalScaling;

        /// <summary>
        /// Random number generator used for generating particles.
        /// </summary>
        private GRandom grandom;

        /// <summary>
        /// Contains references to all active particles until removed.
        /// </summary>
        public List<Particle> particles;
        #endregion

        #region Particle Properties
        /**
         * Properties of this Particle Emitter:
         * Alpha and Beta params used to randomly determine
         * the corresponding property of a particle when it
         * is instantiated by the emitter.
         * 
         * For normally-distributed params, transparency=mean and
         * beta=variance.
         * For uniformly-distributed params, transparency=min and
         * beta=max.
         * For exponentially-distributed params, transparency=mean.
         * beta is unused.
         * For fixed params, only alpha is used.
         **/

        // position is relative to the emitter's center
        protected Parameter3D position = new Parameter3D();
        protected ParameterDouble speed = new ParameterDouble();
        protected ParameterDouble direction = new ParameterDouble();
        protected Parameter3D velocity = new Parameter3D();
        protected Parameter3D acceleration = new Parameter3D();
        protected ParameterDouble angle = new ParameterDouble();
        protected ParameterDouble angularVelocity = new ParameterDouble();
        protected Parameter3D color = new Parameter3D();
        protected ParameterDouble transparency = new ParameterDouble();
        protected ParameterDouble transparencyDelta = new ParameterDouble();
        protected ParameterDouble size = new ParameterDouble();
        protected ParameterDouble growth = new ParameterDouble();
        protected ParameterDouble ttl = new ParameterDouble();
        #endregion

        #region Emitter Properties

        // Particle emitter behavior properties.
        // Belong to the emitter itself, not to the particles it emits.
        
        /// <summary>
        /// The center point of the emitter.
        /// </summary>
        public Vector3D Location { get; set; }

        // TODO: actually use this.
        /// <summary>
        /// The area of the emitter from which particles can be emitted.
        /// </summary>
        public Vector3D EmitDimensions { get; set; }
      
        /// <summary>
        /// The max number of particles allowed to be queued.
        /// </summary>
        public int MaxNumParticles { get; set; }

        /// <summary>
        /// How many particles can be generated each update.
        /// </summary>
        public int EmitRate { get; set; }

        /// <summary>
        /// The mean time between individual particle emissions, in seconds.
        /// </summary>
        public double MeanEmitDelay { get; set; }

        /// <summary>
        /// How long the emitter emits for, in milliseconds.
        /// </summary>
        public double EmitLifetime { get; set; }

        /// <summary>
        /// How long the particle has been emitting for, in seconds. 
        /// </summary>
        public double CurrentLifeTime { get; set; } // Running total of time spent emitting, in Secs.

        /// <summary>
        /// How long until the next particle emission occurs.
        /// </summary>
        public double TimeToNextEmit { get; set; }

        /// <summary>
        /// Whether the emitter should currently be emitting particles.
        /// </summary>
        public bool EmitActivity { get; set; }

        /// <summary>
        /// Whether or not the particles are permanent.
        /// If true, particles will not disappear, otherwise they will.
        /// </summary>
        public bool PermanentParticles { get; set; } // Are particles permanent or do they disappear?

        /// <summary>
        /// The number of textures the emitter can generate. Used for multi-texture emitters.
        /// </summary>
        public int NumTextures { get; set; } // Used for multi-texture emitters.
        #endregion

        #region Constructors
        /// <summary>
        /// Make a new particle emitter.
        /// </summary>
        /// <param name="particleLevel">The particle level of the emitter; how extreme the emitter should be
        /// in terms of particle emission.</param>
        /// <param name="particleScaling">The factor by which to scale the particles.</param>
        public ParticleEmitter(double particleLevel = 1.0, double particleScaling = 1.0)
        {
            GlobalParticleLevels = particleLevel;
            GlobalScaling = particleScaling;
            CurrentLifeTime = 0;
            grandom = new GRandom();
            particles = new List<Particle>();
        }

        /// <summary>
        /// Make a new particle emitter taking a file for parameters.
        /// </summary>
        /// <param name="location">The initial location of the particle emitter.</param>
        /// <param name="paramFileName">The relative path to the file to get the parameters from.</param>
        /// <param name="pLevel">The particle level of the emitter.</param>
        /// <param name="pScaling">The factor by which to scale the particles.</param>
        public ParticleEmitter(Vector3D location, String paramFileName, double pLevel = 1.0, double pScaling = 1.0) 
            : this(pLevel, pScaling)
        {
            // TODO: get parameters from parameter file.
            Location = location;
        }

        /// <summary>
        /// Create a new particle emitter with explicit parameters.
        /// Are you sure you really want to do this?
        /// </summary>
        /// <param name="positionMean">The mean position of particles.</param>
        /// <param name="positionVar">The variance of the position of particles.</param>
        /// <param name="positionDist">The distribution of the position of particles.</param>
        /// <param name="velocityMean">The mean velocity of particles.</param>
        /// <param name="velocityVar">The variance of the velocity of particles.</param>
        /// <param name="velocityDist">The distribution of the velocity of particles.</param>
        /// <param name="accelerationMean">The mean acceleration of particles.</param>
        /// <param name="accelerationVar">The variance of the acceleration of particles.</param>
        /// <param name="accelerationDist">The distribution of the acceleration of particles.</param>
        /// <param name="angleMean">The mean angle of particles.</param>
        /// <param name="angleVar">The variance of the angle of particles.</param>
        /// <param name="angleDist">The distribution of the angle of particles.</param>
        /// <param name="angleVelocityMean">The mean velocity of the change of angle of a particle.
        /// Positive for clockwise, negative for counterclockwise.</param>
        /// <param name="angleVelocityVar">The variance of the velocity of the change of angle 
        /// of particles. Positive for clockwise, negative for counterclockwise.</param>
        /// <param name="angleVelDist">The distribution of the velocity of the change of angle
        /// of particles. Positive for clockwise, negative for counterclockwise.</param>
        /// <param name="colorMean">The mean color of a particle.</param>
        /// <param name="colorVar">The variance of the color of particles.</param>
        /// <param name="colorDist">The distribution of the color of particles.</param>
        /// <param name="alphaMean">The mean transparency of particles.</param>
        /// <param name="alphaVar">The variance of transparency of particles.</param>
        /// <param name="alphaDist">The distribution of transparency of particles.</param>
        /// <param name="alphaDeltaMean">The mean change in transparency of particles.</param>
        /// <param name="alphaDeltaVar">The variation of change in transparency of particles.</param>
        /// <param name="alphaDeltaDist">The distribution of change in transparency of particles.</param>
        /// <param name="sizeMean">The mean size of particles.</param>
        /// <param name="sizeVar">The variance of size of particles.</param>
        /// <param name="sizeDist">The distribution of size of particles.</param>
        /// <param name="sizeDeltaMean">The mean change in size of particles.</param>
        /// <param name="sizeDeltaVar">The variance of change in size of particles.</param>
        /// <param name="sizeDeltaDist">The distribution of change in size of particles.</param>
        /// <param name="ttlMean">The mean lifespan of particles.</param>
        /// <param name="ttlVar">The variance of lifespan of particles.</param>
        /// <param name="ttlDist">The distribution of lifespan of particles.</param>
        /// <param name="location">The location of the emitter.</param>
        /// <param name="dimension">The area from which the emitter should send particles.</param>
        /// <param name="maxNumParticles">The max number of particles that can be queued by the emitter.</param>
        /// <param name="emitRate">The emission rate of the emitter, in milliseconds.</param>
        /// <param name="emitDelay">The delay between emissions, in milliseconds.</param>
        /// <param name="emitLife">How long the emitter should emit for, in milliseconds.</param>
        /// <param name="permanentParticles">Whether or not the particles this emitter emits
        /// are permanent(do not disappear).</param>
        /// <param name="particleLevel">The particle level of the emitter. Determines how extreme 
        /// the particley-ness of this emitter should be.</param>
        public ParticleEmitter(
            /** Particle parameters **/
            Vector3D positionMean, Vector3D positionVar, Distribution positionDist,
            Vector3D velocityMean, Vector3D velocityVar, Distribution velocityDist,
            Vector3D accelerationMean, Vector3D accelerationVar, Distribution accelerationDist,
            double angleMean, double angleVar, Distribution angleDist,
            double angleVelocityMean, double angleVelocityVar, Distribution angleVelDist,
            Vector3D colorMean, Vector3D colorVar, Distribution colorDist,
            double alphaMean, double alphaVar, Distribution alphaDist,
            double alphaDeltaMean, double alphaDeltaVar, Distribution alphaDeltaDist,
            double sizeMean, double sizeVar, Distribution sizeDist,
            double sizeDeltaMean, double sizeDeltaVar, Distribution sizeDeltaDist,
            double ttlMean, double ttlVar, Distribution ttlDist,
            /** Emitter Parameters **/
            Vector3D location,
            Vector3D dimension,
            int maxNumParticles,
            int emitRate,
            int emitDelay,
            int emitLife,
            bool permanentParticles,
            double particleLevel = 1.0
            ) : this(particleLevel) 
        {
            this.position.alpha = positionMean;
            this.position.beta = positionVar;
            this.position.distribution = positionDist;

            this.velocity.alpha = velocityMean;
            this.velocity.beta = velocityVar;
            this.velocity.distribution = velocityDist;

            this.acceleration.alpha = accelerationMean;
            this.acceleration.beta = accelerationVar;
            this.acceleration.distribution = accelerationDist;

            this.angle.alpha = angleMean;
            this.angle.beta = angleVar;
            this.angle.distribution = angleDist;

            this.angularVelocity.alpha = angleVelocityMean;
            this.angularVelocity.beta = angleVelocityVar;
            this.angularVelocity.distribution = angleVelDist;

            this.color.alpha = colorMean;
            this.color.beta = colorVar;
            this.color.distribution = colorDist;

            this.transparency.alpha = alphaMean;
            this.transparency.beta = alphaVar;
            this.transparency.distribution = alphaDist;

            this.transparencyDelta.alpha = alphaDeltaMean;
            this.transparencyDelta.beta = alphaDeltaVar;
            this.transparencyDelta.distribution = alphaDeltaDist;

            this.size.alpha = sizeMean;
            this.size.beta = sizeVar;
            this.size.distribution = sizeDist;

            this.growth.alpha = sizeDeltaMean;
            this.growth.beta = sizeDeltaVar;
            this.growth.distribution = sizeDeltaDist;

            this.ttl.alpha = ttlMean;
            this.ttl.beta = ttlVar;
            this.ttl.distribution = ttlDist;

            this.Location = location;
            this.EmitDimensions = dimension;
            this.MaxNumParticles = maxNumParticles;
            this.EmitRate = emitRate;
            this.MeanEmitDelay = emitDelay;
            this.EmitLifetime = emitLife;
            this.PermanentParticles = permanentParticles;

            NumTextures = 0;   
        }
        #endregion

        /// <summary>
        /// Return a new particle with randomized properties based
        /// on the parameters of this emitter.
        /// </summary>
        /// <returns>A new particle from this emitter.</returns>
        private Particle GenerateParticle()
        {
            Vector3D generateLocation;

            if (position.distribution == Distribution.Normal)
            {
                generateLocation = grandom.GetNormalVector3D(
                    new Vector3D(Location.X + position.alpha.X, 
                        Location.Y + position.alpha.Y,
                        Location.Z + position.alpha.Z),
                         
                    position.beta);
            }
            else
            {
                generateLocation = grandom.GetUniformVector3D(
                    new Vector3D(Location.X - position.alpha.X,
                        Location.Y - position.alpha.Y,
                        Location.Z - position.alpha.Z),
                    new Vector3D(Location.X + position.beta.X,
                        Location.Y + position.beta.Y,
                        Location.Z + position.beta.Z));      
            }


            return new Particle(this,
                generateLocation,
                grandom.GetRandomVector3D(velocity.distribution, velocity.alpha, velocity.beta),
                grandom.GetRandomVector3D(acceleration.distribution, acceleration.alpha, acceleration.beta),
                grandom.GetRandomDouble(angle.distribution, angle.alpha, angle.beta), 
                grandom.GetRandomDouble(angularVelocity.distribution, angularVelocity.alpha, angularVelocity.beta),
                grandom.GetRandomVector3D(color.distribution, color.alpha, color.beta),
                grandom.GetRandomDouble(transparency.distribution, transparency.alpha, transparency.beta),
                grandom.GetRandomDouble(transparencyDelta.distribution, transparencyDelta.alpha, transparencyDelta.beta),
                grandom.GetRandomDouble(size.distribution, size.alpha, size.beta),
                grandom.GetRandomDouble(growth.distribution,growth.alpha, growth.beta),
                (int)grandom.GetRandomDouble(ttl.distribution, ttl.alpha, ttl.beta),
                grandom.GetUniformInt(0, NumTextures));

        }

        /// <summary>
        /// Start the emitter.
        /// </summary>
        public void Start()
        {
            EmitActivity = true;
            CurrentLifeTime = 0;
            TimeToNextEmit = MeanEmitDelay;
        }

        /// <summary>
        /// Immediately cease to emit particles.
        /// </summary>
        public void Stop()
        {
            EmitActivity = false;
        }

        /// <summary>
        /// Whether or not the emitter should still be updated/not be disposed.
        /// </summary>
        /// <returns>Returns true if emitter is either currently emitting, has not yet started to emit, or
        /// no longer emitting but still has particles in queue to be updated, false otherwise.</returns>
        public bool IsAlive()
        {
            return ((EmitActivity) || (CurrentLifeTime == 0) || (particles.Count != 0));
        }

        /// <summary>
        /// Update the emitter.
        /// Stop the emitter if its lifetime has expired.
        /// </summary>
        /// <param name="MsSinceLastUpdate">The number of milliseconds since the last update.</param>
        public void Update(double MsSinceLastUpdate)
        {
            if (EmitActivity)
            {
                CurrentLifeTime += MsSinceLastUpdate; // Update lifetime timer.
                TimeToNextEmit -= MsSinceLastUpdate; // Update emitrate timer.

                if (CurrentLifeTime > EmitLifetime)
                {
                    Stop();
                }
                else
                {
                    // if the emitter is still emitting, and it's been long enough
                    // since the last emit, make some more particles.
                    if ((TimeToNextEmit <= 0) && (EmitActivity))
                    {
                        TimeToNextEmit = (int)grandom.GetExpDouble((double)MeanEmitDelay);

                        // If we still have room on the particle queue, make as many particles
                        // as we're allowed in this update.
                        for (int i = 0; ((i < EmitRate) && (particles.Count < MaxNumParticles)); i++)
                        {
                            particles.Add(GenerateParticle());
                        }

                    }
                }
            }
            
            // update existing particles
            for (int index = 0; index < particles.Count; index++)
            {
                // If particle is still alive or emitter uses perma particles.
                if ((particles[index].lifeLeft > 0) || PermanentParticles)
                {
                    particles[index].Update(MsSinceLastUpdate);
                }
                else
                {
                    // remove dead particle from queue
                    particles.RemoveAt(index);
                    index--;
                }
            }
        }

        /// <summary>
        /// Load emitter properties from an XML file.
        /// </summary>
        /// <param name="emitter">The XML document to load the properties of the emitter from.</param>
        public void LoadEmitterFromXML(XmlDocument emitter)
        {
            XmlNode emitterPars =
                emitter.SelectSingleNode("/ParticleSystem/EmitterParameters");
            XmlNode particlePars =
                emitter.SelectSingleNode("/ParticleSystem/ParticleParameters");
            
            // load emitter properties

            //Location = LoadXMLVector3D(emitterPars.SelectSingleNode("location"));
            EmitDimensions = LoadVector3DFromNode(emitterPars.SelectSingleNode("dimension"));
            MaxNumParticles =
                (int)(GlobalParticleLevels * Convert.ToInt32(emitterPars.SelectSingleNode("maxNumParticles").
                Attributes.GetNamedItem("x").Value));

            EmitRate =
                (int)(GlobalParticleLevels * Convert.ToInt32(emitterPars.SelectSingleNode("emitRate").
                Attributes.GetNamedItem("x").Value));
            MeanEmitDelay =
                Convert.ToDouble(emitterPars.SelectSingleNode("meanEmitDelay").
                Attributes.GetNamedItem("x").Value);
            EmitLifetime =
                Convert.ToDouble(emitterPars.SelectSingleNode("emitterLifetime").
                Attributes.GetNamedItem("x").Value);
            PermanentParticles =
                Convert.ToBoolean(emitterPars.SelectSingleNode("permanentParticles").
                Attributes.GetNamedItem("x").Value);

            // load particle randomization properties.
            position = LoadParameter3DFromNode(particlePars.SelectSingleNode("position"));
            velocity = LoadParameter3DFromNode(particlePars.SelectSingleNode("velocity"));
            
            // TODO: implement speed and direction.
            // speed = LoadXMLParameterDouble(particlePars.SelectSingleNode("speed"));
            // direction = LoadXMLParameterDouble(particlePars.SelectSingleNode("direction"));
            acceleration = LoadParameter3DFromNode(particlePars.SelectSingleNode("acceleration"));
            color = LoadParameter3DFromNode(particlePars.SelectSingleNode("color"));
            angle = LoadParameterDoubleFromNode(particlePars.SelectSingleNode("angle"));
            angularVelocity = LoadParameterDoubleFromNode(particlePars.SelectSingleNode("angularVelocity"));
            transparency = LoadParameterDoubleFromNode(particlePars.SelectSingleNode("transparency"));
            transparencyDelta = LoadParameterDoubleFromNode(particlePars.SelectSingleNode("transparencyDelta"));
            size = LoadParameterDoubleFromNode(particlePars.SelectSingleNode("size"));
            size.alpha *= GlobalScaling;
            size.beta *= GlobalScaling;
            growth = LoadParameterDoubleFromNode(particlePars.SelectSingleNode("growth"));
            ttl = LoadParameterDoubleFromNode(particlePars.SelectSingleNode("ttl"));

        }

        /// <summary>
        /// UNIMPLEMENTED: Transpose magnitude and direction parameters to a 3D (x, y, z)-based one.
        /// </summary>
        /// <param name="magnitude">The magnitude parameter.</param>
        /// <param name="direction">The direction parameter.</param>
        /// <returns>Randomly-distributed magnitude and direction variables
        /// transformed to randomly-distributed 3D vectors.</returns>
        public Parameter3D MagnitudeDirectionTo3DVector(
            ParameterDouble magnitude, 
            ParameterDouble direction) 
        {
            Parameter3D vector = new Parameter3D();

            // TODO: Implement.

            return vector;
        }


        /// <summary>
        /// Load a double parameter from an XML node.
        /// </summary>
        /// <param name="node">The XML node to load the parameter from.</param>
        /// <returns>A double parameter loaded from the node's data.</returns>
        public ParameterDouble LoadParameterDoubleFromNode(XmlNode node)
        {
            ParameterDouble newPar = new ParameterDouble();

            newPar.alpha = Convert.ToDouble(node.Attributes.GetNamedItem("alpha").Value);
            newPar.beta = Convert.ToDouble(node.Attributes.GetNamedItem("beta").Value);
            newPar.distribution = (Distribution)Enum.Parse(typeof(Distribution),
                Convert.ToString(node.Attributes.GetNamedItem("distribution").Value));

            return newPar;
        }

        /// <summary>
        /// Load a 3D parameter from an XML node.
        /// </summary>
        /// <param name="node">The node to load the parameter from.</param>
        /// <returns>A 3D parameter loaded from the node's data.</returns>
        public Parameter3D LoadParameter3DFromNode(XmlNode node)
        {
            Parameter3D newPar = new Parameter3D();

            newPar.alpha = LoadVector3DFromNode(node.SelectSingleNode("alpha"));
            newPar.beta = LoadVector3DFromNode(node.SelectSingleNode("beta"));
            newPar.distribution = (Distribution)Enum.Parse(typeof(Distribution),
                Convert.ToString(node.SelectSingleNode("distribution").Attributes.GetNamedItem("x").Value));

            return newPar;
        }

        /// <summary>
        /// Load a 3D vector from the given XML node.
        /// </summary>
        /// <param name="node">The XML node to load the vector from.</param>
        /// <returns>A 3D vector loaded from the node's data.</returns>
        public Vector3D LoadVector3DFromNode(XmlNode node)
        {
            Vector3D vec = new Vector3D();

            vec.X = Convert.ToDouble(node.Attributes.GetNamedItem("x").Value);
            vec.Y = Convert.ToDouble(node.Attributes.GetNamedItem("y").Value);
            vec.Z = Convert.ToDouble(node.Attributes.GetNamedItem("z").Value);

            return vec;
        }
    }
}
