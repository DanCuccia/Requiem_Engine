using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine.Managers;
using Microsoft.Xna.Framework;
using Engine.Math_Physics;
#pragma warning disable 1591

namespace Engine.Drawing_Objects.ParticleSystems
{
    /// <summary>custom particle for this starfield emitter</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class StarParticle : QuadParticle
    {
        public float myScale = 1f;
        public StarParticle(int i, Texture2D t)
            : base(i, t)
        { }
    }

    /// <summary>StarField emitter</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class StarField : ParticleSystem
    {
        Texture2D texture;

        #region Init

        /// <summary>Default CTOR, construction logic governed by base ParticleSystem class</summary>
        /// <param name="maxParticles">maximum amount of particle to aloc for</param>
        /// <param name="content">xna content manager</param>
        public StarField(ContentManager content, int maxParticles = 512)
            : base(content, maxParticles)
        {
            base.ParticleSystemID = GameIDList.PS_BillBoardParticle_StarField;
        }

        /// <summary>load textures etc</summary>
        protected override void LoadAssets(ContentManager content)
        {
            texture = TextureManager.getInstance().GetTexture("particle//star");
        }

        /// <summary>define how to create a particle</summary>
        /// <returns>custom tornado particle</returns>
        protected override Particle AllocateParticle()
        {
            return new StarParticle(base.InstructionLength, this.texture);
        }

        /// <summary>set general parameters</summary>
        protected override EmissionSettings LoadEmissionSettings(ref EmissionSettings settings)
        {
            settings.EmissionRate = 0;
            settings.EmissionRateVariance = 0;
            settings.FaceCamera = true;
            settings.SpawnBox.X = 100000f;
            settings.SpawnBox.Z = 100000f;
            settings.UseGravity = false;
            settings.InitialVelocity = Vector3.Zero;
            settings.MaximumLife = 60000f;
            settings.MaximumDistance = 10000000f;
            return settings;
        }
        
        /// <summary>define how a particle lives its life</summary>
        protected override List<ParticleEvent> LoadParticleLifeInstructions()
        {
            List<ParticleEvent> life = new List<ParticleEvent>();
            life.Add(new ParticleEvent(0f, ptl_scaleIn, 1000f));
            life.Add(new ParticleEvent(emissionSettings.MaximumLife - 1000f, ptl_scaleOut, 1000f));
            return life;
        }

        #region instructions
        public void ptl_scaleIn(ParticleEvent e, GameTime time, Particle ptl, float durationScalar)
        {
            StarParticle p = ptl as StarParticle;
            ptl.WorldMatrix.UniformScale = durationScalar * p.myScale;
        }
        public void ptl_scaleOut(ParticleEvent e, GameTime time, Particle ptl, float durationScalar)
        {
            StarParticle p = ptl as StarParticle;
            ptl.WorldMatrix.UniformScale = (1 - durationScalar) * p.myScale;
        }
        #endregion instructions

        #endregion Init

        #region API

        /// <summary>define how the particles spawn</summary>
        protected override Particle CreateParticle(ref Particle particle)
        {
            StarParticle ptl = particle as StarParticle;
            ptl.myScale = MyMath.GetRandomFloat(10.1f, 100.9f);
            return particle;
        }

        /// <summary>define how to reset a particle to default states</summary>
        protected override Particle ResetParticle(ref Particle particle)
        {
            StarParticle ptl = particle as StarParticle;
            ptl.myScale = MyMath.GetRandomFloat(0.1f, 0.9f);
            return ptl;
        }

        /// <summary>Last Minute update logic</summary>
        protected override void ParticleUpdate(GameTime time, ref Particle particle)
        { }

        #endregion API

        #region Serialization
        public override XMLMedium GetXML() { return null; }
        public override Object3D GetCopy(ContentManager content) { return null; }
        public override void CreateFromXML(ContentManager content, XMLMedium inputXml) { }
        #endregion
    }
}
