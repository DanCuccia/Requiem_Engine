using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#pragma warning disable 1591

namespace Engine.Managers.Camera
{
    /// <summary>Holds all camera type information</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class Camera
    {
        #region Member Variables

        ICamera             behavior;
        Matrix              view                        = Matrix.Identity;
        Matrix              viewInverse;
        Matrix              projection;
        BoundingFrustum     boundingFrustum;
        
        Viewport            viewport;
        Vector3             position                    = Vector3.Zero;
        Vector3             lookAtTarget                = Vector3.Zero;
        Matrix              camRotation                 = Matrix.Identity;
        float               yaw, pitch, roll            = 0f;

        float               aspectRatio;
        float               nearPlaneDist               = 1f;
        float               farPlaneDist                = 100000f;

        float               shakeMagnitude              = 2f;
        Vector3             shakeOffset                 = Vector3.Zero;
        float               shakeTimer                  = 0f;
        float               shakeDuration               = 0f;
        bool                shakeRamp                   = true;

        #endregion Member Variables

        #region Initialization

        /// <summary>Default Contructor</summary>
        public Camera(Viewport viewPort)
        {
            viewport = viewPort;
            this.aspectRatio = viewport.AspectRatio;
        }

        /// <summary>Experimental - new initialize method</summary>
        /// <param name="cameraBehavior">what behavior this camera uses</param>
        public void Initialize(ICamera cameraBehavior)
        {
            this.behavior = cameraBehavior;
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, nearPlaneDist, farPlaneDist);
            boundingFrustum = new BoundingFrustum(view * projection);
        }

        #endregion

        #region API

        /// <summary>main behavior input call</summary>
        /// <param name="kb">keyboard state</param>
        /// <param name="ms">mouse state</param>
        public void Input(KeyboardState kb, MouseState ms)
        {
            behavior.Input(kb, ms);
        }

        /// <summary> Moves the position of the camera, by a varible m_speed</summary>
        /// <param name="vector">the direction in which to move</param>
        /// <param name="speed">distance moved</param>
        public void MoveCamera(Vector3 vector, float speed = 4f)
        {
            position += speed * vector;
        }

        /// <summary>automate the camera to a position</summary>
        /// <remarks>DEPRECIATED, assign your own AutomatedCamera behavior instead, look inside this function to see how</remarks>
        /// <param name="targetPosition">goto position of the camera target</param>
        /// <param name="cameraPosition">goto position of the camera</param>
        /// <param name="callback">call this when the automation is complete</param>
        /// <param name="stepSpeed">camera speed to new location</param>
        /// <param name="threshold">threshold box, when camera gets within here, callback is executed</param>
        public void SmoothStepTo(Vector3 targetPosition, Vector3 cameraPosition, AutomatedCamera.AutomationCompleteCallback callback, float stepSpeed = 0.15f, float threshold = 0.1f)
        {
            Camera cam = this;
            this.behavior = new AutomatedCamera(ref cam, cameraPosition, targetPosition, callback, stepSpeed, threshold);
        }

        /// <summary>The main update function to be used once before drawing a frame
        /// Updates the view and projection matrices, will update according
        /// CameraType enum variable</summary>
        /// <param name="time">current game time</param>
        public void Update(GameTime time)
        {
            behavior.Update(time);
            this.processShake(time);

            view = Matrix.CreateLookAt(position + shakeOffset, lookAtTarget + shakeOffset, Vector3.Up);
            viewInverse = Matrix.Invert(view);
            boundingFrustum.Matrix = view * projection;
        }

        /// <summary>process the camera's shaking functionality</summary>
        private void processShake(GameTime time)
        {
            if (shakeTimer == -1)
                return;
            shakeTimer += time.ElapsedGameTime.Milliseconds;
            if (shakeTimer >= shakeDuration)
            {
                shakeTimer = -1;
                shakeOffset = Vector3.Zero;
                return;
            }

            float s = shakeTimer / shakeDuration;
            float mag = this.shakeRamp ? shakeMagnitude * (1f - s * s) : shakeMagnitude;
            shakeOffset.X = MyMath.GetRandomFloat(-1, 1) * mag;
            shakeOffset.Y = MyMath.GetRandomFloat(-1, 1) * mag;
            shakeOffset.Z = MyMath.GetRandomFloat(-1, 1) * mag;
        }

