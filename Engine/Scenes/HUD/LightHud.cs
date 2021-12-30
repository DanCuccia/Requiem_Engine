using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Engine.Drawing_Objects;
using Engine.Managers;
using Engine.Math_Physics;

namespace Engine.Scenes.HUD
{
    /// <summary>The light hud holds all the functionality needed to edit Lighting parameters</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class LightHud : HudBase
    {
        #region Member Vars

        PointLight              light               = null;
        LineSphere3D            sphere              = new LineSphere3D();

        Rectangle sliderComposition = new Rectangle(1024 + 6 + 92, 408 + 6, 92, 24);

        #endregion Member Vars

        #region Initialization

        /// <summary>Default CTOR</summary>
        public LightHud(EditorScene editor, PointLight pointLight)
            :base(editor)
        {
            this.light = pointLight;
            toggleBulbToggle(light.showBulb);
            this.sphere.Initialize(light.WorldMatrix.Position, light.falloff, Color.FromNonPremultiplied(light.color));
        }

        /// <summary>load buttons</summary>
        /// <param name="content">content manager</param>
        protected override void LoadButtons(ContentManager content)
        {
            SpriteButton btn = new SpriteButton();
            btn.Initialize("sprites//editor//LE_btnToggleBulb");
            btn.setExecution(null, this.callback_toggleBulb);
            btn.Position = new Vector2(Rect.X + 6, Rect.Y + 6);
            base.AddButton(btn);

            btn = new SpriteButton(GameIDList.Button_LightHud_BulbToggle);
            btn.Initialize("sprites//editor//LE_checkBox");
            btn.Position = new Vector2(Rect.X + 6 + (btn.Texture.Width / 2) + 2, Rect.Y + 6);
            btn.ToggleFrames = false;
            base.AddButton(btn);
        }

        /// <summary>load sliders</summary>
        /// <param name="content">content manager</param>
        protected override void LoadSliders(ContentManager content)
        {
            Slider slider;

            slider = new Slider(content, new Vector2(Rect.X + 8, Rect.Y + 42), 256 - 8, "Red Component", onLightColorChange);
            slider.SetValue(light.color.X);
            base.AddSlider(slider);

            slider = new Slider(content, new Vector2(Rect.X + 8, Rect.Y + 42 + 50), 256 - 8, "Green Component", onLightColorChange);
            slider.SetValue(light.color.Y);
            base.AddSlider(slider);

            slider = new Slider(content, new Vector2(Rect.X + 8, Rect.Y + 42 + 50 + 50), 256 - 8, "Blue Component", onLightColorChange);
            slider.SetValue(light.color.Z);
            base.AddSlider(slider);

            slider = new Slider(content, new Vector2(Rect.X + 8, Rect.Y + 55 + 50 + 50 + 50), 256 - 8, "FallOff", onLightFalloffChange);
            slider.SetValue((light.falloff - PointLight.Min_FallOff) / (PointLight.Max_FallOff - PointLight.Min_FallOff));
            base.AddSlider(slider);

            slider = new Slider(content, new Vector2(Rect.X + 8, Rect.Y + 55 + 50 + 50 + 50 + 50), 256 - 8, "Intensity", onLightIntensityChange);
            slider.SetValue((light.intensity - PointLight.Min_Intensity) / (PointLight.Max_Intensity - PointLight.Min_Intensity));
            base.AddSlider(slider);
        }

        /// <summary>load sprites</summary>
        /// <param name="content">content manager</param>
        protected override void LoadSprites(ContentManager content) { }

        #endregion Initialization

        /// <summary>main render3D call</summary>
        public override void Render3D()
        {
            sphere.Render();
        }
        /// <summary>render the composition frame</summary>
        /// <param name="batch">sprite batch</param>
        protected override void Render2DExtra(SpriteBatch batch)
        {
            batch.Draw(blank, sliderComposition, 
                new Color(sliderList[0].GetValue(), sliderList[1].GetValue(), sliderList[2].GetValue()));
        }
        /// <summary>updates the line sphere</summary>
        public override void Update(GameTime time)
        {
            sphere.Initialize(light.WorldMatrix.Position, light.falloff, Color.FromNonPremultiplied(light.color));
        }

        /// <summary>will toggle the light.showBulb whether it will draw or not</summary>
        /// <param name="value">render the light bulb model value</param>
        private void toggleBulbToggle(bool value)
        {
            light.showBulb = value;
            foreach (SpriteButton btn in buttonList)
            {
                if (btn.ID == GameIDList.Button_LightHud_BulbToggle)
                {
                    if (value == true)
                        btn.CurrentFrameX = 1;
                    else btn.CurrentFrameX = 0;
                }
            }
        }

        #region Callbacks

        /// <summary>slider callback</summary>
        public void callback_toggleBulb()
        {
            toggleBulbToggle(!light.showBulb);
        }
        /// <summary>slider callback</summary>
        public void onLightColorChange()
        {
            light.color = new Vector4(sliderList[0].GetValue(), sliderList[1].GetValue(), sliderList[2].GetValue(), 1f);
            light.Update(ref editor.camera, null);
        }
        /// <summary>slider callback</summary>
        public void onLightFalloffChange()
        {
            light.falloff = PointLight.Min_FallOff + (sliderList[3].GetValue() * (PointLight.Max_FallOff - PointLight.Min_FallOff));
        }
        /// <summary>slider callback</summary>
        public void onLightIntensityChange()
        {
            light.intensity = PointLight.Min_Intensity + (sliderList[4].GetValue() * (PointLight.Max_Intensity - PointLight.Min_Intensity));
        }

        #endregion Callbacks
    }
}
