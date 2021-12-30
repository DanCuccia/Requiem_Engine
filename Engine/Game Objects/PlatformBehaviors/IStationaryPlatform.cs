using System;
using Engine.Game_Objects.PlatformHuds;
using Engine.Managers.Camera;
using Engine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Engine.Game_Objects.PlatformBehaviors
{
    /// <summary>Serializable behavior object</summary>
    [Serializable]
    public class StationaryBehaviorXml : PlatformBehaviorXML
    {
    }

    /// <summary>Default platform behavior</summary>
    /// <author>Daniel Cuccia</author>
    public class IStationaryPlatform : IPlatform
    {
        Platform platform;

        /// <summary>default ctor</summary>
        /// <param name="platform">platform pointer</param>
        public IStationaryPlatform(Platform platform)
        {
            this.platform = platform;
        }

        /// <summary>update logic</summary>
        /// <param name="camera">camera reference for culling</param>
        /// <param name="time">game time</param>
        public void Update(ref Camera camera, GameTime time)
        { }

        /// <summary>get the hud for this platform</summary>
        /// <param name="content">content manager</param>
        /// <param name="editor">Level Editor Scene</param>
        /// <returns>platform hud</returns>
        public PlatformHud GetHud(ContentManager content, EditorScene editor)
        {
            StationaryPlatformHud h = new StationaryPlatformHud(editor, this.platform);
            h.Initialize(content);
            return h;
        }

        /// <summary>Return the serializable object holding behavior values</summary>
        /// <returns>behavior values</returns>
        public PlatformBehaviorXML GetBehaviorXml()
        {
            return new StationaryBehaviorXml();
        }

        /// <summary>override behavior type enum to this interface type</summary>
        /// <returns>stationary enum value</returns>
        public PlatformBehaviorType GetBehaviorType()
        { return PlatformBehaviorType.Stationary; }

        /// <summary>Overriden to a hardcoded name of this behavior</summary>
        /// <returns>stationary string</returns>
        public string GetName()
        { return "Stationary Platform"; }

        /// <summary>load from xml</summary>
        /// <param name="inputXml"></param>
        public void FromXml(PlatformBehaviorXML inputXml) { }
    }
}
