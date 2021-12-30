using Microsoft.Xna.Framework;
using Engine.Math_Physics;

namespace Engine.Drawing_Objects.ParticleSystems
{
    /// <summary>Base abstract class, this holds all values except the drawing object</summary>
    /// <author>Daniel Cuccia</author>
    public abstract class Particle
    {
        /// <summary>individual particle velocity</summary>
        protected Vector3 velocity = Vector3.Zero;
        /// <summary>whether this particle is alive or dead</summary>
        protected bool alive = false;
        /// <summary>total life in millies</summary>
        protected int currentLife = 0;
        /// <summary>array of bools saying "I've completed this instruction"</summary>
        protected bool[] instructionTracker;

        /// <summary>override this to the particle's drawable</summary>
        public virtual WorldMatrix WorldMatrix { set; get; }
        /// <summary>override this to the particle's drawable</summary>
        public virtual Color Color { set; get; }
        /// <summary>override this to the particle's drawable</summary>
        public virtual float Alpha { set; get; }

        /// <summary>Render call to override</summary>
        public abstract void Render();

        /// <summary>default ctor</summary>
        /// <param name="instructionCount">how many instructions the particle allocates for</param>
        public Particle(int instructionCount)
        {
            instructionTracker = new bool[instructionCount];
        }

        /// <summary>velocity component</summary>
        public virtual Vector3 Velocity
        {
            get { return this.velocity; }
            set { this.velocity = value; }
        }
        /// <summary>velocity-Y component</summary>
        public virtual float VelocityY
        {
            get { return this.Velocity.Y; }
            set { this.velocity.Y = value; }
        }
        /// <summary>velocity-X component</summary>
        public virtual float VelocityX
        {
            get { return this.Velocity.X; }
            set { this.velocity.X = value; }
        }
        /// <summary>velocity-Z component</summary>
        public virtual float VelocityZ
        {
            get { return this.Velocity.Z; }
            set { this.velocity.Z = value; }
        }
        /// <summary>Alive or Dead</summary>
        public virtual bool Alive
        {
            get { return this.alive; }
            set { this.alive = value; }
        }
        /// <summary>current life in millies</summary>
        public virtual int CurrentLife
        {
            get { return this.currentLife; }
            set { this.currentLife = value; }
        }
        /// <summary>index directly into the particle's instructionTracker</summary>
        public bool this[int x]
        {
            get { return this.instructionTracker[x]; }
            set { this.instructionTracker[x] = value; }
        }
        /// <summary>Reset the instruction tracker array</summary>
        public void ResetTrackerArray()
        {
            for (int i = 0; i < instructionTracker.Length; i++)
                instructionTracker[i] = false;
        }
    }
}
