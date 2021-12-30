using System.Collections.Generic;
using Engine.Drawing_Objects;
using Engine.Managers;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine.Scenes.HUD;
using Engine.Scenes;

namespace Engine.Game_Objects.PlatformHuds
{
    /// <summary>Each type of platform has a hud for the level editor to use</summary>
    /// <author>Daniel Cuccia</author>
    public abstract class PlatformHud : HudBase
    {
        /// <summary>platform</summary>
        protected Platform platform;

        /// <summary>base construction</summary>
        /// <param name="platform">reference to the edited platform</param>
        /// <param name="editor">Level Editor Scene</param>
        public PlatformHud(EditorScene editor, Platform platform)
            :base(editor)
        {
            this.platform = platform;
        }

        /// <summary>Platform huds will default to the left side</summary>
        /// <returns>left sided rectangle</returns>
        protected override Rectangle LoadRectangle()
        {
            return new Rectangle(0, editor.device.Viewport.Height - 400 - 99, 256, 400);
        }
    }
}
