using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#pragma warning disable 1591

namespace Engine.Scenes.IScene_Animations
{
    /// <summary>This is a concrete implementation of Scene Animate Out logic,
    /// This is what you (the dev) inherits from to create new animation out sequences
    /// NOTE: You MUST Call onComplete() within your onAnimate code in order to finish this animation</summary>
    /// <author>Daniel Cuccia</author>
    public class CSceneAnimateOut : ISceneAnimateOut
    {
        protected Scene scene;

        public delegate void AnimationCompleteCallback();
        public AnimationCompleteCallback onComplete;

        public CSceneAnimateOut(Scene scene, AnimationCompleteCallback onComplete)
        {
            this.scene = scene;
            this.onComplete = onComplete;
        }

        public virtual void OnAnimateOut(GameTime time) { }
        public virtual void Render2D(SpriteBatch batch) { }
        public virtual void Render3D() { }
    }
}
