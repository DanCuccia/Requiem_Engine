using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine.Drawing_Objects;
using Engine.Drawing_Objects.ParticleSystems;
using Engine.Managers;
using Engine.Math_Physics;
using Microsoft.Xna.Framework.Content;
#pragma warning disable 1591

namespace Engine.Scenes.HUD
{
    /// <summary>Used in Level Editor to edit Mega-Particles in real-time</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class MegaParticleHud : HudBase
    {
        #region Member Vars
        ParticleSystem          pSystem     = null;
        Rectangle               composite   = new Rectangle(632, 406, 128, 32);
        Rectangle               compositeBkg= new Rectangle(626, 400, 140, 44);
        #endregion

        #region Init
        /// <summary>Default CTOR</summary>
        /// <param name="particleSystem">the currently worked particle system</param>
        /// <param name="editor">reference to the level editor scene</param>
        public MegaParticleHud(EditorScene editor, ParticleSystem particleSystem)
            :base(editor)
        {
            this.pSystem = particleSystem;
        }

        /// <summary>load rect</summary>
        /// <returns>background rect</returns>
        protected override Rectangle LoadRectangle()
        {
            return new Rectangle(editor.device.Viewport.Width - 768, 400, 768, 400);
        }

        /// <summary>load sliders</summary>
        protected override void LoadSliders(ContentManager content)
        {
            Slider slider;
            //COLOR
            slider = new Slider(editor.content, new Vector2(Rect.X + 8, Rect.Y + 42), 256 - 8, "Red Component", onColorChange);
            slider.SetValue((float)pSystem.ParticleColor.R / 255);
            base.AddSlider(slider);

            slider = new Slider(editor.content, new Vector2(Rect.X + 8, Rect.Y + 42 + 50), 256 - 8, "Green Component", onColorChange);
            slider.SetValue((float)pSystem.ParticleColor.G / 255);
            base.AddSlider(slider);

            slider = new Slider(editor.content, new Vector2(Rect.X + 8, Rect.Y + 42 + 50 + 50), 256 - 8, "Blue Component", onColorChange);
            slider.SetValue((float)pSystem.ParticleColor.B / 255);
            base.AddSlider(slider);

            //SCALING
            slider = new Slider(editor.content, new Vector2(Rect.X + 8, Rect.Y + 234), 256 - 8, "Minumum Spawn Scale", onScalingChange);
            slider.SetValue(MyMath.GetScalarBetween(EmissionSettings.ScaleMin, EmissionSettings.ScaleMax, pSystem.emissionSettings.ScaleSpawnMin));
            base.AddSlider(slider); ;

            slider = new Slider(editor.content, new Vector2(Rect.X + 8, Rect.Y + 234 + 96), 256 - 8, "Maximum Spawn Scale", onScalingChange);
            slider.SetValue(MyMath.GetScalarBetween(EmissionSettings.ScaleMin, EmissionSettings.ScaleMax, pSystem.emissionSettings.ScaleSpawnMax));
            base.AddSlider(slider);

            //GRAVITY
            slider = new Slider(editor.content, new Vector2(Rect.X + 4 + 256, Rect.Y + 42), 256 - 8, "Gravity - X", onGravityChange);
            slider.SetValue((pSystem.emissionSettings.Gravity.X - EmissionSettings.GravityMin) / (EmissionSettings.GravityMax - EmissionSettings.GravityMin));
            base.AddSlider(slider);

            slider = new Slider(editor.content, new Vector2(Rect.X + 4 + 256, Rect.Y + 42 + 50), 256 - 8, "Gravity - Y", onGravityChange);
            slider.SetValue((pSystem.emissionSettings.Gravity.Y - EmissionSettings.GravityMin) / (EmissionSettings.GravityMax - EmissionSettings.GravityMin));
            base.AddSlider(slider);

            slider = new Slider(editor.content, new Vector2(Rect.X + 4 + 256, Rect.Y + 42 + 50 + 50), 256 - 8, "Gravity - Z", onGravityChange);
            slider.SetValue((pSystem.emissionSettings.Gravity.Z - EmissionSettings.GravityMin) / (EmissionSettings.GravityMax - EmissionSettings.GravityMin));
            base.AddSlider(slider);

            //TURBULENCE
            slider = new Slider(editor.content, new Vector2(Rect.X + 4 + 512, Rect.Y + 24), 256 - 4, "Turbulence Effect", onTurbulenceChange);
            slider.SetValue(MyMath.GetScalarBetween(EmissionSettings.VelocityVarianceMin, EmissionSettings.VelocityVarianceMax, pSystem.emissionSettings.InitialVelocityVariance));
            base.AddSlider(slider);

            slider = new Slider(editor.content, new Vector2(Rect.X + 4 + 512, Rect.Y + 24 + 48), 256 - 4, "Turbulence - X", onTurbulenceChange);
            slider.SetValue(MyMath.GetScalarBetween(EmissionSettings.TurbulenceMin, EmissionSettings.TurbulenceMax, pSystem.emissionSettings.InitialTurbulance.X));
            base.AddSlider(slider);

            slider = new Slider(editor.content, new Vector2(Rect.X + 4 + 512, Rect.Y + 24 + 48 + 48), 256 - 4, "Turbulence - Y", onTurbulenceChange);
            slider.SetValue(MyMath.GetScalarBetween(EmissionSettings.TurbulenceMin, EmissionSettings.TurbulenceMax, pSystem.emissionSettings.InitialTurbulance.Y));
            base.AddSlider(slider);

            slider = new Slider(editor.content, new Vector2(Rect.X + 4 + 512, Rect.Y + 24 + 48 + 48 + 48), 256 - 4, "Turbulence - Z", onTurbulenceChange);
            slider.SetValue(MyMath.GetScalarBetween(EmissionSettings.TurbulenceMin, EmissionSettings.TurbulenceMax, pSystem.emissionSettings.InitialTurbulance.Z));
            base.AddSlider(slider);

            //SPAWN BOX
            slider = new Slider(editor.content, new Vector2(Rect.X + 4 + 256, Rect.Y + 212 + 24), 256 - 4, "Spawn Box - X", onSpawnBoxChange);
            slider.SetValue(MyMath.GetScalarBetween(EmissionSettings.SpawnBoxMin, EmissionSettings.SpawnBoxMax, pSystem.emissionSettings.SpawnBox.X));
            base.AddSlider(slider);

            slider = new Slider(editor.content, new Vector2(Rect.X + 4 + 256, Rect.Y + 212 + 24 + 48), 256 - 4, "Spawn Box - Y", onSpawnBoxChange);
            slider.SetValue(MyMath.GetScalarBetween(EmissionSettings.SpawnBoxMin, EmissionSettings.SpawnBoxMax, pSystem.emissionSettings.SpawnBox.Y));
            base.AddSlider(slider);

            slider = new Slider(editor.content, new Vector2(Rect.X + 4 + 256, Rect.Y + 212 + 24 + 48 + 48), 256 - 4, "Spawn Box - Z", onSpawnBoxChange);
            slider.SetValue(MyMath.GetScalarBetween(EmissionSettings.SpawnBoxMin, EmissionSettings.SpawnBoxMax, pSystem.emissionSettings.SpawnBox.Z));
            base.AddSlider(slider);

            //LIFE
            slider = new Slider(editor.content, new Vector2(Rect.X + 4 + 512, Rect.Y + 212 + 24), 256 - 4, "Maximum Life", onMaxLifeChange);
            slider.SetValue(MyMath.GetScalarBetween(EmissionSettings.MaxLifeMin, EmissionSettings.MaxLifeMax, pSystem.emissionSettings.MaximumLife));
            base.AddSlider(slider);

            slider = new Slider(editor.content, new Vector2(Rect.X + 4 + 512, Rect.Y + 212 + 24 + 48), 256 - 4, "Maximum Distance", onMaxDistChange);
            slider.SetValue(MyMath.GetScalarBetween(EmissionSettings.MaxDistMin, EmissionSettings.MaxDistMax, pSystem.emissionSettings.MaximumDistance));
            base.AddSlider(slider);

            slider = new Slider(editor.content, new Vector2(Rect.X + 4 + 512, Rect.Y + 212 + 24 + 48 + 48), 256 - 4, "Emmision Rate", onEmissionChange);
            slider.SetValue(MyMath.GetScalarBetween(EmissionSettings.EmissionRateMin, EmissionSettings.EmissionRateMax, pSystem.emissionSettings.EmissionRate));
            base.AddSlider(slider);
        }

        /// <summary>reset the sliders to .5f</summary>
        public void resetSlidersToNewObject()
        {
            sliderList[5].SetValue(.5f);
            sliderList[6].SetValue(.5f);
            sliderList[7].SetValue(.5f);
        }

        /// <summary>load buttons</summary>
        protected override void LoadButtons(ContentManager content)
        {
            SpriteButton btn = new SpriteButton(GameIDList.Button_MegaHud_UseGravity);
            btn.Initialize("sprites//editor//LE_checkBox");
            btn.Position = new Vector2(Rect.X + 256 + 128, Rect.Y + 6);
            btn.setExecution(null, toggleGravity);
            btn.CurrentFrameX = pSystem.emissionSettings.UseGravity ? 1 : 0;
            btn.ToggleFrames = false;
            base.AddButton(btn);
        }

        /// <summary>load sprites to this hud</summary>
        protected override void LoadSprites(ContentManager content)
        {
            Sprite sprite = new Sprite();
            sprite.Initialize("sprites//editor//LE_colorBkg", new Point(1, 1));
            sprite.Position = new Vector2(Rect.X + 128, Rect.Y + 106);
            spriteList.Add(sprite);

            sprite = new Sprite();
            sprite.Initialize("sprites//editor//LE_gravityBkg", new Point(1, 1));
            sprite.Position = new Vector2(Rect.X + 256 + 128, Rect.Y + 106);
            spriteList.Add(sprite);

            sprite = new Sprite();
            sprite.Initialize("sprites//editor//LE_turbulenceBkg", new Point(1, 1));
            sprite.Position = new Vector2(Rect.X + 256 + 256 + 128, Rect.Y + 106);
            spriteList.Add(sprite);

            sprite = new Sprite();
            sprite.Initialize("sprites//editor//LE_spawnboxBkg", new Point(1, 1));
            sprite.Position = new Vector2(Rect.X + 256 + 128, Rect.Y + 106 + 212);
            spriteList.Add(sprite);

            sprite = new Sprite();
            sprite.Initialize("sprites//editor//LE_particleSizeBkg", new Point(1, 1));
            sprite.Position = new Vector2(Rect.X + 128, Rect.Y + 106 + 212);
            spriteList.Add(sprite);
        }

        #endregion

        #region Callbacks
        public void onColorChange()
        {
            pSystem.ParticleColor = new Color(sliderList[0].GetValue(), sliderList[1].GetValue(), sliderList[2].GetValue(), 1);
        }
        public void onScalingChange()
        {
            float min = MyMath.GetValueBetween(EmissionSettings.ScaleMin, EmissionSettings.ScaleMax, sliderList[3].GetValue());
            float max = MyMath.GetValueBetween(EmissionSettings.ScaleMin, EmissionSettings.ScaleMax, sliderList[4].GetValue());

            if (max < min)
            {
                min = max;
                sliderList[3].SetValue(MyMath.GetScalarBetween(EmissionSettings.ScaleMin, EmissionSettings.ScaleMax, min));
            }
            else if (min > max)
            {
                max = min;
                sliderList[4].SetValue(MyMath.GetScalarBetween(EmissionSettings.ScaleMin, EmissionSettings.ScaleMax, max));
            }

            pSystem.emissionSettings.ScaleSpawnMin = min;
            pSystem.emissionSettings.ScaleSpawnMax = max;
        }
        public void toggleGravity()
        {
            bool value = !pSystem.emissionSettings.UseGravity;
            pSystem.emissionSettings.UseGravity = value;

            foreach (SpriteButton btn in buttonList)
            {
                if (btn.ID == GameIDList.Button_MegaHud_UseGravity)
                {
                    if (value == true)
                        btn.CurrentFrameX = 0;
                    else btn.CurrentFrameX = 1;
                    return;
                }
            }
        }
        public void onGravityChange()
        {
            float w = EmissionSettings.GravityMax - EmissionSettings.GravityMin;
            pSystem.emissionSettings.Gravity.X = EmissionSettings.GravityMin + (sliderList[5].GetValue() * w);
            pSystem.emissionSettings.Gravity.Y = EmissionSettings.GravityMin + (sliderList[6].GetValue() * w);
            pSystem.emissionSettings.Gravity.Z = EmissionSettings.GravityMin + (sliderList[7].GetValue() * w);
        }
        public void onTurbulenceChange()
        {
            pSystem.emissionSettings.InitialVelocityVariance = sliderList[8].GetValue();
            pSystem.emissionSettings.InitialTurbulance.X = MyMath.GetValueBetween(EmissionSettings.TurbulenceMin, EmissionSettings.TurbulenceMax, sliderList[9].GetValue());
            pSystem.emissionSettings.InitialTurbulance.Y = MyMath.GetValueBetween(EmissionSettings.TurbulenceMin, EmissionSettings.TurbulenceMax, sliderList[10].GetValue());
            pSystem.emissionSettings.InitialTurbulance.Z = MyMath.GetValueBetween(EmissionSettings.TurbulenceMin, EmissionSettings.TurbulenceMax, sliderList[11].GetValue());
        }
        public void onSpawnBoxChange()
        {
            pSystem.emissionSettings.SpawnBox.X = MyMath.GetValueBetween(EmissionSettings.SpawnBoxMin, EmissionSettings.SpawnBoxMax, sliderList[12].GetValue());
            pSystem.emissionSettings.SpawnBox.Y = MyMath.GetValueBetween(EmissionSettings.SpawnBoxMin, EmissionSettings.SpawnBoxMax, sliderList[13].GetValue());
            pSystem.emissionSettings.SpawnBox.Z = MyMath.GetValueBetween(EmissionSettings.SpawnBoxMin, EmissionSettings.SpawnBoxMax, sliderList[14].GetValue());
            pSystem.GenerateBoundingBox();
        }
        public void onMaxLifeChange()
        {
            pSystem.emissionSettings.MaximumLife = MyMath.GetValueBetween(EmissionSettings.MaxLifeMin, EmissionSettings.MaxLifeMax, sliderList[15].GetValue());
            pSystem.ReInitializeInstructions();
        }
        public void onMaxDistChange()
        {
            pSystem.emissionSettings.MaximumDistance = MyMath.GetValueBetween(EmissionSettings.MaxDistMin, EmissionSettings.MaxDistMax, sliderList[16].GetValue());
        }
        public void onEmissionChange()
        {
            pSystem.emissionSettings.EmissionRate = MyMath.GetValueBetween(EmissionSettings.EmissionRateMin, EmissionSettings.EmissionRateMax, sliderList[17].GetValue());
        }
        #endregion Callbacks

        #region API

        /// <summary>draws misc hud items</summary>
        /// <param name="batch">sprite batch</param>
        protected override void Render2DExtra(SpriteBatch batch)
        {
            batch.Draw(blank, compositeBkg, Color.Black);
            batch.Draw(blank, composite, new Color(sliderList[0].GetValue(), sliderList[1].GetValue(), sliderList[2].GetValue()));
        }

        public ParticleSystem ParticleSystem
        {
            get { return this.pSystem; }
        }
        #endregion API

    }
}
