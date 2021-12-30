using Engine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Engine.Drawing_Objects;
using Engine.Math_Physics;
using Engine.Game_Objects.PlatformBehaviors;

namespace Engine.Game_Objects.PlatformHuds
{
    /// <summary>a vertical moving platform hud</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class VerticalPlatformHud : PlatformHud
    {
        /// <summary>Default CTOR</summary>
        /// <param name="platform">reference to the edited platform</param>
        /// <param name="editor">Level Editor Scene</param>
        public VerticalPlatformHud(EditorScene editor, Platform platform)
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
            Slider slider;

            slider = new Slider(content, new Vector2(Rect.X, Rect.Y),
                256f, "Distance", onChange, IVerticalPlatform.OffsetMax - IVerticalPlatform.OffsetMin);
            slider.SetValue(MyMath.GetScalarBetween(IVerticalPlatform.OffsetMin,
                IVerticalPlatform.OffsetMax, (platform.Behavior as IVerticalPlatform).Distance));
            base.AddSlider(slider);

            slider = new Slider(content, new Vector2(Rect.X, Rect.Y + 72),
                256f, "Speed", onChange, IVerticalPlatform.MaxSpeed - IVerticalPlatform.MinSpeed);
            slider.SetValue(MyMath.GetScalarBetween(IVerticalPlatform.MinSpeed, IVerticalPlatform.MaxSpeed,
                (platform.Behavior as IVerticalPlatform).Speed));
            base.AddSlider(slider);
        }

        /// <summary>load sprites</summary>
        /// <param name="content">content manager</param>
        protected override void LoadSprites(ContentManager content)
        {

        }

        private void onChange()
        {
            (platform.Behavior as IVerticalPlatform).Distance = MyMath.GetValueBetween(
                IVerticalPlatform.OffsetMin, IVerticalPlatform.OffsetMax, sliderList[0].GetValue());
            (platform.Behavior as IVerticalPlatform).Speed = MyMath.GetValueBetween(
                IVerticalPlatform.MinSpeed, IVerticalPlatform.MaxSpeed, sliderList[1].GetValue());
        }
    }
}
