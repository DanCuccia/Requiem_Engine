using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine.Drawing_Objects;
using Engine.Drawing_Objects.Materials;
using Engine.Math_Physics;
#pragma warning disable 1591

namespace Engine.Scenes.HUD
{
    /// <summary>Parallax Occlusion Mapping material editing UI</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class POMHud : HudBase
    {
        ParallaxOcclusionMaterial material;

        /// <summary>Default CTOR</summary>
        /// <param name="material">pointer to the material we are editing</param>
        /// <param name="editor">level editor</param>
        public POMHud(EditorScene editor, Material material)
            :base(editor)
        {
            if (material.GetType() != typeof(ParallaxOcclusionMaterial))
                throw new InvalidOperationException("POMHud::POMHud - input material is not ParralaxOcclusionMaterial");

            this.material = (ParallaxOcclusionMaterial)material;
        }

        /// <summary>load slider editors</summary>
        /// <param name="content">cotnent manangererere</param>
        protected override void LoadSliders(ContentManager content)
        {
            Slider slider = new Slider(content, new Vector2(Rect.X, Rect.Y), Rect.Width, 
                "Tex Repeat - X", this.onTexRepeatXChange);
            slider.SetValue((material.TextureRepeatX - ParallaxOcclusionMaterial.Min_TextureRepeat) / 
                (ParallaxOcclusionMaterial.Max_TextureRepeat - ParallaxOcclusionMaterial.Min_TextureRepeat));
            sliderList.Add(slider);

            slider = new Slider(content, new Vector2(Rect.X, Rect.Y + 50), Rect.Width, 
                "Tex Repeat - Y", this.onTexRepeatYChange);
            slider.SetValue((material.TextureRepeatY - ParallaxOcclusionMaterial.Min_TextureRepeat) /
                (ParallaxOcclusionMaterial.Max_TextureRepeat - ParallaxOcclusionMaterial.Min_TextureRepeat));
            sliderList.Add(slider);

            slider = new Slider(content, new Vector2(Rect.X, Rect.Y + 50 + 50), Rect.Width, 
                "Extrusion Displacement", this.onExtrusionHeightChange);
            slider.SetValue((material.ExtrusionDisplacement - ParallaxOcclusionMaterial.Min_Extrusion) /
                (ParallaxOcclusionMaterial.Max_Extrusion - ParallaxOcclusionMaterial.Min_Extrusion));
            sliderList.Add(slider);

            slider = new Slider(content, new Vector2(Rect.X, Rect.Y + 50 + 50 + 50), Rect.Width, 
                "Extrusion Depth", this.onExtrusionDepthChange);
            slider.SetValue((material.ExtrusionDepth - ParallaxOcclusionMaterial.Min_Depth) /
                (ParallaxOcclusionMaterial.Max_Depth - ParallaxOcclusionMaterial.Min_Depth));
            sliderList.Add(slider);

            slider = new Slider(content, new Vector2(Rect.X, Rect.Y + 50 + 50 + 50 + 50), Rect.Width, 
                "Specular Level", this.onSpecularLevelChange);
            slider.SetValue((material.Specular - ParallaxOcclusionMaterial.Min_SpecularLevel) /
                (ParallaxOcclusionMaterial.Max_SpecularLevel - ParallaxOcclusionMaterial.Min_SpecularLevel));
            sliderList.Add(slider);

            slider = new Slider(content, new Vector2(Rect.X, Rect.Y + 50 + 50 + 50 + 50 + 50), Rect.Width, 
                "Specular Exponent", this.onSpecularExponentChange);
            slider.SetValue((material.Shine - ParallaxOcclusionMaterial.Min_SpecularExponent) /
                (ParallaxOcclusionMaterial.Max_SpecularExponent - ParallaxOcclusionMaterial.Min_SpecularExponent));
            sliderList.Add(slider);
        }

        /// <summary>unused</summary>
        /// <param name="content">content manager</param>
        protected override void LoadButtons(ContentManager content) { }
        /// <summary>unused</summary>
        /// <param name="content">content manager</param>
        protected override void LoadSprites(ContentManager content) { }

        #region callbacks
        public void onTexRepeatXChange()
        {
            material.TextureRepeatX = sliderList[0].GetValue() * (ParallaxOcclusionMaterial.Max_TextureRepeat - ParallaxOcclusionMaterial.Min_TextureRepeat);
        }

        public void onTexRepeatYChange()
        {
            material.TextureRepeatY = sliderList[1].GetValue() * (ParallaxOcclusionMaterial.Max_TextureRepeat - ParallaxOcclusionMaterial.Min_TextureRepeat);
        }

        public void onExtrusionHeightChange()
        {
            material.ExtrusionDisplacement = sliderList[2].GetValue() * (ParallaxOcclusionMaterial.Max_Extrusion - ParallaxOcclusionMaterial.Min_Extrusion);
        }

        public void onExtrusionDepthChange()
        {
            material.ExtrusionDepth = sliderList[3].GetValue() * (ParallaxOcclusionMaterial.Max_Depth - ParallaxOcclusionMaterial.Min_Depth);
        }

        public void onSpecularLevelChange()
        {
            material.Specular = sliderList[4].GetValue() * (ParallaxOcclusionMaterial.Max_SpecularLevel - ParallaxOcclusionMaterial.Min_SpecularLevel);
        }

        public void onSpecularExponentChange()
        {
            material.Shine = sliderList[5].GetValue() * (ParallaxOcclusionMaterial.Max_SpecularExponent - ParallaxOcclusionMaterial.Min_SpecularExponent);
        }
        #endregion callbacks
    }
}
