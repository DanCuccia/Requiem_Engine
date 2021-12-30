using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Managers.Camera;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#pragma warning disable 1591

namespace Engine.Drawing_Objects.ParticleSystems
{
    /// <summary>All particle events begin here</summary>
    /// <athor>Daniel Cuccia</athor>
    public class ParticleEvent
    {
        protected float eventTime = 0f;
        protected float duration                   = 0f;
        public delegate void EventCall(ParticleEvent e, GameTime time, Particle particle, float durationScalar);
        protected EventCall eventCall              = null;

        /// <summary>Base Particle Event CTOR</summary>
        /// <param name="eventTime">when during a particles life this will trigger</param>
        /// <param name="callback">delegate to the logic you wish to trigger</param>
        /// <param name="duration">length of the particle event in millies</param>
        public ParticleEvent(float eventTime, EventCall callback, float duration = 0f)
        {
            this.eventTime = eventTime;
            this.eventCall = callback;
            this.duration = duration;
        }

        /// <summary>Call the delegate of this particle event</summary>
        /// <param name="time">currentTime of this particleEvent</param>
        /// <param name="particle">the particle this event is applying to</param>
        /// <param name="durationScalar">0-1 value of where this event is within its duration, return 1 if no duration is involved</param>
        /// <returns>Whether or not this event should be tagged as "done" within the particle</returns>
        public virtual void Execute(GameTime time, Particle particle, float durationScalar)
        {
            if (eventCall != null)
                this.eventCall(this, time, particle, durationScalar);
        }

        /// <summary>Override this to get a copy of this event</summary>
        /// <returns>restarted copy of this particle event</returns>
        public ParticleEvent GetCopy()
        {
            return new ParticleEvent(this.eventTime, this.eventCall, this.duration);
        }

        /// <summary>What is called back when this particle event is triggering</summary>
        public virtual EventCall Callback
        {
            set { this.eventCall = value; }
            get { return this.eventCall; }
        }
        /// <summary>When the particle event triggers within a particles lifetime</summary>
        public virtual float EventTime
        {
            get { return this.eventTime; }
        }
        public virtual float Duration
        {
            get { return this.duration; }
            set { this.duration = value; }
        }
    }

    /// <summary>Contains all variables needed to control emission</summary>
    [Serializable]
    public sealed class EmissionSettings
    {
        /// <summary>within how many milliseconds the spawn rate will vary</summary>
        public float EmissionRateVariance = 10f;

        /// <summary>how many millies in between spawns</summary>
        public float EmissionRate = 100f;

        /// <summary>Maximum milliseconds a particle can live for</summary>
        public float MaximumLife = 2000f;

        /// <summary>gravity will or will not apply to the particle</summary>
        public bool UseGravity = false;

        /// <summary>Gravity value is editable per emitter</summary>
        public Vector3 Gravity = new Vector3(0, -0.15f, 0);

        /// <summary>how much initial turbulance to apply, 0 to 1</summary>
        public float InitialVelocityVariance = 0f;

        /// <summary>how much variance of the initialVelocity</summary>
        public Vector3 InitialTurbulance = Vector3.Zero;

        /// <summary>starting velocity of a particle</summary>
        public Vector3 InitialVelocity = new Vector3(0, 2f, 0);

        /// <summary>This depicts half of each dimension</summary>
        public Vector3 SpawnBox = new Vector3(2f);

        /// <summary>The maximum distance from emitter allowed</summary>
        public float MaximumDistance = 1000f;

        /// <summary>flag the particle system to automate  rotation, you must call UpdateCameraPosition(v3)</summary>
        public bool FaceCamera = false;

        /// <summary>When set true, the system will spawn particles until max is met, and never kill particles</summary>
        public bool StayAlive = false;

        /// <summary>particle size is scaled randomly between min and max</summary>
        public float ScaleSpawnMin = 1f;

        /// <summary>particle size is scaled randomly between min and max</summary>
        public float ScaleSpawnMax = 1f;

        /// <summary>the alpha state when the particle is reset, 0 - 1</summary>
        public float BeginningAlpha = 1f;

        /// <summary>The Default Color applied to particles</summary>
        public Color ParticleColor = Color.White;

