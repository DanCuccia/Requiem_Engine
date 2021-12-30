using Engine.Math_Physics;
using Engine;
using Engine.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Managers.Camera
{
    /// <summary>lock on the X axis behavior, follows a WorldMatrix reference</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class AxisLockCamera : ICamera
    {
        Engine.Managers.Camera.Camera camera;
        WorldMatrix chase;
        float chaseDistance;
        Vector3 axisLock;
        Vector3 positionOffset;

        /// <summary>default ctor</summary>
        /// <param name="camera">reference to the camera this behavior affects</param>
        /// <param name="world">reference to the worldmatrix this camera is chasing</param>
        /// <param name="axis">put 1 in the vector3 component, depicting what axis the camera will lock onto</param>
        /// <param name="distance">how far away you want the camera to be from the given worldMatrix object</param>
        /// <param name="positionOffset">camera position offset</param>
        public AxisLockCamera(ref Engine.Managers.Camera.Camera camera, ref WorldMatrix world, Vector3 axis, Vector3 positionOffset, float distance = 750f)
        {
            this.camera = camera;
            this.chase = world;
            this.chaseDistance = distance;
            this.axisLock = axis;
            this.positionOffset = positionOffset;
        }

        /// <summary>update logic</summary>
        public void Update(GameTime time)
        {
            camera.X = chase.X + (chaseDistance * axisLock.X) + positionOffset.X;
            camera.Y = chase.Y + (chaseDistance * axisLock.Y) + positionOffset.Y;
            camera.Z = chase.Z + (chaseDistance * axisLock.Z) + positionOffset.Z;
            camera.lX = chase.X;
            camera.lY = chase.Y;
            camera.lZ = chase.Z;
        }

        /// <summary>unused input logic</summary>
        public void Input(KeyboardState kb, MouseState ms)
        { }

        /// <summary>Distance from target</summary>
        public float Distance
        {
            get { return this.chaseDistance; }
            set { this.chaseDistance = value; }
        }
        /// <summary>what axis this behavior is locked to</summary>
        public Vector3 LockedAxis
        {
            get { return this.axisLock; }
            set { this.axisLock = value; }
        }
        /// <summary>offset to camera position</summary>
        public Vector3 PositionOffset
        {
            get { return this.positionOffset; }
            set { this.positionOffset = value; }
        }
    }
}
