using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine.Managers;
#pragma warning disable 1591

namespace Engine.Drawing_Objects.ParticleSystems
{
    /// <summary>this emits a sparkles</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class GlitterEmitter : ParticleSystem
    {
        Texture2D texture;

        #region Init
        /// <summary>Default CTOR, construction logic governed by base ParticleSystem class</summary>
        /// <param name="maxParticles">maximum amount of particle to aloc for</param>
        /// <param name="content">xna content manager</param>
        public GlitterEmitter(ContentManager content, int maxParticles = 60)
            : base(content, maxParticles)
        {
            base.ParticleSystemID = GameIDList.PS_BillBoardParticle_Glitter;
        }

        /// <summary>Initialize how this emitter spits out particles</summary>
        /// <param name="settings">edit this settings struct</param>
        /// <returns>return the editing struct</returns>
        protected override EmissionSettings LoadEmissionSettings(ref EmissionSettings settings)
        {
            settings.MaximumLife = 3000;
            settings.InitialTurbulance = new Vector3(1.1f, 0f, 1.1f);
            settings.InitialVelocityVariance = 1f;
            settings.InitialVelocity.Y = 1.7f;
            settings.UseGravity = settings.FaceCamera = true;
            settings.EmissionRate = 50f;
            settings.Gravity.Y = .02f;
            return settings;
        }

        /// <summary>Load textures and stuff</summary>
        protected override void LoadAssets(ContentManager content)
        {
            texture = TextureManager.getInstance().GetTexture("particle//star");
        }

        /// <summary>create, and initialize your particle, whatever type it may be, 
        /// this is only called once per particle ever.</summary>
        /// <returns>give the newly allocated particle back to the system</returns>
        protected override Particle AllocateParticle()
        {
            return new QuadParticle(base.InstructionLength, this.texture);
        }

        /// <summary>Load the instructions depicting how a particle lives</summary>
        /// <returns>the list of instructions, it will be sorted and converted to an array</returns>
        protected override List<ParticleEvent> LoadParticleLifeInstructions()
        {
            List<ParticleEvent> output = new List<ParticleEvent>();

            output.Add(new ParticleEvent(0, ptl_grow, 250));
            output.Add(new ParticleEvent(2750, ptl_shrink, 250));
            output.Add(new ParticleEvent(500, ptl_flash, 2200));

            return output;
        }
        #endregion

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
        protected override void ParticleUpdate(GameTime time, ref Particle particle)
        {        }

        #endregion

        #region instructions

        public void ptl_grow(ParticleEvent e, GameTime time, Particle particle, float durationScalar)
        {
            particle.WorldMatrix.UniformScale = (durationScalar) * .1f;
        }

        public void ptl_shrink(ParticleEvent e, GameTime time, Particle particle, float durationScalar)
        {
            particle.WorldMatrix.UniformScale = (1 - durationScalar) * -.1f;
        }

        public void ptl_flash(ParticleEvent e, GameTime time, Particle particle, float durationScalar)
        {
            particle.WorldMatrix.UniformScale = (float)random.NextDouble() * 0.4f + 0.1f;
        }

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
