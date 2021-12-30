using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Scenes.IScene_Animations;
using Engine.Scenes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Engine.Math_Physics;
using Engine.Managers;


namespace Requiem.Scenes.SceneAnimation
{
    /// <summary>animates renderer's blooms settings for the scene during the pause menu</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class PauseAnimOut : CSceneAnimateOut
    {
        Renderer renderer;
        const float duration = 2500f;
        float currentDuration = 0f;

        public PauseAnimOut(Scene scene, AnimationCompleteCallback onComplete)
            : base(scene, onComplete)
        {
            renderer = Renderer.getInstance();
        }

        public override void OnAnimateOut(GameTime time)
        {
            float scalarTime = MathHelper.Clamp(1f - MyMath.GetScalarBetween(0f, duration, currentDuration), 0f, 1f);
            Renderer r = Renderer.getInstance();
            int subtle = (int)BloomPresetIndices.BLOOM_SUBTLE, pause = (int)BloomPresetIndices.BLOOM_PAUSE;

            r.BloomSettings.BaseIntensity = MyMath.GetValueBetween(
                BloomSettings.PresetSettings[subtle].BaseIntensity,
                BloomSettings.PresetSettings[pause].BaseIntensity,
                scalarTime);

            r.BloomSettings.BaseSaturation = MyMath.GetValueBetween(
                BloomSettings.PresetSettings[subtle].BaseSaturation,
                BloomSettings.PresetSettings[pause].BaseSaturation,
                scalarTime);

            r.BloomSettings.BloomIntensity = MyMath.GetValueBetween(
                BloomSettings.PresetSettings[subtle].BloomIntensity,
                BloomSettings.PresetSettings[pause].BloomIntensity,
                scalarTime);

            r.BloomSettings.BloomSaturation = MyMath.GetValueBetween(
                BloomSettings.PresetSettings[subtle].BloomSaturation,
                BloomSettings.PresetSettings[pause].BloomSaturation,
                scalarTime);

            r.BloomSettings.BloomThreshold = MyMath.GetValueBetween(
                BloomSettings.PresetSettings[subtle].BloomThreshold,
                BloomSettings.PresetSettings[pause].BloomThreshold,
                scalarTime);

            r.BloomSettings.BlurAmount = MyMath.GetValueBetween(
                BloomSettings.PresetSettings[subtle].BlurAmount,
                BloomSettings.PresetSettings[pause].BlurAmount,
                scalarTime);

            currentDuration += time.ElapsedGameTime.Milliseconds;
            if (currentDuration >= duration && onComplete != null)
            {
                setRendererToDefault();
                onComplete();
            }
        }

        /// <summary>reset renderer's bloom settings to the 'subtle' setting</summary>
        private void setRendererToDefault()
        {
            renderer.BloomSettings = BloomSettings.PresetSettings[(int)BloomPresetIndices.BLOOM_SUBTLE];
        }

        public override void Render2D(SpriteBatch batch) { }
        public override void Render3D() { }
    }
}
