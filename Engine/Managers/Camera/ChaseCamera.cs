using System;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Managers.Camera
{
    /// <summary>Chasing Camera Behavior</summary>
    public sealed class ChaseCamera : ICamera
    {
        Camera camera;
        WorldMatrix chase;

        Vector3 targetOffset = new Vector3(20, 70, 0);
        float distanceOffset;
        float chaseAngle;
        float cameraYOffset;
        Point lastMSPosition;
        int wheelValue;
        Vector2 yOffsetClamps = new Vector2(10, 500);
        Vector2 distanceClamps = new Vector2(350, 1000);

        /// <summary>default ctor</summary>
        /// <param name="camera">reference to the camera this behavior is affecting</param>
        /// <param name="chasedObject">reference to the worldmatrix of the object this chases</param>
        /// <param name="targetOffset">offset of the look at target</param>
        public ChaseCamera(ref Camera camera, ref WorldMatrix chasedObject, Vector3 targetOffset)
        {
            this.camera = camera;
            this.chase = chasedObject;
            wheelValue = Mouse.GetState().ScrollWheelValue;
            this.targetOffset = targetOffset;
        }

        /// <summary>update logic</summary>
        public void Update(GameTime time)
        {
            camera.lX = chase.Position.X + targetOffset.X;
            camera.lY = chase.Position.Y + targetOffset.Y;
            camera.lZ = chase.Position.Z + targetOffset.Z;

            camera.X = chase.X + (distanceOffset * (float)Math.Sin(MathHelper.ToRadians(chaseAngle)));
            camera.Z = chase.Z + (distanceOffset * (float)Math.Cos(MathHelper.ToRadians(chaseAngle)));
            camera.Y = chase.Y + cameraYOffset;
        }

        /// <summary>input logic</summary>
        public void Input(KeyboardState kb, MouseState ms)
        {
            if (kb.IsKeyDown(Keys.OemPeriod) || kb.IsKeyDown(Keys.NumPad6))
                chaseAngle += 2f;
            if (kb.IsKeyDown(Keys.OemComma) || kb.IsKeyDown(Keys.NumPad4))
                chaseAngle -= 2f;

            if (kb.IsKeyDown(Keys.I) || kb.IsKeyDown(Keys.NumPad8))
            {
                cameraYOffset += 2f;
                cameraYOffset = MathHelper.Clamp(cameraYOffset, yOffsetClamps.X, yOffsetClamps.Y);
            }
            if (kb.IsKeyDown(Keys.K) || kb.IsKeyDown(Keys.NumPad2))
            {
                cameraYOffset -= 2f;
                cameraYOffset = MathHelper.Clamp(cameraYOffset, yOffsetClamps.X, yOffsetClamps.Y);
            }

            if (ms.MiddleButton == ButtonState.Pressed)
            {
                chaseAngle += -(ms.X - lastMSPosition.X) * .3f;
                cameraYOffset += -(ms.Y - lastMSPosition.Y) * .7f;

                cameraYOffset = MathHelper.Clamp(cameraYOffset, yOffsetClamps.X, yOffsetClamps.Y);

                lastMSPosition = new Point(ms.X, ms.Y);
                Mouse.SetPosition(lastMSPosition.X, lastMSPosition.Y);
            }

            if (ms.ScrollWheelValue > wheelValue)
            {
                distanceOffset -= 20f;
                distanceOffset = MathHelper.Clamp(distanceOffset, distanceClamps.X, distanceClamps.Y);
                wheelValue = ms.ScrollWheelValue;
            }
            if (ms.ScrollWheelValue < wheelValue)
            {
                distanceOffset += 20f;
                distanceOffset = MathHelper.Clamp(distanceOffset, distanceClamps.X, distanceClamps.Y);
                wheelValue = ms.ScrollWheelValue;
            }

            lastMSPosition.X = ms.X;
            lastMSPosition.Y = ms.Y;
        }
    }
}
