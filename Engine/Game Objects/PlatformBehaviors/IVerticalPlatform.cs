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
    public class VerticalBehaviorXml : PlatformBehaviorXML
    {
        public float offset;
        public float speed;
        public float centerAxisPosition;
    }

    /// <summary>Vertical Platform behavior interface</summary>
    /// <author>Daniel Cuccia</author>
    public class IVerticalPlatform : IPlatform
    {
        Platform platform;
        float offsetValue = IVerticalPlatform.OffsetMin;
        float currentOffset = 0f;
        float speed = IVerticalPlatform.MinSpeed;
        public float center;
        public float Distance
        {
            set { offsetValue = value; }
            get { return offsetValue; }
        }
        public float Speed
        {
            set { speed = value; }
            get { return speed; }
        }

        /// <summary>max move this value in both directs</summary>
        public const float OffsetMax = 2000f;
        /// <summary>min move this value in both directs</summary>
        public const float OffsetMin = 100f;
        /// <summary>max platform speed</summary>
        public const float MaxSpeed = 5f;
        /// <summary>min platform speed</summary>
        public const float MinSpeed = 0.5f;

        /// <summary>default ctor</summary>
        /// <param name="platform">platform pointer</param>
        public IVerticalPlatform(Platform platform)
        {
            this.platform = platform;
        }

        /// <summary>update logic</summary>
        /// <param name="camera">camera reference for culling</param>
        /// <param name="time">game time</param>
        public void Update(ref Camera camera, GameTime time)
        {
            currentOffset += speed;
            if (currentOffset >= 360)
                currentOffset -= 360;
            platform.WorldMatrix.Y = center + ((float)Math.Sin(MathHelper.ToRadians(currentOffset)) * offsetValue);
        }

        /// <summary>get the hud for this platform</summary>
        /// <param name="content">content manager</param>
        /// <param name="editor">Level Editor Scene</param>
        /// <returns>platform hud</returns>
        public PlatformHud GetHud(ContentManager content, EditorScene editor)
        {
            VerticalPlatformHud h = new VerticalPlatformHud(editor, this.platform);
            h.Initialize(content);
            return h;
        }

        /// <summary>Return the serializable object holding behavior values</summary>
        /// <returns>behavior values</returns>
        public PlatformBehaviorXML GetBehaviorXml()
        {
            VerticalBehaviorXml o = new VerticalBehaviorXml();
            o.offset = this.offsetValue;
            o.speed = this.speed;
            o.centerAxisPosition = this.center;
            return o;
        }

        /// <summary>override behavior type enum to this interface type</summary>
        /// <returns>stationary enum value</returns>
        public PlatformBehaviorType GetBehaviorType()
        { return PlatformBehaviorType.Vertical; }

        /// <summary>Overriden to a hardcoded name of this behavior</summary>
        /// <returns>vertical string</returns>
        public string GetName()
        { return "Vertical Platform"; }

        /// <summary>load from xml</summary>
        /// <param name="inputXml"></param>
        public void FromXml(PlatformBehaviorXML inputXml)
        {
            VerticalBehaviorXml input = inputXml as VerticalBehaviorXml;
            this.speed = input.speed;
            this.offsetValue = input.offset;
            this.center = input.centerAxisPosition;
        }
    }
}
