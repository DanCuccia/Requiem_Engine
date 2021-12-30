using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Scenes.IScene_Animations;
using Microsoft.Xna.Framework;
using Engine.Managers;
using Microsoft.Xna.Framework.Graphics;
using Engine.Scenes;

namespace Requiem.Scenes.SceneAnimation
{
    /// <summary>animated the renderer properties out of the title scene</summary>
    public sealed class TitleSceneAnimOut : CSceneAnimateOut
    {
        Texture2D blank;
        Rectangle rect;
        Renderer renderer;

        float currentFade = 0f;
        float fadeSpeed = 0.005f;

        public TitleSceneAnimOut(Scene scene, AnimationCompleteCallback onComplete)
            : base(scene, onComplete)
        {
            blank = scene.content.Load<Texture2D>("sprites//blank");
            rect = new Rectangle(0, 0, scene.device.Viewport.Width, scene.device.Viewport.Height);
            renderer = Renderer.getInstance();
        }

        /// <summary>animte out cycle logic</summary>
        /// <param name="time">current game timing values</param>
        public override void OnAnimateOut(GameTime time)
        {
            base.scene.Update(time);

            renderer.BloomSettings.BlurAmount += 0.01f;
            renderer.BloomSettings.BloomThreshold -= 0.01f;
            renderer.BloomSettings.BloomIntensity = Math.Max(
                renderer.BloomSettings.BloomIntensity, renderer.BloomSettings.BloomIntensity + 0.01f);
            renderer.BloomSettings.BloomSaturation += 0.01f;

            currentFade += fadeSpeed;
            if (currentFade >= 1f && onComplete != null)
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

        /// <summary>2D render call</summary>
        /// <param name="batch">xna sprite batch rendering object</param>
        public override void Render2D(SpriteBatch batch) 
        {
            batch.Draw(blank, rect, Color.FromNonPremultiplied(255, 255, 255, (int)(currentFade * 255)));
        }

        /// <summary>render 3D call</summary>
        public override void Render3D() 
        {

        }
    }
}
