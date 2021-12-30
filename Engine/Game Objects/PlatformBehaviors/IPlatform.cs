using System;
using System.Xml.Serialization;
using Engine.Drawing_Objects;
using Engine.Game_Objects.PlatformHuds;
using Engine.Managers.Camera;
using Engine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Engine.Game_Objects.PlatformBehaviors
{
    /// <summary>Base serializing class</summary>
    [Serializable]
    [XmlInclude(typeof(HorizontalBehaviorXml))]
    [XmlInclude(typeof(RotatingBehaviorXml))]
    [XmlInclude(typeof(StationaryBehaviorXml))]
    [XmlInclude(typeof(VerticalBehaviorXml))]
    public abstract class PlatformBehaviorXML : XMLMedium
    { }

    /// <summary>enum values of what type of behavior a platform interface is</summary>
    public enum PlatformBehaviorType
    {
        /// <summary>does not move</summary>
        Stationary = 0,
        /// <summary>moves left/right</summary>
        Horizontal,
        /// <summary>moves up/down</summary>
        Vertical,
        /// <summary>rotates on x</summary>
        Rotating
    }

    /// <summary>Platform behavior interface</summary>
    /// <author>Daniel Cuccia</author>
    public interface IPlatform
    {
        /// <summary>Update logic</summary>
        /// <param name="camera">Camera reference for culling</param>
        /// <param name="time">game time</param>
        void Update(ref Camera camera, GameTime time);

        /// <summary>get the hud of the current platform</summary>
        /// <param name="content">content manager</param>
        /// <param name="editor">Level Editor Scene</param>
        /// <returns>hud object</returns>
        PlatformHud GetHud(ContentManager content, EditorScene editor);

        /// <summary>Return the serializable object holding behavior values</summary>
        /// <returns>behavior values</returns>
        PlatformBehaviorXML GetBehaviorXml();

        /// <summary>Get the enum value of the type of behavior this is</summary>
        /// <returns>PlatformBehaviorType enum value</returns>
        PlatformBehaviorType GetBehaviorType();

        /// <summary>Get the name of derived behariors (for editor</summary>
        /// <returns>string name</returns>
        string GetName();

        /// <summary>load local parameters from xml</summary>
        /// <param name="inputXml">deserialized object</param>
        void FromXml(PlatformBehaviorXML inputXml);
    }
}
