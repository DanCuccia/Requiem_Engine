using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using StillDesign.PhysX;
#pragma warning disable 0649

namespace Requiem.Movement
{
    /// <summary>
    /// Automated movement behavior
    /// Moves the owner in a specified direction
    /// </summary>
    /// <author> Gabrial Dubois </author>
    class AutomatedDirectionalMovement : CAutomatedMovement
    {
        //private Vector3 direction;
        StillDesign.PhysX.MathPrimitives.Vector3 position;

        public AutomatedDirectionalMovement(ref WorldMatrix world, ref Actor actor, Vector3 direction, float acceleration, float maxVelocity)
            : base(ref world, ref actor)
        {
            this.direction = direction;
            this.acceleration = acceleration;
            this.maxVelocity = maxVelocity;

            this.direction.Normalize();
            this.direction *= acceleration;
        }

        public override void Update(GameTime time)
        {
            base.actor.GlobalPosition = this.position;
            base.worldMatrix.X = base.actor.GlobalPosition.X;
            base.worldMatrix.Y = base.actor.GlobalPosition.Y;
            base.worldMatrix.Z = base.actor.GlobalPosition.Z;
        }

        public override void AddImpulse(Vector3 direction, float amount)
        {
            //...
        }
    }
}
