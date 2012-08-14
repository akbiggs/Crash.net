using System;
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
        Game parent;
        SpriteEffects spriteEffects = SpriteEffects.None;
        BlendState blendState = BlendState.Additive;
        SpriteSortMode spriteSortMode = SpriteSortMode.Deferred;

        private List<Texture2D> TextureList = new List<Texture2D>();
        Texture2D particleTexture;
        private bool USES_MULTIPLE_TEXTURES = false;

        /**
         * XML-based contructor. Initializes emitter
         * with parameters from xmlFileName.
         **/
        public XNAEmitter(Game p, Vector2 location, String xmlFileName, double pLevel = 1.0, double pScaling = 1.0) : base(pLevel, pScaling)
        {
            XmlDocument doc = new XmlDocument();
            Location = new Vector3D(location.X, location.Y, 0);
            parent = p;
            doc.Load(xmlFileName);
            LoadXMLEmitter(doc);
            LoadXNAXMLParameters(doc);
        }

        /**
         * Explicit contructor initializes emitter with 
         * specified parameters.
         **/
        public XNAEmitter(
            Vector3D positionMean, Vector3D positionVar, Distribution pDist,
            Vector3D velocityMean, Vector3D velocityVar, Distribution vDist,
            Vector3D accelerationMean, Vector3D accelerationVar, Distribution aDist,
            double angleMean, double angleVar, Distribution angDist,
            double angVelocityMean, double angVelocityVar, Distribution angVelDist,
            Vector3D colorMean, Vector3D colorVar, Distribution colDist,
            double alphaMean, double alphaVar, Distribution transDist,
            double alphaDeltaMean, double alphaDeltaVar, Distribution transDeltaDist,
            double sizeMean, double sizeVar, Distribution sizeDist,
            double sizeDeltaMean, double sizeDeltaVar, Distribution sizeGrowthDist,
            double ttlMean, double ttlVar, Distribution ttlDist,
            Vector3D location,
            Vector3D dimension,
            int maxNumPart,
            int emitRate,
            int emitDelay,
            int emitLife,
            bool permParts)
            : base(positionMean, positionVar, pDist,
            velocityMean, velocityVar, vDist,
            accelerationMean, accelerationVar, aDist,
            angleMean, angleVar, angDist,
            angVelocityMean, angVelocityVar, angVelDist,
            colorMean, colorVar, colDist,
            alphaMean, alphaVar, transDist,
            alphaDeltaMean, alphaDeltaVar, transDeltaDist,
            sizeMean, sizeVar, sizeDist,
            sizeDeltaMean, sizeDeltaVar, sizeGrowthDist,
            ttlMean, ttlVar,ttlDist,
            location,
            dimension,
             maxNumPart,
             emitRate,
             emitDelay,
             emitLife,
             permParts)
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
                Texture2D n = parent.Content.Load<Texture2D>(
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

            spriteBatch.Begin(spriteSortMode, blendState);
            foreach (Particle p in particles)
            {
                if ((p.TTL > 0) || PermanentParticles)
                {
                    if (USES_MULTIPLE_TEXTURES)
                    {
                        DrawTexture = TextureList[p.TextureIndex];
                    }
                    spriteBatch.Draw(DrawTexture, 
                        new Vector2((float)p.position.X, (float)p.position.Y), 
                        new Rectangle(0,0, particleTexture.Width, particleTexture.Height), 
                        new Color((float)p.color.X, (float)p.color.Y, (float)p.color.Z, (float)p.transparency), 
                        (float)p.angle,
                        new Vector2(particleTexture.Width / 2, particleTexture.Height / 2), 
                        (float)p.size, 
                        spriteEffects, 0f); 

                }
            }

            spriteBatch.End();
            
        }
    }
}
