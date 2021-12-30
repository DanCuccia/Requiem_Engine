using System;
using System.Collections.Generic;
using Engine.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine.Math_Physics;
#pragma warning disable 1591

namespace Engine.Drawing_Objects.ParticleSystems
{
    /// <summary>Custom Particle system used for a player's ability</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class LightningTrailEmitter : ParticleSystem
    {
        Texture2D texture;

        #region Init
        /// <summary>default CTOR</summary>
        /// <param name="content">xna content manager</param>
        /// <param name="maxParticles">maximum amount of particles to allocate for</param>
        public LightningTrailEmitter(ContentManager content, int maxParticles = 100)
            :base(content, maxParticles)
        {
            base.ParticleSystemID = GameIDList.PS_BillBoardParticle_Lightning;
        }

        /// <summary>overriden to a specific size</summary>
        public override void GenerateBoundingBox()
        {
            this.OBB = new Math_Physics.OrientedBoundingBox(new Vector3(-10f, -10f, -10f), new Vector3(10f, 10f, 10f));
        }

        /// <summary>initialize the emission of the system</summary>
        /// <param name="settings">reference to the base settings object</param>
        /// <returns>settings going back to base class for control</returns>
        protected override EmissionSettings LoadEmissionSettings(ref EmissionSettings settings)
        {
            settings.BeginningAlpha = 1f;
            settings.EmissionRate = 0f;
            settings.FaceCamera = false;
            settings.Gravity = settings.InitialTurbulance = Vector3.Zero;
            settings.InitialVelocity.Y = settings.InitialVelocity.Z = 1.5f;
            settings.MaximumLife = 2500f;
            settings.MaximumDistance = 500f;
            settings.ParticleColor = Color.FromNonPremultiplied(255, 252, 175, 255);
            return settings;
        }

        /// <summary>load textures</summary>
        /// <param name="content">xna content manager</param>
        protected override void LoadAssets(ContentManager content)
        {
            this.texture = content.Load<Texture2D>("particle//lightning");
        }

        /// <summary>define what each particle is</summary>
        /// <returns>newly allocated particle</returns>
        protected override Particle AllocateParticle()
        {
            return new QuadParticle(base.InstructionLength, this.texture);
        }

        /// <summary>load the particle's instruction list</summary>
        /// <returns></returns>
        protected override List<ParticleEvent> LoadParticleLifeInstructions()
        {
            List<ParticleEvent> output = new List<ParticleEvent>();

            output.Add(new ParticleEvent(0f, ptl_grow, 500f));

            return output;
        }

        #region instructions
        public void ptl_grow(ParticleEvent e, GameTime t, Particle p, float d)
        {
            p.WorldMatrix.UniformScale = d * .1f;
        }
        #endregion

        #endregion Init

        #region API
        /// <summary>Setup the recently reset particle</summary>
        /// <param name="particle">this particle was reset by the system with automated velocity and position</param>
        /// <returns>any edited parameters</returns>
        protected override Particle CreateParticle(ref Particle particle)
        {
            particle.WorldMatrix.UniformScale = 0f;
            return particle;
        }

        /// <summary>optional update</summary>
        protected override void ParticleUpdate(GameTime time, ref Particle particle) { }
        #endregion

        #region Serialization
        public override XMLMedium GetXML()
        {
            ParticleSystemXML output = new ParticleSystemXML();
            output.id = base.ParticleSystemID;
            output.settings = this.emissionSettings;
            output.worldMatrix = this.WorldMatrix.GetXml();
            return output;
        }
        public override Object3D GetCopy(ContentManager content)
        {
            GlitterEmitter output = new GlitterEmitter(content);
            output.emissionSettings = this.emissionSettings.GetCopy();
            output.WorldMatrix = this.WorldMatrix.Clone();
            return output;
        }
        public override void CreateFromXML(ContentManager content, XMLMedium inputXml)
        {
            ParticleSystemXML input = (ParticleSystemXML)inputXml;
            this.emissionSettings = input.settings;
            this.WorldMatrix.FromXML(input.worldMatrix);
            this.ReInitializeInstructions();
        }
        #endregion
    }
}
