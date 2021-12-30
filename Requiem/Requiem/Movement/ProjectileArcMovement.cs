using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Requiem.Movement
{
    /// <summary>this projectile movement class moves the object by speed in an Arc towards the given destination</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class ProjectileArcMovement : CManualMovement
    {
        Curve3D path = new Curve3D();
        double time = 0;

        /// <summary>default Ctor</summary>
        /// <param name="world">reference of the worldMatrix this object affects</param>
        /// <param name="destination">the location this movement class will end up at</param>
        /// <param name="duration">how long in millies the movement takes to get from beginning position to ending position</param>
        public ProjectileArcMovement(ref WorldMatrix world, Vector3 destination, float duration, float height)
            : base(ref world)
        {
            Vector3 vectorTo = destination - world.Position;
            vectorTo.Normalize();
            Vector3 center = world.Position + (vectorTo * (Vector3.Distance(world.Position, destination) * 0.5f));
            center.Y += height;

            path.AddPoint(world.Position, 0);
            path.AddPoint(center, duration * 0.5f);
            path.AddPoint(destination, duration);

            path.SetTangents();
        }

        /// <summary>Un-Used</summary>
        public override void Input(KeyboardState kb, MouseState ms, GamePadState gp) { }

        /// <summary>update the worldMatrix's position in a arcing fashion</summary>
        public override void Update(GameTime time)
        {
            this.time += time.ElapsedGameTime.Milliseconds;
            base.worldMatrix.Position = path.GetPointOnCurve((float)this.time);
        }

        public override void AddImpulse(Vector3 direction, float amount) { }

        public override void Input(Keys key) { }
    }
}
