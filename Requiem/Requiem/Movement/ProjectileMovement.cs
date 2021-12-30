using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Requiem.Movement
{
    /// <summary>This movement class is used for projectiles, so no physX actor is required</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class ProjectileMovement : CManualMovement
    {
        Vector3 vector = Vector3.Zero;
        float speed = 5f;

        /// <summary>default ctor, replace default parameters</summary>
        /// <param name="world">reference to the worldMatrix this object effects</param>
        /// <param name="vector">direction of this movement</param>
        /// <param name="speed">speed of the movement</param>
        public ProjectileMovement(ref WorldMatrix world, Vector3 vector, float speed)
            :base(ref world)
        {
            this.vector = vector;
            this.speed = speed;
        }

        /// <summary>Un-used</summary>
        public override void Input(KeyboardState kb, MouseState ms, GamePadState gp) { }

        /// <summary>move the worldMatrix forward by vector and speed</summary>
        public override void Update(GameTime time)
        {
            if(worldMatrix != null)
                base.worldMatrix.Position += vector * (speed * modifier);
        }

        public override void AddImpulse(Vector3 direction, float amount) { }

        public override void Input(Keys key) { }
    }
}
