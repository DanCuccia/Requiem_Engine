using Engine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Engine.Drawing_Objects;
using Engine.Math_Physics;
using Engine.Game_Objects.PlatformBehaviors;

namespace Engine.Game_Objects.PlatformHuds
{
    /// <summary>A platform which rotates HUD</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class RotatingPlatformHud : PlatformHud
    {
        /// <summary>Default CTOR</summary>
        /// <param name="platform">reference to the edited platform</param>
        /// <param name="editor">Level Editor Scene</param>
        public RotatingPlatformHud(EditorScene editor, Platform platform)
            : base(editor, platform)
        {

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
            Slider slider = new Slider(content, new Vector2(Rect.X, Rect.Y), 256f, "Speed",
                onChange, IRotatingPlatform.MaxSpeed - IRotatingPlatform.MinSpeed);
            slider.SetValue(MyMath.GetScalarBetween(IRotatingPlatform.MinSpeed, IRotatingPlatform.MaxSpeed,
                (platform.Behavior as IRotatingPlatform).Speed));
            base.AddSlider(slider);
        }

        /// <summary>load sprites</summary>
        /// <param name="content">content manager</param>
        protected override void LoadSprites(ContentManager content)
        {

        }

        private void onChange()
        {
            (platform.Behavior as IRotatingPlatform).Speed = MyMath.GetValueBetween(
                IRotatingPlatform.MinSpeed, IRotatingPlatform.MaxSpeed, sliderList[0].GetValue());
        }
    }
}
