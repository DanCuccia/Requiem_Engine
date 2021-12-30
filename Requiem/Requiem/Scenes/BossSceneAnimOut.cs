using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Scenes.IScene_Animations;
using Engine.Scenes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Requiem.Scenes
{
    public sealed class BossSceneAnimOut : CSceneAnimateOut
    {
        Texture2D blank;
        Rectangle rect;

        float currentFade = 0f;
        float fadeSpeed = 0.005f;

        /// <summary> default CTOR </summary> 
        public BossSceneAnimOut(Scene scene, AnimationCompleteCallback onComplete)
            : base(scene, onComplete)
        {
            blank = scene.content.Load<Texture2D>("sprites//blank");
            rect = new Rectangle(0, 0, scene.device.Viewport.Width, scene.device.Viewport.Height);
        }

        /// <summary> update replacement logic </summary> 
        public override void OnAnimateOut(GameTime time)
        {
            currentFade += fadeSpeed;

            if (currentFade > 1f && onComplete != null)
                onComplete();

            scene.Update(time);
        }

        /// <summary> Draw 2D call (your scene will still render) </summary> 
        public override void Render2D(SpriteBatch batch)
        {
            batch.Draw(blank, rect, Color.FromNonPremultiplied(255, 255, 255, (int)(currentFade * 255)));
        }

        /// <summary>Draw 3D call (your scene will still render)</summary>
        public override void Render3D() { }
    }
}
