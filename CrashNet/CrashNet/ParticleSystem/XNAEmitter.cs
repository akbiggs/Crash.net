﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrashNet.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using CrashNet.GRNG;

namespace CrashNet.ParticleSystem
{
    class XNAEmitter : ParticleEmitter
    {
        SpriteEffects spriteEffects = SpriteEffects.None;
        BlendState blendState = BlendState.Additive;
        SpriteSortMode spriteSortMode = SpriteSortMode.Deferred;

        private List<Texture2D> TextureList = new List<Texture2D>();
        Texture2D particleTexture;
        private bool USES_MULTIPLE_TEXTURES = false;

        /// <summary>
        /// XML-based constructor.
        /// Initalizes emitter with parameters from given XML file.
        /// </summary>
        /// <param name="location">The position of the emitter.</param>
        /// <param name="xmlFileName">The name of the file containing the parameters for the emitter.</param>
        /// <param name="pLevel"></param>
        /// <param name="pScaling"></param>
        public XNAEmitter(/**Game p,**/ Vector2 location, String xmlFileName, double pLevel = 1.0, double pScaling = 1.0) 
            : base(pLevel, pScaling)
        {
            XmlDocument doc = new XmlDocument();
            Location = new Vector3D(location.X, location.Y, 0);
            /**parent = p;**/
            doc.Load(xmlFileName);
            LoadEmitterFromXML(doc);
            LoadXNAXMLParameters(doc);
        }

        /// <summary>
        /// Initializes emitter with specified parameters.
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
        public XNAEmitter(
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
            double lifespanMean, double lifespanVar, Distribution lifespanDist,
            Vector3D location,
            Vector3D dimension,
            int maxNumPart,
            int emitRate,
            int emitDelay,
            int emitLife,
            bool permanentParticles)
            : base(positionMean, positionVar, positionDist,
            velocityMean, velocityVar, velocityDist,
            accelerationMean, accelerationVar, accelerationDist,
            angleMean, angleVar, angleDist,
            angleVelocityMean, angleVelocityVar, angleVelDist,
            colorMean, colorVar, colorDist,
            alphaMean, alphaVar, alphaDist,
            alphaDeltaMean, alphaDeltaVar, alphaDeltaDist,
            sizeMean, sizeVar, sizeDist,
            sizeDeltaMean, sizeDeltaVar, sizeDeltaDist,
            lifespanMean, lifespanVar,lifespanDist,
            location,
            dimension,
             maxNumPart,
             emitRate,
             emitDelay,
             emitLife,
             permanentParticles)
        {
             
        }

        /**
         * Wrapper for Location Vector3D.
         **/
        public void SetLocation(Vector2 location)
        {
            Location = new Vector3D(location.X, location.Y, 0);
        }

        /**
         * Loads emitter parameters from XML Document.
         **/
        public void LoadXNAXMLParameters(XmlDocument doc)
        {
            XmlNode XNAPars =
                doc.SelectSingleNode("/ParticleSystem/XNAParameters");

            spriteEffects = (SpriteEffects)Enum.Parse(typeof(SpriteEffects),
                Convert.ToString(XNAPars.SelectSingleNode("spriteEffects").
                Attributes.GetNamedItem("x").Value));

            String blendString = Convert.ToString(XNAPars.SelectSingleNode("blendState").
                Attributes.GetNamedItem("x").Value);

            if (blendString == "AlphaBlend")
            {
                blendState = BlendState.AlphaBlend;
            }
            if (blendString == "NonPremultiplied")
            {
                blendState = BlendState.NonPremultiplied;
            }
            if (blendString == "Opaque")
            {
                blendState = BlendState.Opaque;
            }
            else
            {
                blendState = BlendState.Additive;
            }

            spriteSortMode = (SpriteSortMode)Enum.Parse(typeof(SpriteSortMode),
                Convert.ToString(XNAPars.SelectSingleNode("spriteSortMode").
                Attributes.GetNamedItem("x").Value));

            foreach (XmlNode texture in XNAPars.SelectSingleNode("textureList"))
            {
                /** Old way of doing it using Game class:
                Texture2D n = parent.Content.Load<Texture2D>(
                    Convert.ToString(texture.Attributes.GetNamedItem("x").Value));
                 * **/
                // New way using TextureManager class:
                Texture2D n = TextureManager.GetTexture(
                    Convert.ToString(texture.Attributes.GetNamedItem("x").Value));
                
                LoadTexture(n);
            }
        }

        /**
         * Adds textures to the list of particle
         * textures.
         **/
        public void LoadTexture(Texture2D texture)
        {
            TextureList.Add(texture);
            NumTextures++;
            if (TextureList.Count > 1)
            {
                USES_MULTIPLE_TEXTURES = true;
            }
            else
            {
                particleTexture = texture;
            }
        }

        /**
         * A setter for manually modifying the emitter's
         * blendstate. Useful for changing advanced blendstate
         * options not doable through the XML init.
         **/
        public void SetBlendState(BlendState bstate)
        {
            blendState = bstate;
        }

       
        /**
         * XNA-specific Draw method.
         **/
        public void Draw(SpriteBatch spriteBatch)
        {
            
            Texture2D DrawTexture = particleTexture;
            spriteBatch.End();
            spriteBatch.Begin(spriteSortMode, blendState);
            foreach (Particle p in particles)
            {
                if ((p.lifeLeft > 0) || PermanentParticles)
                {
                    if (USES_MULTIPLE_TEXTURES)
                    {
                        DrawTexture = TextureList[p.TextureIndex];
                    }
                    spriteBatch.Draw(DrawTexture, 
                        new Vector2((float)p.position.X, (float)p.position.Y), 
                        new Rectangle(0,0, particleTexture.Width, particleTexture.Height),
                        new Color((int)p.color.X, (int)p.color.Y, (int)p.color.Z, (int)(p.transparency * 255)), 
                        (float)p.angle,
                        new Vector2(particleTexture.Width / 2, particleTexture.Height / 2), 
                        (float)p.size, 
                        spriteEffects, 0f); 

                }
            }
            spriteBatch.End();
            spriteBatch.Begin();
            
        }
    }
}
