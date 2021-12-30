using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Requiem.Movement
{
    /// <summary>This concrete base movement interface class uses the same logic,
    /// but does not require a PhysX Actor</summary>
    /// <author>Daniel Cuccia</author>
    public abstract class CManualMovement :IMovement
    {
        protected WorldMatrix worldMatrix;
        protected float modifier = 1f;

        public CManualMovement(ref WorldMatrix world)
        {
            this.worldMatrix = world;
        }

        public abstract void Input(KeyboardState kb, MouseState ms, GamePadState gp);
        public abstract void Input(Keys key);
        public abstract void Update(GameTime time);
        public void ForceReset() { }
        public void ModifySpeed(float scalar) { this.modifier = scalar; }
        public abstract void AddImpulse(Vector3 direction, float amount);
        public virtual Vector3 Direction() { return Vector3.Zero; }
    }
}
