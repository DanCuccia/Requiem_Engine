using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Engine.Managers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Engine.Math_Physics;
#pragma warning disable 1591

namespace Engine.Drawing_Objects.ParticleSystems
{
    /// <summary>quad particle extension to hold extra scaling data</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class VampireQuadParticle : QuadParticle
    {
        public float MyScale = 1f;
        public Vector3 MyStartLocation = Vector3.Zero;

        public VampireQuadParticle(int i, Texture2D t)
            : base(i, t) { }
    }

    /// <summary>emitter used to render the vampire spell</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class VampireEmitter : ParticleSystem
    {
        Texture2D texture;

        const float startScaleMin = 0.9f;
        const float startScaleMax = 2.5f;

        public VampireEmitter(ContentManager content, int maxParticles = 64)
            : base(content, maxParticles)
        {
            base.ParticleSystemID = GameIDList.PS_BillBoardParticle_Vampire;
        }

        protected override void LoadAssets(ContentManager content)
        {
            texture = TextureManager.getInstance().GetTexture("particle//star");
        }

        protected override Particle AllocateParticle()
        {
            return new VampireQuadParticle(base.InstructionLength, texture);
        }

        protected override EmissionSettings LoadEmissionSettings(ref EmissionSettings settings)
        {
            settings.BeginningAlpha = 1f;
            settings.FaceCamera = true;

            settings.UseGravity = false;

            settings.ScaleSpawnMax = 0.4f;
            settings.ScaleSpawnMin = 0.001f;

            settings.ParticleColor = new Color(208, 16, 38);
            settings.SpawnBox = new Vector3(150f, 50f, 150f);

            settings.MaximumLife = 4000f;
            settings.MaximumDistance = 600f;

            settings.EmissionRate = 20f;
            settings.EmissionRateVariance = 50f;

            return settings;
        }

        protected override List<ParticleEvent> LoadParticleLifeInstructions()
        {
            List<ParticleEvent> output = new List<ParticleEvent>();

            output.Add(new ParticleEvent(100f, ptl_shrink, 1000f));
            output.Add(new ParticleEvent(100f, ptl_suck, emissionSettings.MaximumLife));

            return output;
        }

        private void ptl_shrink(ParticleEvent e, GameTime time, Particle ptl, float durationScalar)
        {
            VampireQuadParticle p = ptl as VampireQuadParticle;
            p.WorldMatrix.UniformScale = (1f - durationScalar) * p.MyScale;
            if (p.WorldMatrix.UniformScale <= .0001f)
                p.Alive = false;
        }

        private void ptl_suck(ParticleEvent e, GameTime time, Particle ptl, float durationScalar)
        {
            VampireQuadParticle p = ptl as VampireQuadParticle;
            p.WorldMatrix.Position = Vector3.Lerp(p.MyStartLocation, WorldMatrix.Position, durationScalar);
        }

        protected override Particle CreateParticle(ref Particle particle)
        {
            VampireQuadParticle p = particle as VampireQuadParticle;
            p.MyScale = p.WorldMatrix.UniformScale;
            p.MyStartLocation = p.WorldMatrix.Position;
            p.Color = new Color(208, 16, 38);
            return particle;
        }

        protected override void ParticleUpdate(GameTime time, ref Particle particle) { }

        #region Serialization
        public override XMLMedium GetXML() { return null; }
        public override Object3D GetCopy(ContentManager content) { return null; }
        public override void CreateFromXML(ContentManager content, XMLMedium inputXml)
        { throw new System.NotImplementedException(); }
        #endregion
    }
}
