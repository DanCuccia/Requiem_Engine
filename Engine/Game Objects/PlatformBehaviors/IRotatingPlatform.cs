using System;
using Engine.Game_Objects.PlatformHuds;
using Engine.Managers.Camera;
using Engine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
#pragma warning disable 1591

namespace Engine.Game_Objects.PlatformBehaviors
{
    /// <summary>Serializable behavior object</summary>
    [Serializable]
    public class RotatingBehaviorXml : PlatformBehaviorXML
    {
        public float speed;
    }

    /// <summary>rotating behavior</summary>
    /// <author>Daniel Cuccia</author>
    public class IRotatingPlatform : IPlatform
    {
        Platform platform;

        float speed = IRotatingPlatform.MinSpeed;
        public float Speed
        {
            set { speed = value; }
            get { return speed; }
        }

        public const float MinSpeed = 1f;
        public const float MaxSpeed = 7.5f;

        /// <summary>default ctor</summary>
        /// <param name="platform">platform pointer</param>
        public IRotatingPlatform(Platform platform)
        {
            this.platform = platform;
        }

        /// <summary>update logic</summary>
        /// <param name="camera">camera reference for culling</param>
        /// <param name="time">game time</param>
        public void Update(ref Camera camera, GameTime time)
        {
            platform.WorldMatrix.rZ += speed;
            if (platform.WorldMatrix.rZ >= 360)
                platform.WorldMatrix.rZ -= 360;
        }

        /// <summary>get the hud for this platform</summary>
        /// <param name="content">content manager</param>
        /// <param name="editor">Level Editor Scene</param>
        /// <returns>platform hud</returns>
        public PlatformHud GetHud(ContentManager content, EditorScene editor)
        {
            RotatingPlatformHud h = new RotatingPlatformHud(editor, this.platform);
            h.Initialize(content);
            return h;
        }

        /// <summary>Return the serializable object holding behavior values</summary>
        /// <returns>behavior values</returns>
        public PlatformBehaviorXML GetBehaviorXml()
        {
            RotatingBehaviorXml o = new RotatingBehaviorXml();
            o.speed = this.speed;
            return o;
        }

        /// <summary>override behavior type enum to this interface type</summary>
        /// <returns>stationary enum value</returns>
        public PlatformBehaviorType GetBehaviorType()
        { return PlatformBehaviorType.Rotating; }

        /// <summary>Overriden to a hardcoded name of this behavior</summary>
        /// <returns>rotating string</returns>
        public string GetName()
        { return "Rotating Platform"; }

        /// <summary>load from xml</summary>
        /// <param name="inputXml"></param>
        public void FromXml(PlatformBehaviorXML inputXml)
        {
            RotatingBehaviorXml input = inputXml as RotatingBehaviorXml;
            this.speed = input.speed;
        }
    }
}
