using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StillDesign.PhysX;

namespace Requiem.Movement
{
    /// <summary>first tier concrete movement interface,
    /// we wrap the reference world matrix here for derived movement interfaces to control</summary>
    /// <author>Daniel Cuccia</author>
    public abstract class CMovement : IMovement
    {
        protected WorldMatrix worldMatrix;
        protected Actor playerActor;
        protected float modifier = 1;

        /// <summary>initialize base pointers</summary>
        /// <param name="world">reference to the world matrix this movement controls</param>
        /// <param name="playerActor">the PhysX actor object this movment controls</param>
        public CMovement(ref WorldMatrix world, ref Actor playerActor)
        {
            this.worldMatrix = world;
            this.playerActor = playerActor;
            StillDesign.PhysX.MathPrimitives.Vector3 p = this.playerActor.GlobalPosition;
            p.X = world.X; p.Y = world.Y; p.Z = world.Z;
            playerActor.GlobalPosition = p;
        }

        public abstract void Input(KeyboardState kb, MouseState ms, GamePadState gp);
        public abstract void Input(Keys key);
        public abstract void Update(GameTime time);
        public abstract void ForceReset();
        public void ModifySpeed(float scalar) { this.modifier = scalar; }
        public abstract void AddImpulse(Vector3 direction, float amount);
        public virtual Vector3 Direction() { return Vector3.Zero; }
    }
}
