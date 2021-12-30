using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StillDesign.PhysX;

namespace Requiem.Movement
{
    /// <summary>enemy's who stand still at a distance use this movement interface</summary>
    public sealed class EnemyDistanceMovement : CMovement
    {
        float heightOffset;

        /// <summary>default ctor</summary>
        /// <param name="world">reference to the world matrix</param>
        /// <param name="actor">reference to the physX actor</param>
        public EnemyDistanceMovement(ref WorldMatrix world, ref Actor actor)
            : base(ref world, ref actor)
        {
            heightOffset = (playerActor.Shapes[0] as CapsuleShape).Height;
        }

        /// <summary>update world to actor</summary>
        /// <param name="time">blah</param>
        public override void Update(GameTime time)
        {
            worldMatrix.X = playerActor.GlobalPosition.X;
            worldMatrix.Y = playerActor.GlobalPosition.Y - heightOffset;
            worldMatrix.Z = playerActor.GlobalPosition.Z;
            StillDesign.PhysX.MathPrimitives.Vector3 v = playerActor.LinearVelocity;
            v.X = v.Z = 0f;
            playerActor.LinearVelocity = v;
        }

        /// <summary>currently unused</summary>
        public override void Input(KeyboardState kb, MouseState ms, GamePadState gp) { }

        /// <summary>reset actor to world matrix</summary>
        public override void ForceReset()
        {
            StillDesign.PhysX.MathPrimitives.Vector3 p = playerActor.GlobalPosition;
            p.X = worldMatrix.X;
            p.Y = worldMatrix.Y;
            p.Z = worldMatrix.Z;
            playerActor.GlobalPosition = p;
        }

        /// <summary>Unused</summary>
        public override void AddImpulse(Vector3 direction, float amount) { }


        public override void Input(Keys key) { }
    }
}
