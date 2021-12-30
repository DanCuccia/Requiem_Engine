using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Engine.Math_Physics;

namespace Engine.Managers.Camera
{
    /// <summary>Orbit Camera behavior</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class OrbitCamera : ICamera
    {
        Camera camera;
        WorldMatrix origin;

        float orbitAngle = 0;
        float orbitDist = 100f;
        float orbitHeight = 50f;
        float orbitSpeed = 0.5f;

        /// <summary>default ctor</summary>
        /// <param name="camera">reference to camera this behavior is affecting</param>
        /// <param name="world">reference to the world matrix this behavior will use as "origin"</param>
        public OrbitCamera(ref Camera camera, ref WorldMatrix world)
        {
            this.camera = camera;
            this.origin = world;
        }

        /// <summary>update orbits the camera around origin</summary>
        public void Update(GameTime time)
        {
            camera.Y = orbitHeight;
            camera.X = orbitDist * (float)Math.Sin(MathHelper.ToRadians(orbitAngle));
            camera.Z = orbitDist * (float)Math.Cos(MathHelper.ToRadians(orbitAngle));

            camera.lX = origin.X;
            camera.lY = origin.Y + 20f;
            camera.lZ = origin.Z;

            orbitAngle += orbitSpeed;
        }

        /// <summary>un-used, this is an automated behavior</summary>
        public void Input(KeyboardState kb, MouseState ms)
        { }

        /// <summary>current angle of the orbit</summary>
        public float OrbitAngle
        {
            get { return this.orbitAngle; }
            set { this.orbitAngle = value; }
        }
        /// <summary>distance from center</summary>
        public float OrbitDistanace
        {
            get { return this.orbitDist; }
            set { this.orbitDist = value; }
        }
        /// <summary>height off center</summary>
        public float OrbitHeight
        {
            get { return this.orbitHeight; }
            set { this.orbitHeight = value; }
        }
        /// <summary>orbit speed</summary>
        public float OrbitSpeed
        {
            get { return this.orbitSpeed; }
            set { this.orbitSpeed = value; }
        }
    }
}
