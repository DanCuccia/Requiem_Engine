using System;
using System.Collections.Generic;
using Engine.Managers;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#pragma warning disable 1591

namespace Engine.Drawing_Objects.ParticleSystems
{
    /// <summary>particle emitter used to display player healing effects</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class HealingEmitter : ParticleSystem
    {
        Texture2D texture;

        private const float startDistMax = 50f;
        private const float startDistMin = 10f;
        private const float endDistMax = 40f;
        private const float endDistMin = 15f;
        private const float startSpeedMin = 0.01f;
        private const float startSpeedMax = 0.07f;
        private const float endSpeedMin = 0.02f;
        private const float endSpeedMax = 0.1f;
        private const float startDSpeedMin = 0.04f;
        private const float startDSpeedMax = 1.3f;

        #region Init
        /// <summary>default CTOR</summary>
        /// <param name="content">xna content manager</param>
        /// <param name="maxParticles">maximum amount of particles to allocate for</param>
        public HealingEmitter(ContentManager content, int maxParticles = 32)
            :base(content, maxParticles)
        {
            base.ParticleSystemID = GameIDList.PS_BillBoardParticle_Healing;
        }

        /// <summary>overriden to a specific size</summary>
        public override void GenerateBoundingBox()
        {
            this.OBB = new OrientedBoundingBox(new Vector3(-20f, -20f, -20f), new Vector3(20f, 20f, 20f));
        }

        /// <summary>initialize the emission of the system</summary>
        /// <param name="settings">reference to the base settings object</param>
        /// <returns>settings going back to base class for control</returns>
        protected override EmissionSettings LoadEmissionSettings(ref EmissionSettings settings)
        {
            settings.EmissionRate = 0;
            settings.EmissionRateVariance = 5;
            settings.FaceCamera = settings.UseGravity = true;
            settings.SpawnBox.X = settings.SpawnBox.Z = 150f;
            settings.MaximumLife = 4500;
            settings.Gravity.Y = -0.008f;
            settings.InitialVelocity = Vector3.Zero;
            return settings;
        }

        /// <summary>load textures</summary>
        /// <param name="content">xna content manager</param>
        protected override void LoadAssets(ContentManager content)
        {
            texture = TextureManager.getInstance().GetTexture("particle//star");
        }

        /// <summary>define what each particle is</summary>
        /// <returns>newly allocated particle</returns>
        protected override Particle AllocateParticle()
        {
            return new TornadoQuadParticle(base.InstructionLength, this.texture);
        }

        /// <summary>load the particle's instruction list</summary>
        /// <returns>a list of particle events, which gets converted to an array</returns>
        protected override List<ParticleEvent> LoadParticleLifeInstructions()
        {
            List<ParticleEvent> output = new List<ParticleEvent>();
            output.Add(new ParticleEvent(0, ptl_orbit, emissionSettings.MaximumLife));
            return output;
        }
        private void ptl_orbit(ParticleEvent e, GameTime time, Particle ptl, float durationScalar)
        {
            TornadoQuadParticle tptl = ptl as TornadoQuadParticle;

            tptl.angle += tptl.speed;
            tptl.dist = MathHelper.Lerp(HealingEmitter.startDistMax, HealingEmitter.endDistMin, tptl.distSpeed);

            tptl.Quad.WorldMatrix.X = this.WorldMatrix.X + (tptl.dist * (float)Math.Cos(tptl.angle));
            tptl.Quad.WorldMatrix.Z = this.WorldMatrix.Z + (tptl.dist * (float)Math.Sin(tptl.angle));
        }

        #endregion Init

        #region API
        /// <summary>Setup the recently reset particle</summary>
        /// <param name="particle">this particle was reset by the system with automated velocity and position</param>
        /// <returns>any edited parameters</returns>
        protected override Particle CreateParticle(ref Particle particle)
        {
            particle.WorldMatrix.UniformScale = MyMath.GetRandomFloat(0.005f, 0.3f);
            return particle; 
        }

        /// <summary>optional update</summary>
        protected override void ParticleUpdate(GameTime time, ref Particle particle) { }
        #endregion

        #region Serialization
        public override XMLMedium GetXML()  { return null; }
        public override Object3D GetCopy(ContentManager content) { return null; }
        public override void CreateFromXML(ContentManager content, XMLMedium inputXml) { }
        #endregion
    }
}
