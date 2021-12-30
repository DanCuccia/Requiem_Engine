using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StillDesign.PhysX;

namespace Requiem.Movement
{
    /// <summary>The Player uses this movement when he dies, in order to smoothly spawn back at the last location</summary>
    public sealed class MoveTo : CMovement
    {
        Vector3 gotoPosition;
        Vector3 direction;
        float speed = 3f;
        float threshold = 5f;

        public delegate void MoveToCallback();
        MoveToCallback callback;

        /// <summary>default ctor</summary>
        /// <param name="world">reference to the worldmatrix</param>
        /// <param name="actor">reference to the physx actor</param>
        /// <param name="desiredPosition">the desired location the worldMatrix will move to</param>
        public MoveTo(ref WorldMatrix world, ref Actor actor, Vector3 desiredPosition, MoveToCallback callback)
            : base(ref world, ref actor)
        {
            this.gotoPosition = desiredPosition;
            this.callback = callback;
            this.direction = desiredPosition - world.Position;
            this.direction.Normalize();
        }

        /// <summary>Un-Used, this movement is automated</summary>
        public override void Input(KeyboardState kb, MouseState ms, GamePadState gp) { }

        /// <summary>main update call</summary>
        public override void Update(GameTime time)
        {
            worldMatrix.Position += (direction * speed);
            if (Vector3.Distance(worldMatrix.Position, gotoPosition) < threshold)
            {
                worldMatrix.Position = gotoPosition;
                worldMatrix.Y += 150f;
                if (callback != null)
                    callback();
            }
        }

        /// <summary>hard force-reset the actor to the worldMatrix (just in case)</summary>
        public override void ForceReset()
        {
            StillDesign.PhysX.MathPrimitives.Vector3 v = this.playerActor.GlobalPosition;
            v.X = worldMatrix.X;
            v.Y = worldMatrix.Y;
            v.Z = worldMatrix.Z;
            this.playerActor.GlobalPosition = v;
        }

        public override void AddImpulse(Vector3 direction, float amount)
        {
            //...
        }

        public override void Input(Keys key) { }
    }
}