        #region Min/Max
        public const float ScaleMin = 1f;
        public const float ScaleMax = 30f;
        public const float GravityMin = -.5f;
        public const float GravityMax = .5f;
        public const float VelocityVarianceMin = 0f;
        public const float VelocityVarianceMax = 1f;
        public const float TurbulenceMin = -1f;
        public const float TurbulenceMax = 1f;
        public const float SpawnBoxMin = 5f;
        public const float SpawnBoxMax = 20000f;
        public const float MaxLifeMin = 500f;
        public const float MaxLifeMax = 5000f;
        public const float MaxDistMin = 20f;
        public const float MaxDistMax = 2000f;
        public const float EmissionRateMin = 2000f;
        public const float EmissionRateMax = 0f;
        #endregion

        /// <summary>Returns a copy of this structure without any references to original</summary>
        /// <returns>copy of this emission setting object</returns>
        public EmissionSettings GetCopy()
        {
            EmissionSettings output = new EmissionSettings();
            output.BeginningAlpha = this.BeginningAlpha;
            output.EmissionRate = this.EmissionRate;
            output.EmissionRateVariance = this.EmissionRateVariance;
            output.FaceCamera = this.FaceCamera;
            output.Gravity = this.Gravity;
            output.InitialTurbulance = this.InitialTurbulance;
            output.InitialVelocity = this.InitialVelocity;
            output.InitialVelocityVariance = this.InitialVelocityVariance;
            output.MaximumDistance = this.MaximumDistance;
            output.MaximumLife = this.MaximumLife;
            output.ParticleColor = this.ParticleColor;
            output.ScaleSpawnMax = this.ScaleSpawnMax;
            output.ScaleSpawnMin = this.ScaleSpawnMin;
            output.SpawnBox = this.SpawnBox;
            output.StayAlive = this.StayAlive;
            output.UseGravity = this.UseGravity;
            return output;
        }
    }

    /// <summary>Custom xml object containing all parameters needed to replicate this system</summary>
    [Serializable]
    public class ParticleSystemXML : XMLMedium
    {
        /// <summary>emission settings xml</summary>
        public EmissionSettings settings = null;
        /// <summary>location of the emitter</summary>
        public WorldMatrixXml worldMatrix = null;
        /// <summary>id of the particle emitter</summary>
        public int id = -1;
    }

    // -------------------------------------------------------------------------------------------
    //              PARTICLE SYSTEM
    // -------------------------------------------------------------------------------------------
    /// <summary>This is the base class which all particle systems inherits and manipulates,
    /// notice most automated logic is encapsulated in here, and even out of reach to the derived classes</summary>
    /// <author>Daniel Cuccia</author>
    public abstract class ParticleSystem : Object3D
    {
        #region Member Vars

        public int ParticleSystemID { set; get; }

        private Particle[] particleArray;
        protected Particle[] ParticleArray
        { get { return particleArray; } }
        private ParticleEvent[] particleLifeInstruction;

        public EmissionSettings emissionSettings = null;
        private float emitCounter = 0f;

        protected Random random = new Random();

        #endregion Member Vars

        #region Init
        /// <summary>Default CTOR</summary>
        /// <param name="maxParticles">maximum amount of particles to allocate for</param>
        /// <param name="content">xna content manager</param>
        public ParticleSystem(ContentManager content, int maxParticles)
        {
            this.ParticleSystemID = -1;
            this.emissionSettings = new EmissionSettings();
            this.emissionSettings = this.LoadEmissionSettings(ref emissionSettings);
            this.loadInstructionArray();
            this.LoadAssets(content);

            this.particleArray = new Particle[maxParticles];
            for (int i = 0; i < particleArray.Length; i++)
            {
                this.particleArray[i] = this.AllocateParticle();
            }
        }

        /// <summary>This gets overriden so the derived class can load any assets to the quads</summary>
        /// <returns>newly allocated particle</returns>
        protected abstract Particle AllocateParticle();

        /// <summary>this is called during base class construction so, derived class can get any assets ready</summary>
        /// <param name="content"></param>
        protected abstract void LoadAssets(ContentManager content);

        /// <summary>This wraps the call to derived load instruction func, 
        /// then orders them by who is executed first</summary>
        private void loadInstructionArray()
        {
            List<ParticleEvent> instructions = this.LoadParticleLifeInstructions();
            this.particleLifeInstruction = instructions.OrderBy(x => x.EventTime).ToArray();
        }

        /// <summary>re-load the instruction list (used in level editor)</summary>
        public void ReInitializeInstructions()
        {
            this.loadInstructionArray();
        }

        /// <summary>This gets overriden by each particular particle emitter derived from this</summary>
        /// <param name="settings">the newly created settings this emitter will set to</param>
        /// <returns>the customly fixed up emission settings</returns>
        protected abstract EmissionSettings LoadEmissionSettings(ref EmissionSettings settings);

