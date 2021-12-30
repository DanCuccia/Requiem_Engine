using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Managers.Camera
{
    /// <summary>Automated Camera Behavior</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class AutomatedCamera : ICamera
    {
        Camera camera;
        Vector3 positionGoto;
        Vector3 targetGoto;
        float stepSpeed = 0.15f;
        float automationCompleteThreshold = .1f;

        /// <summary>delegate called when automation completes</summary>
        public delegate void AutomationCompleteCallback();
        AutomationCompleteCallback callback;

        /// <summary>default ctor</summary>
        /// <param name="camera">reference to the camera this behavior is affecting</param>
        /// <param name="gotoPosition">the desired location of this automation</param>
        /// <param name="lookAtPosition">the desired lookat location of this automation</param>
        /// <param name="callback">delegate function used to callback after the automation is complete</param>
        /// <param name="stepSpeed">the speed the camera will smoothstep to the desired locations</param>
        /// <param name="threshold">within what distance of the desired go-to position you want to activate 'complete' logic</param>
        public AutomatedCamera(ref Camera camera, 
            Vector3 gotoPosition,
            Vector3 lookAtPosition,
            AutomationCompleteCallback callback,
            float stepSpeed = 0.15f, 
            float threshold = 0.1f)
        {
            this.camera = camera;
            this.positionGoto = gotoPosition;
            this.targetGoto = lookAtPosition;
            this.stepSpeed = stepSpeed;
            this.automationCompleteThreshold = threshold;
            this.callback = callback;
        }

        /// <summary>update logic</summary>
        public void Update(GameTime time)
        {
            camera.RotationMatrix.Forward.Normalize();
            camera.RotationMatrix = Matrix.CreateFromAxisAngle(camera.RotationMatrix.Forward, camera.Roll);

            camera.Position = Vector3.SmoothStep(camera.Position, positionGoto, stepSpeed);
            camera.LookAtTarget = Vector3.SmoothStep(camera.LookAtTarget, targetGoto, stepSpeed);

            camera.Yaw = MathHelper.SmoothStep(camera.Yaw, 0f, 0.1f);
            camera.Pitch = MathHelper.SmoothStep(camera.Pitch, 0f, 0.1f);

            if (Vector3.Distance(camera.Position, positionGoto) <= automationCompleteThreshold &&
                Vector3.Distance(camera.LookAtTarget, targetGoto) <= automationCompleteThreshold)
            {
                camera.Position = positionGoto;
                camera.LookAtTarget = targetGoto;
				camera.Behavior = new StationaryCamera();
                if (callback != null)
                {
                    callback();
                }
            }
        }

        /// <summary>unused input logic</summary>
        public void Input(KeyboardState kb, MouseState ms)
        { }
    }
}
