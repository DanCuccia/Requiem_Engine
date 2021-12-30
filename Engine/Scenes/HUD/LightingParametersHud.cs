using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine.Drawing_Objects;
using Engine.Drawing_Objects.Materials;
using Engine.Math_Physics;

namespace Engine.Scenes.HUD
{
    /// <summary>On-screen Hud to edit material's lighting parameters</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class LightingParametersHud : HudBase
    {
        Material material;

        /// <summary>Default CTOR</summary>
        /// <param name="material">pointer to the material we are editing</param>
        /// <param name="editor">level editor pointer</param>
        public LightingParametersHud(EditorScene editor, Material material)
            :base(editor)
        {
            this.material = material;
        }

        /// <summary>load background rect</summary>
        /// <returns></returns>
        protected override Rectangle LoadRectangle()
        {
            return new Rectangle(editor.device.Viewport.Width - 256, 408, 256, 306);
        }

        /// <summary>load slider editors</summary>
        protected override void LoadSliders(ContentManager content)
        {
            Slider slider = new Slider(content, new Vector2(Rect.X, Rect.Y), Rect.Width, "Ambient", this.onAmbientChange);
            slider.SetValue((material.Ambient - Material.MinAmbient) / (Material.MaxAmbient - Material.MinAmbient));
            base.AddSlider(slider);

            slider = new Slider(content, new Vector2(Rect.X, Rect.Y + 50), Rect.Width, "Diffuse", this.onDiffuseChange);
            slider.SetValue((material.Diffuse - Material.MinDiffuse) / (Material.MaxDiffuse - Material.MinDiffuse));
            base.AddSlider(slider);

            slider = new Slider(content, new Vector2(Rect.X, Rect.Y + 50 + 50), Rect.Width, "Specular Level", this.onSpecularChange);
            slider.SetValue((material.Specular - Material.MinSpecular) / (Material.MaxSpecular - Material.MinSpecular));
            base.AddSlider(slider);

            slider = new Slider(content, new Vector2(Rect.X, Rect.Y + 50 + 50 + 50), Rect.Width, "Specular Exponent", this.onSpecularExponentChange);
            slider.SetValue((material.Shine - Material.MinShine) / (Material.MaxShine - Material.MinShine));
            base.AddSlider(slider);
        }

        /// <summary>no buttons in this hud</summary>
        /// <param name="content">content manager</param>
        protected override void LoadButtons(ContentManager content) { }

        /// <summary>load sprites</summary>
        /// <param name="content">content manager</param>
        protected override void LoadSprites(ContentManager content) { }

        #region callbacks
        /// <summary>slider callback</summary>
        public void onAmbientChange()
        {
            material.Ambient = sliderList[0].GetValue() * (Material.MaxAmbient - Material.MinAmbient);
        }
        /// <summary>slider callback</summary>
        public void onDiffuseChange()
        {
            material.Diffuse = sliderList[1].GetValue() * (Material.MaxDiffuse - Material.MinDiffuse);
        }
        /// <summary>slider callback</summary>
        public void onSpecularChange()
        {
            material.Specular = sliderList[2].GetValue() * (Material.MaxSpecular - Material.MinSpecular);
        }
        /// <summary>slider callback</summary>
        public void onSpecularExponentChange()
        {
            material.Shine = sliderList[3].GetValue() * (Material.MaxShine - Material.MinShine);
        }
        #endregion callbacks
    }
}