        /// <summary>This gets overriden to load the particle life instruction list per derived particle emitter</summary>
        protected abstract List<ParticleEvent> LoadParticleLifeInstructions();

        #endregion Init

        #region API

        /// <summary>Major update call - does not update if frustum culled</summary>
        public override void Update(ref Camera camera, GameTime time)
        {
            base.boundingSphere.Center.X = WorldMatrix.X;
            base.boundingSphere.Center.Y = WorldMatrix.Y;
            base.boundingSphere.Center.Z = WorldMatrix.Z;
            base.boundingSphere.Radius = this.emissionSettings.MaximumDistance;
            base.CullObject(ref camera);
            if (inFrustum == false)
                return;

            this.determineSpawn(time);

            Color col = particleArray[0].Color;

            for(int i = 0; i < particleArray.Length; i++)
            {
                if (particleArray[i].Alive == true)
                {
                    //update particle's life
                    particleArray[i].CurrentLife += time.ElapsedGameTime.Milliseconds;

                    //run instruction logic generated by derived class
                    this.beginInstructionAutomation(time, ref particleArray[i] );

                    //check and apply gravity
                    if (this.emissionSettings.UseGravity == true)
                        particleArray[i].Velocity += emissionSettings.Gravity;

                    //check and rotate to camera
                    if(this.emissionSettings.FaceCamera == true)
                        if(camera != null)
                            particleArray[i].WorldMatrix.rY = 180 + MathHelper.ToDegrees((float)Math.Atan2(particleArray[i].WorldMatrix.X - camera.Position.X,
                                particleArray[i].WorldMatrix.Z - camera.Position.Z));

                    //apply velocity
                    particleArray[i].WorldMatrix.Position += particleArray[i].Velocity;

                    //user-derived "last-minute" update (optional logic)
                    this.ParticleUpdate( time, ref particleArray[i] );

                    if (emissionSettings.StayAlive == false)
                    {
                        //check maximum life
                        if (particleArray[i].CurrentLife >= emissionSettings.MaximumLife)
                            particleArray[i].Alive = false;

                        //check distance
                        if (Vector3.Distance(particleArray[i].WorldMatrix.Position, this.WorldMatrix.Position) > emissionSettings.MaximumDistance)
                            particleArray[i].Alive = false;
                    }
                }
            }
        }

        /// <summary>This is the entry point of the automated instruction logic</summary>
        /// <param name="ptl">the current particle running automation</param>
        /// <param name="time">current game time</param>
        private void beginInstructionAutomation(GameTime time, ref Particle ptl)
        {
            for (int i = 0; i < particleLifeInstruction.Length; i++)
            {
                if (ptl.CurrentLife >= particleLifeInstruction[i].EventTime && ptl[i] == false)
                {
                    float dScalar = (ptl.CurrentLife - particleLifeInstruction[i].EventTime) / particleLifeInstruction[i].Duration;
                    particleLifeInstruction[i].Execute(time, ptl, float.IsInfinity(dScalar) ? 1f : (dScalar > 1 ? 1f : dScalar));
                    if (ptl.CurrentLife > particleLifeInstruction[i].EventTime + particleLifeInstruction[i].Duration)
                        ptl[i] = true;
                }
            }
        }

        /// <summary>This gets overriden and called for every particle, note: this logic
        /// is for any special case testing you need to do outside of the instruction list,
        /// this is called AFTER the instruction logic has run its routine</summary>
        /// <param name="particle">the particle currently getting updated</param>
        /// <param name="time">current timing values</param>
        protected abstract void ParticleUpdate(GameTime time, ref Particle particle);

        /// <summary>spawning is completely automated by the base class after emissionSettings has been set</summary>
        private void determineSpawn(GameTime time)
        {
            emitCounter += time.ElapsedGameTime.Milliseconds;
            if (emitCounter >= this.emissionSettings.EmissionRate)
            {
                emitCounter -= this.emissionSettings.EmissionRate;
                SpawnParticle();
            }
        }

        /// <summary>called by this system to spawn a particle, call the user-overriden function to edit it,
        /// using the first dead particle found in the array</summary>
        protected void SpawnParticle()
        {
            for (int i = 0; i < this.particleArray.Length; i++)
            {
                if (particleArray[i].Alive == false)
                {
                    //reset the particle, and get the pointer
                    Particle particle = this.ResetParticle(ref particleArray[i]);

                    //set velocity, sending the v3 of the particle
                    particle.Velocity = this.InitStartVelocity(particle.Velocity);

                    //set position, sending the v3 of the particle
                    particle.WorldMatrix.Position = this.InitStartPosition(particle.WorldMatrix.Position);

                    //allow derived class to make modifications
                    particle = this.CreateParticle(ref particle);
                    
                    return;
                }
            }
        }

