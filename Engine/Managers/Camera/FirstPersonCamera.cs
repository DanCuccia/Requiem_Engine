using Engine.Managers.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Engine.Math_Physics;

namespace Engine.Managers.Camera
{
    /// <summary>Camera for first-person game mode</summary>
    /// <author>Gabe Dubois</author>
    public sealed class FirstPersonCamera : ICamera
    {
        Engine.Managers.Camera.Camera camera;
        WorldMatrix chase;

        /// <summary>default ctor</summary>
        /// <param name="camera">reference to the camera this behavior affects</param>
        /// <param name="world">reference to a worldMatrix</param>
        public FirstPersonCamera(ref Engine.Managers.Camera.Camera camera, ref WorldMatrix world)
        {
            this.camera = camera;
            this.chase = world;

            camera.X = chase.X;
            camera.Y = chase.Y + 50;
            camera.Z = chase.Z;
            camera.lX = chase.X;
            camera.lY = chase.Y;
            camera.lZ = chase.Z;
        }

        /// <summary>update logic</summary>
        public void Update(GameTime time)
        {
            camera.X = chase.X;
            camera.Y = chase.Y + 50;
            camera.Z = chase.Z;

            camera.RotationMatrix.Forward.Normalize();
            camera.RotationMatrix.Up.Normalize();
            camera.RotationMatrix.Right.Normalize();
            camera.RotationMatrix = Matrix.CreateFromYawPitchRoll(camera.Yaw, camera.Pitch, camera.Roll);

            camera.LookAtTarget = camera.Position + camera.RotationMatrix.Forward;
        }

        /// <summary>unused input logic</summary>
        public void Input(KeyboardState kb, MouseState ms)
        {
            float maxAngle = 45;
            maxAngle = MathHelper.ToRadians(maxAngle);

            camera.Pitch += -(ms.Y - camera.Viewport.Height / 2) * .003f;
            camera.Yaw += -(ms.X - camera.Viewport.Width / 2) * .003f;
            Mouse.SetPosition(camera.Viewport.Width / 2, camera.Viewport.Height / 2);

            GamePadState gp = GamePad.GetState(PlayerIndex.One);

            camera.Pitch += -gp.ThumbSticks.Left.Y * .03f;
            camera.Yaw += -gp.ThumbSticks.Left.X * .03f;
            Mouse.SetPosition(camera.Viewport.Width / 2, camera.Viewport.Height / 2);

            if (camera.Pitch > maxAngle)
            {
                camera.Pitch = maxAngle;
            }
            else if (camera.Pitch < -maxAngle)
            {
                camera.Pitch = -maxAngle;
            }

            if (camera.Yaw > maxAngle)
            {
                camera.Yaw = maxAngle;
            }
            else if (camera.Yaw < -maxAngle)
            {
                camera.Yaw = -maxAngle;
            }
        }
    }
}
