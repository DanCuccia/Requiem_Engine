using Engine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Engine.Game_Objects.PlatformHuds
{
    /// <summary>a stationary platform hud</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class StationaryPlatformHud : PlatformHud
    {
        /// <summary>Default CTOR</summary>
        /// <param name="platform">reference to the edited platform</param>
        /// <param name="editor">Level Editor Scene</param>
        public StationaryPlatformHud(EditorScene editor, Platform platform)
            : base(editor, platform)
        {

        }

        /// <summary>override placement of this hud</summary>
        /// <returns>new hud location</returns>
        protected override Rectangle LoadRectangle()
        {
            return new Rectangle(0, editor.device.Viewport.Height - 400, 256, 400);
        }

        /// <summary>load buttons</summary>
        /// <param name="content">content manager</param>
        protected override void LoadButtons(ContentManager content)
        {

        }

        /// <summary>load sliders</summary>
        /// <param name="content">content manager</param>
        protected override void LoadSliders(ContentManager content)
        {

        }

        /// <summary>load sprites</summary>
        /// <param name="content">content manager</param>
        protected override void LoadSprites(ContentManager content)
        {

        }
    }
}
