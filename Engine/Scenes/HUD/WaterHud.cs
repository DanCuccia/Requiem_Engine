using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine.Drawing_Objects;
using Engine.Drawing_Objects.Materials;
using Engine.Math_Physics;
using Engine.Managers;
#pragma warning disable 1591

namespace Engine.Scenes.HUD
{
    /// <summary>This Hud will display a schmorgesborg of sliders to </summary>
    /// <author>Daniel Cuccia</author>
    public sealed class WaterHud : HudBase
    {
        Rectangle compositionDeep;
        Rectangle compositionShallow;
        public Rectangle line;
        WaterMaterial material;

        /// <summary>Default CTOR</summary>
        /// <param name="editor">level editor pointer</param>
        /// <param name="material">the water material this hud will edit</param>
        public WaterHud(EditorScene editor, WaterMaterial material)
            :base(editor)
        {
            this.material = material;
            line = new Rectangle(Rect.X, Rect.Y + 28 + (48 * 3) + 5, Rect.Width, 2);
        }

        /// <summary>overriden background rect</summary>
        /// <returns>rect</returns>
        protected override Rectangle LoadRectangle()
        {
            return new Rectangle(editor.device.Viewport.Width - 512, 408, 512, 402);
        }

        /// <summary>privately called to load visual sliders</summary>
        protected override void LoadSliders(ContentManager content)
        {
            compositionDeep = new Rectangle(Rect.X + 128, Rect.Y + 2, 92, 24);

            Slider slider = new Slider(content, new Vector2(Rect.X + 6, Rect.Y + 28), 256 - 6, "Red", onDeepRedChange, 255f);
            slider.SetValue(material.Settings.DeepColor.X);
            base.AddSlider(slider);
            slider = new Slider(content, new Vector2(Rect.X + 6, Rect.Y + 28 + 48), 256 - 6, "Green", onDeepGreenChange, 255f);
            slider.SetValue(material.Settings.DeepColor.Y);
            base.AddSlider(slider);
            
            slider = new Slider(content, new Vector2(Rect.X + 6, Rect.Y + 28 + 48 + 48), 256 - 6, "Blue", onDeepBlueChange, 255f);
            slider.SetValue(material.Settings.DeepColor.Z);
            base.AddSlider(slider);

            compositionShallow = new Rectangle(Rect.X + 256 + 128, Rect.Y + 2, 92, 24);

            slider = new Slider(content, new Vector2(Rect.X + 6 + 256, Rect.Y + 28), 256 - 6, "Red", onShallowRedChange, 255f);
            slider.SetValue(material.Settings.ShallowColor.X);
            base.AddSlider(slider);
            slider = new Slider(content, new Vector2(Rect.X + 6 + 256, Rect.Y + 28 + 48), 256 - 6, "Green", onShallowGreenChange, 255f);
            slider.SetValue(material.Settings.ShallowColor.Y);
            base.AddSlider(slider);
            slider = new Slider(content, new Vector2(Rect.X + 6 + 256, Rect.Y + 28 + 48 + 48), 256 - 6, "Blue", onShallowBlueChange, 255f);
            slider.SetValue(material.Settings.ShallowColor.Z);
            base.AddSlider(slider);

            slider = new Slider(content, new Vector2(Rect.X + 6, Rect.Y + 28 + (48 * 3) + 16), 256 - 6, "Fresnal Bias", onFresnalChange);
            slider.SetValue((material.Settings.FresnelBias - WaterSettings.Min_FresnalBias) / (WaterSettings.Max_FresnalBias - WaterSettings.Min_FresnalBias));
            base.AddSlider(slider);

            slider = new Slider(content, new Vector2(Rect.X + 6, Rect.Y + 28 + (48 * 4) + 20), 256 - 6, "Fresnal Power", onFresnalPowerChange);
            slider.SetValue((material.Settings.FresnelPower - WaterSettings.Min_FresnalPower) / (WaterSettings.Max_FresnalPower - WaterSettings.Min_FresnalPower));
            base.AddSlider(slider);

            slider = new Slider(content, new Vector2(Rect.X + 6 + 256, Rect.Y + 28 + (48 * 4) + 20), 256 - 6, "HDR Multiplier", onHDRMultiplierChange);
            slider.SetValue((material.Settings.HDRMultiplier - WaterSettings.Min_HDRMultiplier) / (WaterSettings.Max_HDRMultiplier - WaterSettings.Min_HDRMultiplier));
            base.AddSlider(slider);

            slider = new Slider(content, new Vector2(Rect.X + 6 + 256, Rect.Y + 28 + (48 * 3) + 20), 256 - 6, "Reflection Amount", onReflectionAmountChange);
            slider.SetValue((material.Settings.ReflectionAmount - WaterSettings.Min_ReflectionAmount) / (WaterSettings.Max_ReflectionAmount - WaterSettings.Min_ReflectionAmount));
            base.AddSlider(slider);

            slider = new Slider(content, new Vector2(Rect.X + 6, Rect.Y + 28 + (48 * 5) + 20), 256 - 6, "Texture Scale X", onTextureScaleXChange);
            slider.SetValue((material.Settings.TextureScale.X - WaterSettings.Min_TextureScale) / (WaterSettings.Max_TextureScale - WaterSettings.Min_TextureScale));
            base.AddSlider(slider);

            slider = new Slider(content, new Vector2(Rect.X + 6, Rect.Y + 28 + (48 * 6) + 20), 256 - 6, "Texture Scale Y", onTextureScaleYChange);
            slider.SetValue((material.Settings.TextureScale.Y - WaterSettings.Min_TextureScale) / (WaterSettings.Max_TextureScale - WaterSettings.Min_TextureScale));
            base.AddSlider(slider);

            slider = new Slider(content, new Vector2(Rect.X + 6 + 256, Rect.Y + 28 + (48 * 5) + 20), 256 - 6, "Bump Speed X", onBumpSpeedXChange);
            slider.SetValue((material.Settings.BumpSpeed.X - WaterSettings.Min_BumpSpeed) / (WaterSettings.Max_BumpSpeed - WaterSettings.Min_BumpSpeed) +.5f);
            base.AddSlider(slider);

            slider = new Slider(content, new Vector2(Rect.X + 6 + 256, Rect.Y + 28 + (48 * 5) + 20 + 48), 256 - 6, "Bump Speed Y", onBumpSpeedYChange);
            slider.SetValue((material.Settings.BumpSpeed.Y - WaterSettings.Min_BumpSpeed) / (WaterSettings.Max_BumpSpeed - WaterSettings.Min_BumpSpeed) + .5f);
            base.AddSlider(slider);
        }

