using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Scenes.IScene_Animations
{
    /// <summary>
    /// Create a concrete interface from this, assign it to a scene, and now you can have
    /// entirely seperated scene update logic only used for animating in and out of the scene
    /// 
    /// DO NOT INHERIT DIRECTLY FROM THIS INTERFACE,
    /// -you must inherit new animations from:       CSceneAnimateOUT
    /// 
    /// This logic REPLACES your standard scene Update logic,
    ///     -if you need to update sprite animations, do so manually,
    ///     -if you need to update managers, you MUST do it manually.
    ///         -This includes camera!
    ///         
    /// During the time this active, your scene input() WILL NOT be called
    /// </summary>
    /// <author>Daniel Cuccia</author>
    public interface ISceneAnimateOut
    {
        /// <summary>animte out cycle logic</summary>
        /// <param name="time">current game timing values</param>
        void OnAnimateOut(GameTime time);
        /// <summary>2D render call</summary>
        /// <param name="batch">xna sprite batch rendering object</param>
        void Render2D(SpriteBatch batch);
        /// <summary>render 3D call</summary>
        void Render3D();
    }
}
