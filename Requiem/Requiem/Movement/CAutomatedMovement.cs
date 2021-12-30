using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StillDesign.PhysX;

namespace Requiem.Movement 
{
    /// <summary>
    /// first tier concrete automated movement interface,
    /// we wrap the reference world matrix here for 
    /// derived movement interfaces to control
    /// </summary>
    /// <author> Gabrial Dubois </author>
    public abstract class CAutomatedMovement : IMovement
    {
        protected WorldMatrix worldMatrix;
        protected Actor actor;
        protected Vector3 direction;
        protected float acceleration = 23f;
        protected float maxVelocity = 90f;
        protected float speedDamping = 19f;
        protected float jumpImpulse = 115f;
        protected float modifier = 1f;

        /// <summary>initialize base pointers</summary>
        /// <param name="world">reference to the world matrix this movement controls</param>
        /// <param name="playerActor">the PhysX actor object this movment controls</param>
        public CAutomatedMovement(ref WorldMatrix world, ref Actor actor)
        {
            this.worldMatrix = world;
            this.actor = actor;
        }

        public void Input(KeyboardState kb, MouseState ms, GamePadState gp) { }
        public void Input(Keys key) { }
        public abstract void Update(GameTime time);
        public void ForceReset() { }
        public void ModifySpeed(float scalar) { this.modifier = scalar; }
        public abstract void AddImpulse(Vector3 direction, float amount);
        public virtual Vector3 Direction() { return Vector3.Zero; }
    }
}