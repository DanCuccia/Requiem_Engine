using Engine.Drawing_Objects;
using Engine.Game_Objects.PlatformBehaviors;
using Engine.Math_Physics;
using Engine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Engine.Game_Objects.PlatformHuds
{
    /// <summary>a vertical moving platform hud</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class HorizontalPlatformHud : PlatformHud
    {
        /// <summary>Default CTOR</summary>
        /// <param name="platform">reference to the edited platform</param>
        /// <param name="editor">Level Editor Scene</param>
        public HorizontalPlatformHud(EditorScene editor, Platform platform)
            : base(editor, platform)
        { }

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
                256f, "Distance", onChange, IHorizontalPlatform.OffsetMax - IHorizontalPlatform.OffsetMin);
            slider.SetValue(MyMath.GetScalarBetween(IHorizontalPlatform.OffsetMin, 
                IHorizontalPlatform.OffsetMax, (platform.Behavior as IHorizontalPlatform).Distance));
            base.AddSlider(slider);

            slider = new Slider(content, new Vector2(Rect.X, Rect.Y + 72),
                256f, "Speed", onChange, IHorizontalPlatform.MaxSpeed - IHorizontalPlatform.MinSpeed);
            slider.SetValue(MyMath.GetScalarBetween(IHorizontalPlatform.MinSpeed, IHorizontalPlatform.MaxSpeed,
                (platform.Behavior as IHorizontalPlatform).Speed));
            base.AddSlider(slider);
        }

        /// <summary>load sprites</summary>
        /// <param name="content">content manager</param>
        protected override void LoadSprites(ContentManager content)
        {

        }

        private void onChange()
        {
            (platform.Behavior as IHorizontalPlatform).Distance = MyMath.GetValueBetween(
                IHorizontalPlatform.OffsetMin, IHorizontalPlatform.OffsetMax, sliderList[0].GetValue());
            (platform.Behavior as IHorizontalPlatform).Speed = MyMath.GetValueBetween(
                IHorizontalPlatform.MinSpeed, IHorizontalPlatform.MaxSpeed, sliderList[1].GetValue());
        }
    }
}
