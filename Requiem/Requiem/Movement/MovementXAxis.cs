using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StillDesign.PhysX;
using PVector3 = StillDesign.PhysX.MathPrimitives.Vector3;

namespace Requiem.Movement
{
    /// <summary>This movement is the primary scrolling movement the player uses</summary>
    /// <author>Gabriel Dubois, modified by Daniel Cuccia</author>
    public sealed class MovementXAxis : CMovement
    {
        PVector3 velocity;
        Keys pressedKey = Keys.None;
        Buttons pressedButton = Buttons.B;
        float drag = 2f;
        float speed = 5f;
        public static float MaxSpeed = 90f;
        public static float JumpImpulse = 115f;
        float heightOffset;
        
        /// <summary>default CTOR</summary>
        /// <param name="world">reference to this world</param>
        /// <param name="playerActor">reference to this playerActor</param>
        public MovementXAxis(ref WorldMatrix world, ref Actor playerActor)
            : base(ref world, ref playerActor)
        {
            velocity = playerActor.LinearVelocity;
            heightOffset = (playerActor.Shapes[0] as CapsuleShape).Height;
        }

        /// <summary>main input logic</summary>
        public override void Input(KeyboardState kb, MouseState ms, GamePadState gp)
        {
            velocity = this.playerActor.LinearVelocity;
            this.keyboard(kb);
            this.Gamepad(gp);
            playerActor.LinearVelocity = velocity;
        }

        /// <summary>used by movement behaviors from enemy AI</summary>
        /// <param name="key">a button the AI decided to push</param>
        public override void Input(Keys key)
        {
            if (key == Keys.A)
                velocity.X -= speed;
            if (key == Keys.D)
                velocity.X += speed;

            if (velocity.X > MovementXAxis.MaxSpeed)
                velocity.X = MovementXAxis.MaxSpeed;
            if (velocity.X < -MovementXAxis.MaxSpeed)
                velocity.X = -MovementXAxis.MaxSpeed;

            if ((key == Keys.Space) && (pressedKey != Keys.Space))
            {
                pressedKey = Keys.Space;
                velocity.Y += MovementXAxis.JumpImpulse;
            }
            else if ((key != Keys.Space) && (pressedKey == Keys.Space))
            {
                pressedKey = Keys.None;
            }

            playerActor.LinearVelocity = velocity;
        }

        /// <summary>keyboard -input logic</summary>
        /// <param name="kb">current keyboard state</param>
        private void keyboard(KeyboardState kb)
        {
            if (kb.IsKeyDown(Keys.A))
                velocity.X -= speed;
            if (kb.IsKeyDown(Keys.D))
                velocity.X += speed;
            
            if (velocity.X > MovementXAxis.MaxSpeed)
                velocity.X = MovementXAxis.MaxSpeed;
            if (velocity.X < -MovementXAxis.MaxSpeed)
                velocity.X = -MovementXAxis.MaxSpeed;

            if (kb.IsKeyDown(Keys.Space) && pressedKey != Keys.Space)
            {
                pressedKey = Keys.Space;
                velocity.Y += MovementXAxis.JumpImpulse;
            }
            else if (kb.IsKeyUp(Keys.Space) && pressedKey == Keys.Space)
            {
                pressedKey = Keys.None;
            }
        }

        /// <summary>gamepad -input logic</summary>
        /// <param name="kb">current gamepad state</param>
        private void Gamepad(GamePadState gp)
        {
            if (gp.IsButtonDown(Buttons.DPadLeft))
                velocity.X -= speed;
            if (gp.IsButtonDown(Buttons.DPadRight))
                velocity.X += speed;

            if (gp.ThumbSticks.Left.X < -0.5f)
                velocity.X -= speed;
            if (gp.ThumbSticks.Left.X > 0.5f)
                velocity.X += speed;

            if (velocity.X > MovementXAxis.MaxSpeed)
                velocity.X = MovementXAxis.MaxSpeed;
            if (velocity.X < -MovementXAxis.MaxSpeed)
                velocity.X = -MovementXAxis.MaxSpeed;

            if (gp.IsButtonDown(Buttons.A) && pressedButton != Buttons.A)
            {
                pressedButton = Buttons.A;
                velocity.Y += MovementXAxis.JumpImpulse;
            }
            else if (gp.IsButtonUp(Buttons.A) && pressedButton == Buttons.A)
            {
                pressedButton = Buttons.B;
            }
        }

        /// <summary>update logic</summary>
        public override void Update(GameTime time)
        {
            if (playerActor == null)
                return;
            if (playerActor.IsDisposed == true)
                return;
            if (worldMatrix == null)
                return;
            base.worldMatrix.X = base.playerActor.GlobalPosition.X;
            base.worldMatrix.Y = base.playerActor.GlobalPosition.Y - heightOffset;
            velocity.X = (velocity.X < 0) ? velocity.X + drag : velocity.X - drag;
            velocity.X *= modifier;
        }

        public override void ForceReset()
        {
            PVector3 p = base.playerActor.GlobalPosition;
            p.X = worldMatrix.X; p.Y = worldMatrix.Y; p.Z = worldMatrix.Z;
            base.playerActor.GlobalPosition = p;
        }

        public override void AddImpulse(Vector3 direction, float amount)
        {
            direction.Normalize();
            direction *= amount;

            velocity.X += direction.X;
            velocity.Y += direction.Y;
        }
    }
}