        /// <summary>Reset Camera: rotation(0,0,0) world(0,0,0)</summary>
        public void ResetCamera()
        {
            yaw = pitch = roll = 0.0f;
            camRotation = Matrix.Identity;
            position = Vector3.Zero;
        }

        /// <summary>Determine whether the boundingsphere is within the view frustum</summary>
        /// <param name="bSpere">bounding sphere to text</param>
        /// <returns>true if inside of frustum, false if outside of frustum</returns>
        public bool IsInFrustum(BoundingSphere bSpere)
        {
            ContainmentType type = ContainmentType.Disjoint;

            type = this.boundingFrustum.Contains(bSpere);
            if (type != ContainmentType.Disjoint)
                return true;
            else return false;
        }

        /// <summary>shake the camera</summary>
        /// <param name="magnitude">amount of movement from the center point</param>
        /// <param name="duration">millisecond duration of shakage</param>
        /// <param name="ramp">ramp the magnitude going in and going out</param>
        public void Shake(float magnitude, float duration, bool ramp = true)
        {
            this.shakeMagnitude = magnitude;
            this.shakeRamp = ramp;
            this.shakeDuration = duration;
            this.shakeTimer = 0f;
        }

        #endregion API

        #region Mutators
        public Vector3 Position
        {
            set { position = value; }
            get { return position; }
        }
        public Vector4 PositionDot4
        {
            get { return new Vector4(position.X, position.Y, position.Z, 1f); }
        }
        public Vector3 LookAtTarget
        {
            set { lookAtTarget = value; }
            get { return lookAtTarget; }
        }
        public float AspectRatio
        {
            get { return aspectRatio; }
            set
            {
                aspectRatio = value;
                projection = Matrix.CreatePerspectiveFieldOfView( MathHelper.PiOver4, 
                    aspectRatio,
                    nearPlaneDist,
                    farPlaneDist);

            }
        }
        public float NearPlaneDist
        {
            get { return nearPlaneDist; }
            set 
            { 
                nearPlaneDist = value;
                projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                    aspectRatio, 
                    nearPlaneDist, 
                    farPlaneDist);
            }
        }
        public float FarPlaneDist
        {
            get { return farPlaneDist; }
            set 
            { 
                farPlaneDist = value;
                projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 
                    aspectRatio, 
                    nearPlaneDist, 
                    farPlaneDist);
            }
        }
        public Matrix ViewMatrix
        {
            get { return view; }
        }
        public Matrix ViewInverseMatrix
        {
            get { return viewInverse; }
        }
        public Matrix ProjectionMatrix
        {
            get { return projection; }
        }
        public Matrix RotationMatrix
        {
            get { return camRotation; }
            set { this.camRotation = value; }
        }
        public float Yaw
        {
            get { return yaw; }
            set { yaw = value; }
        }
        public float Pitch
        {
            get { return pitch; }
            set { pitch = value; }
        }
        public float Roll
        {
            get { return this.roll; }
            set { this.roll = value; }
        }
        public Viewport Viewport
        {
            get { return viewport; }
            set { viewport = value; }
        }
        public float X
        {
            get { return this.position.X; }
            set { this.position.X = value; }
        }
        public float Y
        {
            get { return this.position.Y; }
            set { this.position.Y = value; }
        }
        public float Z
        {
            get { return this.position.Z; }
            set { this.position.Z = value; }
        }
        public float lX
        {
            get { return this.lookAtTarget.X; }
            set { this.lookAtTarget.X = value; }
        }
        public float lY
        {
            get { return this.lookAtTarget.Y; }
            set { this.lookAtTarget.Y = value; }
        }
        public float lZ
        {
            get { return this.lookAtTarget.Z; }
            set { this.lookAtTarget.Z = value; }
        }
        public ICamera Behavior
        {
            set { this.behavior = value; }
            get { return this.behavior; }
        }
        #endregion
    }
}
