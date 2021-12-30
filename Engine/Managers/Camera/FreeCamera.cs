using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Managers.Camera
{
    /// <summary>Concrete base class - free camera behavior</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class FreeCamera : ICamera
    {
        Camera camera;

        float cameraSpeed = 4f;
        float sensitivity = .003f;

        /// <summary>default ctor</summary>
        /// <param name="camera">reference to the camera this behavior is effecting</param>
        public FreeCamera(ref Camera camera)
        {
            this.camera = camera;
            Viewport vp = Renderer.getInstance().Device.Viewport;
            Mouse.SetPosition(vp.Width / 2, vp.Height / 2);
        }

        /// <summary>update logic</summary>
        public void Update(GameTime time)
        {
            camera.RotationMatrix.Forward.Normalize();
            camera.RotationMatrix.Up.Normalize();
            camera.RotationMatrix.Right.Normalize();
            camera.RotationMatrix = Matrix.CreateFromYawPitchRoll(camera.Yaw, camera.Pitch, camera.Roll);
            camera.LookAtTarget = camera.Position + camera.RotationMatrix.Forward;
        }

        /// <summary>input logic</summary>
        public void Input(KeyboardState kb, MouseState ms)
        {
            camera.Pitch += -(ms.Y - camera.Viewport.Height * .5f) * sensitivity;
            camera.Yaw += -(ms.X - camera.Viewport.Width * .5f) * sensitivity;
            Mouse.SetPosition((int)(camera.Viewport.Width * .5f), (int)(camera.Viewport.Height * .5f));

            if (kb.IsKeyDown(Keys.W))
            {
                if (kb.IsKeyDown(Keys.Space))
                    camera.MoveCamera(camera.RotationMatrix.Forward, cameraSpeed);
                camera.MoveCamera(camera.RotationMatrix.Forward, cameraSpeed);
            }
            if (kb.IsKeyDown(Keys.S))
            {
                if (kb.IsKeyDown(Keys.Space))
                    camera.MoveCamera(-camera.RotationMatrix.Forward, cameraSpeed);
                camera.MoveCamera(-camera.RotationMatrix.Forward, cameraSpeed);
            }
            if (kb.IsKeyDown(Keys.A))
            {
                if (kb.IsKeyDown(Keys.Space))
                    camera.MoveCamera(-camera.RotationMatrix.Right, cameraSpeed);
                camera.MoveCamera(-camera.RotationMatrix.Right, cameraSpeed);
            }
            if (kb.IsKeyDown(Keys.D))
            {
                if (kb.IsKeyDown(Keys.Space))
                    camera.MoveCamera(camera.RotationMatrix.Right, cameraSpeed);
                camera.MoveCamera(camera.RotationMatrix.Right, cameraSpeed);
            }
            if (kb.IsKeyDown(Keys.E))
            {
                if (kb.IsKeyDown(Keys.Space))
                    camera.MoveCamera(camera.RotationMatrix.Up, cameraSpeed);
                camera.MoveCamera(camera.RotationMatrix.Up, cameraSpeed);
            }
            if (kb.IsKeyDown(Keys.Q))
            {
                if (kb.IsKeyDown(Keys.Space))
                    camera.MoveCamera(-camera.RotationMatrix.Up, cameraSpeed);
                camera.MoveCamera(-camera.RotationMatrix.Up, cameraSpeed);
            }
        }

        /// <summary>multiplier against left/right up/down rotation </summary>
        public float Sensitivity
        {
            get { return this.sensitivity; }
            set { this.sensitivity = value; }
        }
        /// <summary>movement speed of the camera</summary>
        public float Speed
        {
            get { return this.cameraSpeed; }
            set { this.cameraSpeed = value; }
        }
    }
}
