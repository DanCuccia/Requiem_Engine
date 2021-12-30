using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Engine.Managers;

namespace Engine.Drawing_Objects.ParticleSystems
{
    /// <summary>Controls Mega-Particles to create a volumetric smoke</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class SmokeEmitter : ParticleSystem
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
        public SmokeEmitter(ContentManager content, int maxParticles = 90)
            : base(content, maxParticles)
        {
            base.ParticleSystemID = GameIDList.PS_MegaParticle_Smoke;
        }

        /// <summary>Initialize how this emitter spits out particles</summary>
        /// <param name="settings">edit this settings struct</param>
        /// <returns>return the editing struct</returns>
        protected override EmissionSettings LoadEmissionSettings(ref EmissionSettings settings)
        {
            settings.MaximumLife = 4000;
            settings.InitialTurbulance = new Vector3(.7f, 0f, .7f);
            settings.InitialVelocityVariance = 1f;
            settings.UseGravity = settings.FaceCamera = true;
            settings.EmissionRate = 45;
            settings.Gravity.Y = .0f;
            settings.ScaleSpawnMin = 10f;
            settings.ScaleSpawnMax = 22f;
            settings.SpawnBox.X = settings.SpawnBox.Z = 50;
            settings.SpawnBox.Y = 20f;
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
            return new MegaParticle(content, base.InstructionLength);
        }

        /// <summary>Load the instructions depicting how a particle lives</summary>
        /// <returns>the list of instructions, it will be sorted and converted to an array</returns>
        protected override List<ParticleEvent> LoadParticleLifeInstructions()
        {
            List<ParticleEvent> output = new List<ParticleEvent>();
            output.Add(new ParticleEvent(emissionSettings.MaximumLife - (emissionSettings.MaximumLife / 5), ptl_fadeOut, emissionSettings.MaximumLife / 5));
            output.Add(new ParticleEvent(emissionSettings.MaximumLife / 2, ptl_bellow, emissionSettings.MaximumLife / 2));
            return output;
        }

        #region life instructions
        private void ptl_fadeOut(ParticleEvent e, GameTime time, Particle particle, float durationScalar)
        {
            particle.Alpha = (1-durationScalar);
        }
        private void ptl_bellow(ParticleEvent e, GameTime time, Particle particle, float durationScalar)
        {
            particle.VelocityY -= .005f;
        }
        #endregion

        #endregion Initialize

        #region API

        /// <summary>Setup the recently reset particle</summary>
        /// <param name="particle">this particle was reset by the system with automated velocity and position</param>
        /// <returns>any edited parameters</returns>
        protected override Particle CreateParticle(ref Particle particle)
        {
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

        #region Serialization & Editing

        /// <summary>get xml object</summary>
        /// <returns>this xml object</returns>
        public override XMLMedium GetXML()
        {
            ParticleSystemXML output = new ParticleSystemXML();
            output.id = base.ParticleSystemID;
            output.settings = this.emissionSettings;
            output.worldMatrix = this.WorldMatrix.GetXml();
            return output;
        }
        /// <summary>get a copy of this object used in level editor</summary>
        /// <param name="content">xna content manager</param>
        /// <returns>copy of this drawable</returns>
        public override Object3D GetCopy(ContentManager content)
        {
            SmokeEmitter output = new SmokeEmitter(content);
            output.emissionSettings = this.emissionSettings.GetCopy();
            output.WorldMatrix = this.WorldMatrix.Clone();
            return output;
        }
        /// <summary>remake this object from xml</summary>
        /// <param name="content">xna content manager</param>
        /// <param name="inputXml">asset from xml</param>
        public override void CreateFromXML(ContentManager content, XMLMedium inputXml)
        {
            ParticleSystemXML input = (ParticleSystemXML)inputXml;
            this.emissionSettings = input.settings;
            this.WorldMatrix.FromXML(input.worldMatrix);
            this.ReInitializeInstructions();
        }

        #endregion Serialization

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
