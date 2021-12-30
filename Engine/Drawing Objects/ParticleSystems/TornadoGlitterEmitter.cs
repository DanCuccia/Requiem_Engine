using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine.Managers;
using Engine.Drawing_Objects.Materials;
using Engine.Math_Physics;
#pragma warning disable 1591

namespace Engine.Drawing_Objects.ParticleSystems
{
    /// <summary>the tornado effect needs to track some extra variables for the math</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class TornadoQuadParticle : QuadParticle
    {
        public TornadoQuadParticle(int instructionCount, Texture2D texture = null)
            :base(instructionCount, texture)
        { }
        public float angle = 0f;
        public float dist = 100f;
        public float distSpeed = 0.01f;
        public float speed = .5f;
    }

    /// <summary>This emitter creates a tornado-like downward swirl of particles</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class TornadoGlitterEmitter : ParticleSystem
    {
        Texture2D texture;

        private const float startDistMax = 190f;
        private const float startDistMin = 140f;
        private const float endDistMax = 40f;
        private const float endDistMin = 15f;
        private const float startSpeedMin = 0.01f;
        private const float startSpeedMax = 0.07f;
        private const float endSpeedMin = 0.02f;
        private const float endSpeedMax = 0.1f;
        private const float startDSpeedMin = 0.04f;
        private const float startDSpeedMax = 01.3f;

        #region Init

        /// <summary>Default CTOR, construction logic governed by base ParticleSystem class</summary>
        /// <param name="maxParticles">maximum amount of particle to aloc for</param>
        /// <param name="content">xna content manager</param>
        public TornadoGlitterEmitter(ContentManager content, int maxParticles = 128)
            : base(content, maxParticles)
        {
            base.ParticleSystemID = GameIDList.PS_BillBoardParticle_Tornado;
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
            TornadoQuadParticle ptl = new TornadoQuadParticle(base.InstructionLength, this.texture);
            ptl.Quad.Material = new TexturedQuadMaterial(ptl.Quad, texture);
            ptl.Quad.WorldMatrix.UniformScale = .1f;
            return ptl;
        }

        /// <summary>set general parameters</summary>
        protected override EmissionSettings LoadEmissionSettings(ref EmissionSettings settings)
        {
            settings.EmissionRate = 45;
            settings.EmissionRateVariance = 5;
            settings.FaceCamera = settings.UseGravity = true;
            settings.SpawnBox.X = settings.SpawnBox.Z = 150f;
            settings.MaximumLife = 4500;
            settings.Gravity.Y = 0.008f;
            settings.InitialVelocity = Vector3.Zero;
            return settings;
        }
        
        /// <summary>define how a particle lives its life</summary>
        protected override List<ParticleEvent> LoadParticleLifeInstructions()
        {
            List<ParticleEvent> life = new List<ParticleEvent>();
            life.Add(new ParticleEvent(0, ptl_orbit, emissionSettings.MaximumLife));
            return life;
        }

        #region instructions
        public void ptl_orbit(ParticleEvent e, GameTime time, Particle ptl, float durationScalar)
        {
            TornadoQuadParticle tptl = ptl as TornadoQuadParticle;

            tptl.angle += tptl.speed;
            tptl.dist = MathHelper.Lerp(TornadoGlitterEmitter.startDistMax, TornadoGlitterEmitter.endDistMin, tptl.distSpeed);

            tptl.Quad.WorldMatrix.X = this.WorldMatrix.X + (tptl.dist * (float)Math.Cos(tptl.angle));
            tptl.Quad.WorldMatrix.Z = this.WorldMatrix.Z + (tptl.dist * (float)Math.Sin(tptl.angle));
        }
        public void ptl_scaleOut(ParticleEvent e, GameTime time, Particle ptl, float durationScalar)
        {
            ptl.WorldMatrix.UniformScale = 1 - durationScalar;
        }
        #endregion instructions

        #endregion Init

        #region API

        /// <summary>define how the particles spawn</summary>
        protected override Particle CreateParticle(ref Particle particle)
        {
            TornadoQuadParticle ptl = particle as TornadoQuadParticle;
            ptl.Quad.WorldMatrix.X = this.WorldMatrix.X + (endDistMax * (float)Math.Cos(ptl.angle));
            ptl.Quad.WorldMatrix.Z = this.WorldMatrix.Z + (endDistMax * (float)Math.Sin(ptl.angle));
            return particle;
        }

        /// <summary>define how to reset a particle to default states</summary>
        protected override Particle ResetParticle(ref Particle ptl)
        {
            TornadoQuadParticle tptl = ptl as TornadoQuadParticle;
            tptl.Alive = true;
            tptl.CurrentLife = 0;
            tptl.ResetTrackerArray();
            tptl.angle = (float)random.NextDouble() * 360f;
            tptl.dist = MyMath.GetRandomFloat(TornadoGlitterEmitter.startDistMin, TornadoGlitterEmitter.startDistMax, random);
            tptl.speed = MyMath.GetRandomFloat(TornadoGlitterEmitter.startSpeedMin, TornadoGlitterEmitter.startSpeedMax, random);
            tptl.distSpeed = MyMath.GetRandomFloat(TornadoGlitterEmitter.startDSpeedMin, TornadoGlitterEmitter.startDSpeedMax, random);
            tptl.Quad.Green = 255;
            tptl.Quad.Blue = MyMath.GetScalarBetween(TornadoGlitterEmitter.endDistMin, TornadoGlitterEmitter.startDistMax, tptl.dist) * 255;
            tptl.Quad.Red = 9;
            return ptl;
        }

        /// <summary>Last Minute update logic</summary>
        protected override void ParticleUpdate(GameTime time, ref Particle particle)
        {        }

        #endregion API

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
            TornadoGlitterEmitter output = new TornadoGlitterEmitter(content);
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
