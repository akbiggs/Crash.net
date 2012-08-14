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
        /**
         * Members
         **/

        /**
         * Global FX levels for particle system.
         **/
        private double GlobalParticleLevels;

        /**
         * Size scaling.
         **/
        private double GlobalScaling;

        /**
         * Random number generator used for initializing particles.
         **/
        
        private GRandom grandom;

        /**
         * List contains references to all active particles until removed.
         **/
        public List<Particle> particles;

        

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

        /**
         * Position is relative to emitter's centre.
         **/
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
      
        /**
         * Particle Emitter behaviour parameters. These properties 
         * belong to the emitter itself, not to the particles.
         **/
        public Vector3D Location { get; set; } // Center point of emitter.
        public Vector3D EmitDimensions { get; set; } // Area from which particles can be emitted. currently unused.     
        public int MaxNumParticles { get; set; } // Max number of particles allowed in queue.
        public int EmitRate { get; set; } // How many particles we can generate each update.
        public double MeanEmitDelay { get; set; } // Mean time between individual new particle emits, in Secs.
        public double EmitLifetime { get; set; } // How long emitter emits for, in MS.
        public double CurrentLifeTime { get; set; } // Running total of time spent emitting, in Secs.
        public double TimeToNextEmit { get; set; } // How long ago we last emitted, in Secs.
        public bool EmitActivity { get; set; } // Whether emitter should currently emitting particles.
        public bool PermanentParticles { get; set; } // Are particles permanent or do they disappear?
        public int NumTextures { get; set; } // Used for multi-texture emitters.

        /**
         * Constructors
         **/
        public ParticleEmitter(double pLevel = 1.0, double pScaling = 1.0)
        {
            GlobalParticleLevels = pLevel;
            GlobalScaling = pScaling;
            CurrentLifeTime = 0;
            grandom = new GRandom();
            particles = new List<Particle>();
        }

        /**
         * Constructor taking filename with particle parameters.
         **/
        public ParticleEmitter(Vector3D location, String paramFileName, double pLevel = 1.0, double pScaling = 1.0) : this(pLevel, pScaling)
        {
            Location = location;
        }

        /**
         * Explicit Constructor.
         **/ 
        public ParticleEmitter(
            /** Particle parameters **/
            Vector3D positionMean, Vector3D positionVar, Distribution pDist,
            Vector3D velocityMean, Vector3D velocityVar, Distribution vDist,
            Vector3D accelerationMean, Vector3D accelerationVar, Distribution aDist,
            double angleMean, double angleVar, Distribution angleDist,
            double angVelocityMean, double angVelocityVar, Distribution angVelDist,
            Vector3D colorMean, Vector3D colorVar, Distribution colDist,
            double alphaMean, double alphaVar, Distribution tranDist,
            double alphaDeltaMean, double alphaDeltaVar, Distribution tranDeltaDist,
            double sizeMean, double sizeVar, Distribution sizeDist,
            double sizeDeltaMean, double sizeDeltaVar, Distribution sizeDeltaDist,
            double ttlMean, double ttlVar, Distribution ttlDist,
            /** Emitter Parameters **/
            Vector3D location,
            Vector3D dimension,
            int maxNumPart,
            int emitRate,
            int emitDelay,
            int emitLife,
            bool permParts,
            double pLevel = 1.0
            ) : this(pLevel) 
        {
            this.position.alpha = positionMean;
            this.position.beta = positionVar;
            this.position.distribution = pDist;

            this.velocity.alpha = velocityMean;
            this.velocity.beta = velocityVar;
            this.velocity.distribution = vDist;

            this.acceleration.alpha = accelerationMean;
            this.acceleration.beta = accelerationVar;
            this.acceleration.distribution = aDist;

            this.angle.alpha = angleMean;
            this.angle.beta = angleVar;
            this.angle.distribution = angleDist;

            this.angularVelocity.alpha = angVelocityMean;
            this.angularVelocity.beta = angVelocityVar;
            this.angularVelocity.distribution = angVelDist;

            this.color.alpha = colorMean;
            this.color.beta = colorVar;
            this.color.distribution = colDist;

            this.transparency.alpha = alphaMean;
            this.transparency.beta = alphaVar;
            this.transparency.distribution = tranDist;

            this.transparencyDelta.alpha = alphaDeltaMean;
            this.transparencyDelta.beta = alphaDeltaVar;
            this.transparencyDelta.distribution = tranDeltaDist;

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
            this.MaxNumParticles = maxNumPart;
            this.EmitRate = emitRate;
            this.MeanEmitDelay = emitDelay;
            this.EmitLifetime = emitLife;
            this.PermanentParticles = permParts;

            NumTextures = 0;   
        }

        /**
         * Returns a new particle with randomized properties based on
         * emitter parameters.
         **/
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

        /**
         * Starts the emitter.
         **/
        public void Start()
        {
            EmitActivity = true;
            CurrentLifeTime = 0;
            TimeToNextEmit = MeanEmitDelay;
        }

        /** 
         * Immediately cease to emit particles.
         **/
        public void Stop()
        {
            EmitActivity = false;
        }

        /**
         * Returns true if emitter is either currently emitting, has not yet started to emit, or
         * no longer emitting but still has particles in queue to be updated.
         **/
        public bool IsAlive()
        {
            return ((EmitActivity) || (CurrentLifeTime == 0) || (particles.Count != 0));
        }

        /** 
         * Updates every particle for this emitter.
         **/
        public void Update(double MsSinceLastUpdate /**MilliSeconds**/)
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
                    if ((TimeToNextEmit <= 0) && (EmitActivity))
                    {
                        /**
                        * If emitter is still emitting, and it's been
                        * long enough since the last emit, let's make
                        * some more particles.
                        **/
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
            
            /** 
             * Update existing particles:
             **/
            for (int index = 0; index < particles.Count; index++)
            {
                // If particle is still alive or emitter uses perma particles.
                if ((particles[index].TTL > 0) || PermanentParticles)
                {
                    particles[index].Update(MsSinceLastUpdate);
                }
                else
                {
                    /**
                    * Remove dead particle from queue.
                    **/
                    particles.RemoveAt(index);
                    index--;
                }
            }
        }

        /**
        * Loads emitter parameters from XML file.
        **/
        public void LoadXMLEmitter(XmlDocument emitter)
        {
            XmlNode emitterPars =
                emitter.SelectSingleNode("/ParticleSystem/EmitterParameters");
            XmlNode particlePars =
                emitter.SelectSingleNode("/ParticleSystem/ParticleParameters");
            /**
             * Load Emitter Parameters:
             **/
            //Location = LoadXMLVector3D(emitterPars.SelectSingleNode("location"));
            EmitDimensions = LoadXMLVector3D(emitterPars.SelectSingleNode("dimension"));
            MaxNumParticles =
                (int)(GlobalParticleLevels * Convert.ToInt32(emitterPars.SelectSingleNode("maxNumParticles").
                Attributes.GetNamedItem("x").Value));

            EmitRate =
                (int)(GlobalParticleLevels * Convert.ToInt32(emitterPars.SelectSingleNode("emitRate").
                Attributes.GetNamedItem("x").Value));
            MeanEmitDelay =
                Convert.ToInt32(emitterPars.SelectSingleNode("meanEmitDelay").
                Attributes.GetNamedItem("x").Value);
            EmitLifetime =
                Convert.ToInt32(emitterPars.SelectSingleNode("emitterLifetime").
                Attributes.GetNamedItem("x").Value);
            PermanentParticles =
                Convert.ToBoolean(emitterPars.SelectSingleNode("permanentParticles").
                Attributes.GetNamedItem("x").Value);

            /**
             * Load particle randomization parameters:
             **/
            position = LoadXMLParameter3D(particlePars.SelectSingleNode("position"));
            velocity = LoadXMLParameter3D(particlePars.SelectSingleNode("velocity"));
            /**
             * Speed and Direction: Not implemented yet. 
             **/
            // speed = LoadXMLParameterDouble(particlePars.SelectSingleNode("speed"));
            // direction = LoadXMLParameterDouble(particlePars.SelectSingleNode("direction"));
            acceleration = LoadXMLParameter3D(particlePars.SelectSingleNode("acceleration"));
            color = LoadXMLParameter3D(particlePars.SelectSingleNode("color"));
            angle = LoadXMLParameterDouble(particlePars.SelectSingleNode("angle"));
            angularVelocity = LoadXMLParameterDouble(particlePars.SelectSingleNode("angularVelocity"));
            transparency = LoadXMLParameterDouble(particlePars.SelectSingleNode("transparency"));
            transparencyDelta = LoadXMLParameterDouble(particlePars.SelectSingleNode("transparencyDelta"));
            size = LoadXMLParameterDouble(particlePars.SelectSingleNode("size"));
            size.alpha *= GlobalScaling;
            size.beta *= GlobalScaling;
            growth = LoadXMLParameterDouble(particlePars.SelectSingleNode("growth"));
            ttl = LoadXMLParameterDouble(particlePars.SelectSingleNode("ttl"));

        }


        /**
         * Transposes a parameter using magnitude and direction to 
         * one using a 3D vector.
         * TODO: Implement.
         **/
        public Parameter3D MagnitudeDirectionTo3DVector(
            ParameterDouble magnitude, 
            ParameterDouble direction) 
        {
            Parameter3D vector = new Parameter3D();
            
            /**
             * Randomly-distributed magnitude and direction variables
             * transformed to randomly-distributed 3d x,y,z vectors.
             **/

            return vector;
        }


        /** 
         * Loads a Double parameter from the specified
         * XMLNode.
         **/
        public ParameterDouble LoadXMLParameterDouble(XmlNode node)
        {
            ParameterDouble newPar = new ParameterDouble();

            newPar.alpha = Convert.ToDouble(node.Attributes.GetNamedItem("alpha").Value);
            newPar.beta = Convert.ToDouble(node.Attributes.GetNamedItem("beta").Value);
            newPar.distribution = (Distribution)Enum.Parse(typeof(Distribution),
                Convert.ToString(node.Attributes.GetNamedItem("distribution").Value));

            return newPar;
        }

        /**
         * Loads a 3D Parameter from the specified XMLNode
         **/
        public Parameter3D LoadXMLParameter3D(XmlNode node)
        {
            Parameter3D newPar = new Parameter3D();

            newPar.alpha = LoadXMLVector3D(node.SelectSingleNode("alpha"));
            newPar.beta = LoadXMLVector3D(node.SelectSingleNode("beta"));
            newPar.distribution = (Distribution)Enum.Parse(typeof(Distribution),
                Convert.ToString(node.SelectSingleNode("distribution").Attributes.GetNamedItem("x").Value));

            return newPar;
        }

        /**
         * Parses and returns a Vector3D from the specified xmlnode.
         **/
        public Vector3D LoadXMLVector3D(XmlNode node)
        {
            Vector3D vec = new Vector3D();

            vec.X = Convert.ToDouble(node.Attributes.GetNamedItem("x").Value);
            vec.Y = Convert.ToDouble(node.Attributes.GetNamedItem("y").Value);
            vec.Z = Convert.ToDouble(node.Attributes.GetNamedItem("z").Value);

            return vec;
        }
    }
}