        /// <summary>no buttons in this hud</summary>
        /// <param name="content">content manager</param>
        protected override void LoadButtons(ContentManager content) { }

        /// <summary>unused spirtes</summary>
        /// <param name="content">content manager</param>
        protected override void LoadSprites(ContentManager content) { }

        protected override void Render2DExtra(SpriteBatch batch)
        {
            batch.DrawString(font, "Deep Color", new Vector2(Rect.X + 24, Rect.Y + 9), Color.White);
            batch.Draw(blank, compositionDeep, new Color(sliderList[0].GetValue(), sliderList[1].GetValue(), sliderList[2].GetValue()));
            batch.DrawString(font, "Shallow Color", new Vector2(Rect.X + 24 + 256, Rect.Y + 9), Color.White);
            batch.Draw(blank, compositionShallow, new Color(sliderList[3].GetValue(), sliderList[4].GetValue(), sliderList[5].GetValue()));
            batch.Draw(blank, line, MyColors.AlphaWhite);
        }

        #region Callbacks
        public void onDeepRedChange()
        {
            material.Settings.DeepColor.X = sliderList[0].GetValue();
        }

        public void onDeepGreenChange()
        {
            material.Settings.DeepColor.Y = sliderList[1].GetValue();
        }

        public void onDeepBlueChange()
        {
            material.Settings.DeepColor.Z = sliderList[2].GetValue();
        }

        public void onShallowRedChange()
        {
            material.Settings.ShallowColor.X = sliderList[3].GetValue();
        }

        public void onShallowGreenChange()
        {
            material.Settings.ShallowColor.Y = sliderList[4].GetValue();
        }

        public void onShallowBlueChange()
        {
            material.Settings.ShallowColor.Z = sliderList[5].GetValue();
        }

        public void onFresnalChange()
        {
            material.Settings.FresnelBias = WaterSettings.Min_FresnalBias + (sliderList[6].GetValue() * (WaterSettings.Max_FresnalBias - WaterSettings.Min_FresnalBias));
        }

        public void onFresnalPowerChange()
        {
            material.Settings.FresnelPower = WaterSettings.Min_FresnalPower + (sliderList[7].GetValue() * (WaterSettings.Max_FresnalPower - WaterSettings.Min_FresnalPower));
        }

        public void onHDRMultiplierChange()
        {
            material.Settings.HDRMultiplier = WaterSettings.Min_HDRMultiplier + (sliderList[8].GetValue() * (WaterSettings.Max_HDRMultiplier - WaterSettings.Min_HDRMultiplier));
        }

        public void onReflectionAmountChange()
        {
            material.Settings.ReflectionAmount = WaterSettings.Min_ReflectionAmount + (sliderList[9].GetValue() * (WaterSettings.Max_ReflectionAmount - WaterSettings.Min_ReflectionAmount));
        }

        public void onTextureScaleXChange()
        {
            material.Settings.TextureScale.X = WaterSettings.Min_TextureScale + (sliderList[10].GetValue() * (WaterSettings.Max_TextureScale - WaterSettings.Min_TextureScale));
        }

        public void onTextureScaleYChange()
        {
            material.Settings.TextureScale.Y = WaterSettings.Min_TextureScale + (sliderList[11].GetValue() * (WaterSettings.Max_TextureScale - WaterSettings.Min_TextureScale));
        }

        public void onBumpSpeedXChange()
        {
            float w = (WaterSettings.Max_BumpSpeed - WaterSettings.Min_BumpSpeed);
            material.Settings.BumpSpeed.X = WaterSettings.Min_BumpSpeed + (sliderList[12].GetValue() * w);
        }

        public void onBumpSpeedYChange()
        {
            float w = (WaterSettings.Max_BumpSpeed - WaterSettings.Min_BumpSpeed);
            material.Settings.BumpSpeed.Y = WaterSettings.Min_BumpSpeed + (sliderList[13].GetValue() * w);
        }

        #endregion Callbacks
    }
}
