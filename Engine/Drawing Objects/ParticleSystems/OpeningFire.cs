using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Engine.Managers;
#pragma warning disable 1591

namespace Engine.Drawing_Objects.ParticleSystems
{
    /// <summary>Custom Fire system for the Title Scene (SplashScene3)</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class OpeningFire : ParticleSystem
    {
        ContentManager content;
        private float[] lightingProperties = 
            {   //ambient, diffuse, specular, shine
                  0.2f,     1f,      .1f,     64f
            };

        #region Initialize
        /// <summary>Default CTOR, construction logic governed by base ParticleSystem class</summary>
        /// <param name="maxParticles">maximum amount of particle to aloc for</param>
        /// <param name="content">xna content manager</param>
        public OpeningFire(ContentManager content, int maxParticles = 300)
            : base(content, maxParticles)
        {
            base.ParticleSystemID = GameIDList.PS_MegaParticle_Opening;
        }

        /// <summary>Initialize how this emitter spits out particles</summary>
        /// <param name="settings">edit this settings struct</param>
        /// <returns>return the editing struct</returns>
        protected override EmissionSettings LoadEmissionSettings(ref EmissionSettings settings)
        {
            settings.MaximumLife = 7000f;
            settings.InitialTurbulance = new Vector3(.7f, 0f, .7f);
            settings.InitialVelocityVariance = 1f;
            settings.UseGravity = true;
            settings.FaceCamera = false;
            settings.EmissionRate = 1f;
            settings.Gravity.Y = 0.03f;
            settings.ScaleSpawnMin = 20f;
            settings.ScaleSpawnMax = 30f;
            settings.SpawnBox.X = 400f;
            settings.SpawnBox.Z = 50f;
            settings.SpawnBox.Y = 50f;
            return settings;
        }

        /// <summary>Load textures and stuff</summary>
        protected override void LoadAssets(ContentManager content)
        {
            this.content = content;
        }

        /// <summary>Create whatever type particle needed (mega sphere)</summary>
        /// <returns>Mega Sphere Particle</returns>
        protected override Particle AllocateParticle()
        {
            return new MegaFireParticle(content, base.InstructionLength);
        }

        /// <summary>Load the instructions depicting how a particle lives</summary>
        /// <returns>the list of instructions, it will be sorted and converted to an array</returns>
        protected override List<ParticleEvent> LoadParticleLifeInstructions()
        {
            List<ParticleEvent> output = new List<ParticleEvent>();
            output.Add(new ParticleEvent(0f, ptl_grow, 500f));
            output.Add(new ParticleEvent(emissionSettings.MaximumLife / 4f, ptl_shrink, (emissionSettings.MaximumLife / 4f) * 3f));
            return output;
        }

        private void ptl_grow(ParticleEvent e, GameTime time, Particle particle, float durationScalar)
        {
            MegaFireParticle p = particle as MegaFireParticle;
            p.WorldMatrix.UniformScale = durationScalar * p.MaxScale;
        }
        private void ptl_shrink(ParticleEvent e, GameTime time, Particle particle, float durationScalar)
        {
            particle.WorldMatrix.UniformScale = (particle as MegaFireParticle).MaxScale * (1 - durationScalar);
        }
        #endregion

        #region API

        /// <summary>Setup the recently reset particle</summary>
        /// <param name="particle">this particle was reset by the system with automated velocity and position</param>
        /// <returns>any edited parameters</returns>
        protected override Particle CreateParticle(ref Particle particle)
        {
            MegaFireParticle p = particle as MegaFireParticle;
            //p.Color = Color.Yellow;
            p.Color = Color.Black;
            p.MaxScale = particle.WorldMatrix.UniformScale;
            p.WorldMatrix.UniformScale = 0f;
            return particle;
        }

        /// <summary>Depict how the reset logic works</summary>
        /// <param name="particle">the particle being reset</param>
        /// <returns>the particle back to default values</returns>
        protected override Particle ResetParticle(ref Particle particle)
        {
            base.ResetParticle(ref particle);
            MegaParticle ptl = particle as MegaParticle;
            ptl.MegaSphere.Material.LightingProperties = this.lightingProperties;
            return ptl;
        }

        /// <summary>optional update</summary>
        /// <param name="particle">reference to the particle</param>
        /// <param name="time">current game times</param>
        protected override void ParticleUpdate(GameTime time, ref Particle particle)
        { }

        #endregion API

        public override void CreateFromXML(ContentManager content, XMLMedium inputXml)
        {
            throw new System.NotImplementedException();
        }
        public override Object3D GetCopy(ContentManager content)
        {
            throw new System.NotImplementedException();
        }
        public override XMLMedium GetXML()
        {
            throw new System.NotImplementedException();
        }
        
        #region Mat Sample
        /// <summary>ambience of each megasphere</summary>
        public float Ambient
        {
            set { lightingProperties[0] = value; }
            get { return lightingProperties[0]; }
        }
        /// <summary>Diffuse of each megasphere</summary>
        public float Diffuse
        {
            set { lightingProperties[1] = value; }
            get { return lightingProperties[1]; }
        }
        /// <summary>Specular of each megasphere</summary>
        public float Specular
        {
            set { lightingProperties[2] = value; }
            get { return lightingProperties[2]; }
        }
        /// <summary>Shine of each megasphere</summary>
        public float Shine
        {
            set { lightingProperties[3] = value; }
            get { return lightingProperties[3]; }
        }
        /// <summary>LightingProperties of each megasphere</summary>
        public float[] LightingProperties
        {
            set { this.lightingProperties = value; }
            get { return this.lightingProperties; }
        }
        #endregion
    }
}
