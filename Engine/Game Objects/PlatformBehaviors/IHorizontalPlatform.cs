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
    public class HorizontalBehaviorXml : PlatformBehaviorXML
    {
        public float offset;
        public float speed;
        public float centerAxisPosition;
    }

    /// <summary>horizontal moving behavior</summary>
    /// <author>Dan Cuccia</author>
    public class IHorizontalPlatform : IPlatform
    {
        Platform platform;
        float offsetValue = IHorizontalPlatform.OffsetMin;
        public float Distance
        {
            set { offsetValue = value; }
            get { return offsetValue; }
        }
        float currentOffset = 0f;
        float speed = IHorizontalPlatform.MinSpeed;
        public float Speed
        {
            set { speed = value; }
            get { return speed; }
        }
        public float center;

        /// <summary>max move this value in both directs</summary>
        public const float OffsetMax = 2000f;
        /// <summary>min move this value in both directs</summary>
        public const float OffsetMin = 100f;
        /// <summary>max platform speed</summary>
        public const float MaxSpeed = 5;
        /// <summary>min platform speed</summary>
        public const float MinSpeed = 0.1f;

        /// <summary>default ctor</summary>
        /// <param name="platform"></param>
        public IHorizontalPlatform(Platform platform)
        {
            this.platform = platform;
            this.center = platform.WorldMatrix.X;
        }

        /// <summary>update logic</summary>
        /// <param name="camera">camera reference for culling</param>
        /// <param name="time">game time</param>
        public void Update(ref Camera camera, GameTime time)
        {
            currentOffset += speed;
            if (currentOffset >= 360)
                currentOffset -= 360;
            platform.WorldMatrix.X = center + ((float)Math.Sin(MathHelper.ToRadians(currentOffset)) * offsetValue);
        }

        /// <summary>get the hud for this platform</summary>
        /// <param name="content">content manager</param>
        /// <param name="editor">level editor scene</param>
        /// <returns>platform hud</returns>
        public PlatformHud GetHud(ContentManager content, EditorScene editor)
        {
            PlatformHud h = new HorizontalPlatformHud(editor, this.platform);
            h.Initialize(content);
            return h;
        }

        /// <summary>Return the serializable object holding behavior values</summary>
        /// <returns>behavior values</returns>
        public PlatformBehaviorXML GetBehaviorXml()
        {
            HorizontalBehaviorXml output = new HorizontalBehaviorXml();
            output.offset = this.offsetValue;
            output.speed = this.speed;
            output.centerAxisPosition = center;
            return output;
        }

        /// <summary>override behavior type enum to this interface type</summary>
        /// <returns>stationary enum value</returns>
        public PlatformBehaviorType GetBehaviorType()
        { return PlatformBehaviorType.Horizontal; }

        /// <summary>Overriden to a hardcoded name of this behavior</summary>
        /// <returns>horizonal string</returns>
        public string GetName()
        { return "Horizontal Platform"; }

        /// <summary>load from xml</summary>
        /// <param name="inputXml"></param>
        public void FromXml(PlatformBehaviorXML inputXml)
        {
            HorizontalBehaviorXml input = inputXml as HorizontalBehaviorXml;
            this.speed = input.speed;
            this.offsetValue = input.offset;
            this.center = input.centerAxisPosition;
        }
    }
}
