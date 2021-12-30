using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Scenes.IScene_Animations
{
    /// <summary>This Animation will Softly fade from the current scene to black</summary>
    /// <author>Daniel Cuccia</author>
    public class SceneAnimateOut_BlackFade : CSceneAnimateOut
    {
        Texture2D blank;
        Rectangle rect;

        float currentFade = 0f;
        float fadeSpeed = 0.01f;

        /// <summary> default CTOR </summary> 
        public SceneAnimateOut_BlackFade(Scene scene, AnimationCompleteCallback onComplete)
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
        }

        /// <summary> Draw 2D call (your scene will still render) </summary> 
        public override void Render2D(SpriteBatch batch)
        {
            batch.Draw(blank, rect, Color.FromNonPremultiplied(0, 0, 0, (int)(currentFade * 255)));
        }

        /// <summary>Draw 3D call (your scene will still render)</summary>
        public override void Render3D() { }
    }
}
