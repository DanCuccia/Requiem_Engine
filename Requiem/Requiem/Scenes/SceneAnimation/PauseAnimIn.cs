using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Scenes.IScene_Animations;
using Engine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Math_Physics;
using Engine.Managers;

namespace Requiem.Scenes.SceneAnimation
{
    /// <summary>animates renderer's blooms settings for the scene during the pause menu</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class PauseAnimIn : CSceneAnimateIn
    {
        const float duration = 2500f;
        float currentDuration = 0f;

        public PauseAnimIn(Scene scene, AnimationCompleteCallback onComplete)
            : base(scene, onComplete)
        { }

        public override void OnAnimateIn(GameTime time)
        {
            currentDuration += time.ElapsedGameTime.Milliseconds;
            if (currentDuration >= duration)
            {
                onComplete();
                this.scene.animateIn = null;
                return;
            }

            float scalarTime = MyMath.GetScalarBetween(0f, duration, currentDuration);
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
        }

        public override void Render2D(SpriteBatch batch) { }
        public override void Render3D() { }
    }
}