        /// <summary>Initialize initial velocity settings, allowed to be overriden by derived emitter</summary>
        /// <param name="v">starting velocity reference</param>
        /// <returns>modified v3 velocity</returns>
        protected Vector3 InitStartVelocity(Vector3 v)
        {
            float r = (float)this.random.NextDouble() * 2 - 1;
            v.X = r * this.emissionSettings.InitialVelocityVariance * this.emissionSettings.InitialTurbulance.X;
            r = (float)this.random.NextDouble() * 2 - 1;
            v.Z = r * this.emissionSettings.InitialVelocityVariance * this.emissionSettings.InitialTurbulance.Z;
            r = (float)this.random.NextDouble() * 2 - 1;
            v.Y = r * this.emissionSettings.InitialVelocityVariance * this.emissionSettings.InitialTurbulance.Y;
             return v;
        }

        /// <summary>Initialize initial position randomly within spawnbox, allowed to be overriden by derived emitter</summary>
        /// <param name="v">starting position reference</param>
        /// <returns>modified v3 velocity</returns>
        protected Vector3 InitStartPosition(Vector3 v)
        {
            v.X = this.WorldMatrix.X + (-emissionSettings.SpawnBox.X + (emissionSettings.SpawnBox.X * 2 * (float)random.NextDouble()));
            v.Y = this.WorldMatrix.Y + (-emissionSettings.SpawnBox.Y + (emissionSettings.SpawnBox.Y * 2 * (float)random.NextDouble()));
            v.Z = this.WorldMatrix.Z + (-emissionSettings.SpawnBox.Z + (emissionSettings.SpawnBox.Z * 2 * (float)random.NextDouble()));
            return v;
        }

        /// <summary>This gets overriden by the derived particle emitters</summary>
        /// <param name="particle">newly created particle, do not re-allocate! only re-init() and adjust parameters</param>
        /// <returns>customly fixed-up particle</returns>
        protected abstract Particle CreateParticle(ref Particle particle);

        /// <summary>Main Render all Particles call</summary>
        public override void Render()
        {
            if (inFrustum == true)
            {
                foreach (Particle ptl in particleArray)
                {
                    if (ptl.Alive == true)
                        ptl.Render();
                }
            }
        }

        /// <summary>Derived states must define how to default their particles</summary>
        /// <param name="particle">the particle to be reset</param>
        /// <returns>a particle in its default state</returns>
        protected virtual Particle ResetParticle(ref Particle particle)
        {
            particle.Alive = true;
            particle.CurrentLife = 0;
            particle.ResetTrackerArray();
            particle.WorldMatrix.UniformScale = MyMath.GetRandomFloat(emissionSettings.ScaleSpawnMin, emissionSettings.ScaleSpawnMax);
            particle.Alpha = emissionSettings.BeginningAlpha;
            return particle;
        }

        /// <summary>Generate the emitter's bounding box using it's spawn box</summary>
        public override void GenerateBoundingBox()
        {
            this.OBB = new OrientedBoundingBox(-this.emissionSettings.SpawnBox, this.emissionSettings.SpawnBox);
        }

        /// <summary>Update the OBB with this emitter's world matrix</summary>
        public override void UpdateBoundingBox()
        {
            if (OBB != null)
                OBB.Update(this.WorldMatrix.GetWorldMatrix());
        }

        public override void RenderDebug() 
        {
            if (OBB != null)
                OBB.Render();
        }
        public override void RenderImplicit(Effect effect) { }

        public int InstructionLength
        {
            get 
            { 
                if (particleLifeInstruction != null)
                    return particleLifeInstruction.Length; 
                else return -1; 
            }
        }

        public int LivingParticleCount
        {
            get
            {
                int output = 0;
                foreach (Particle ptl in particleArray)
                    if (ptl.Alive == true)
                        output++;
                return output;
            }
        }


        #endregion API

        #region Generalization Vars for Editing
        public Color ParticleColor
        {
            get { return this.particleArray[0].Color; }
            set
            {
                foreach (Particle ptl in particleArray)
                    ptl.Color = value;
            }
        }
        #endregion Generalization Vars for Editing
    }
}
