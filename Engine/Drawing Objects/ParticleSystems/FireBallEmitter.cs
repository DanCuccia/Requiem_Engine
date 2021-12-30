using System.Collections.Generic;
using Engine.Managers;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
#pragma warning disable 1591

namespace Engine.Drawing_Objects.ParticleSystems
{
    /// <summary>This emitter is used for fireball projectiles</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class FireBallEmitter : ParticleSystem
    {
        ContentManager content;

        #region Init
        /// <summary>default CTOR</summary>
        /// <param name="content">xna content manager</param>
        /// <param name="maxParticles">maximum amount of particles to allocate for</param>
        public FireBallEmitter(ContentManager content, int maxParticles = 20)
            :base(content, maxParticles)
        {
            base.ParticleSystemID = GameIDList.PS_MegaParticle_FireBall;
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
            settings.EmissionRate = 100f;
            settings.FaceCamera = false;
            settings.Gravity = settings.InitialTurbulance = Vector3.Zero;
            settings.InitialVelocity.Y = 1.5f;
            settings.InitialVelocity.Z = 1.5f;
            settings.MaximumLife = 60000f;
            settings.MaximumDistance = 150f;
            settings.ParticleColor = Color.FromNonPremultiplied(255, 252, 175, 255);
            return settings;
        }

        /// <summary>load textures</summary>
        /// <param name="content">xna content manager</param>
        protected override void LoadAssets(ContentManager content)
        {
            this.content = content;
        }

        /// <summary>define what each particle is</summary>
        /// <returns>newly allocated particle</returns>
        protected override Particle AllocateParticle()
        {
            return new MegaParticle(content, InstructionLength);
        }

        /// <summary>load the particle's instruction list</summary>
        /// <returns>a list of particle events, which gets converted to an array</returns>
        protected override List<ParticleEvent> LoadParticleLifeInstructions()
        {
            List<ParticleEvent> output = new List<ParticleEvent>();

            output.Add(new ParticleEvent(100f, ptl_shrink, 50000f));

            return output;
        }

        private void ptl_shrink(ParticleEvent e, GameTime t, Particle p, float d)
        {
            p.WorldMatrix.UniformScale -= .002f;
            if (p.WorldMatrix.UniformScale <= 0f)
                p.Alive = false;
        }

        #endregion Init

        #region API
        /// <summary>Setup the recently reset particle</summary>
        /// <param name="particle">this particle was reset by the system with automated velocity and position</param>
        /// <returns>any edited parameters</returns>
        protected override Particle CreateParticle(ref Particle particle)
        {
            particle.WorldMatrix.UniformScale = MyMath.GetRandomFloat(3f, 10f);
            return particle; 
        }

        /// <summary>optional update</summary>
        protected override void ParticleUpdate(GameTime time, ref Particle particle) 
        {        }
        #endregion

        #region Serialization
        public override XMLMedium GetXML()  { return null; }
        public override Object3D GetCopy(ContentManager content) { return null; }
        public override void CreateFromXML(ContentManager content, XMLMedium inputXml) { }
        #endregion
    }
}
