using Engine;
using Engine.Managers;
using Engine.Scenes;
using Engine.Scenes.IScene_Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Requiem.Scenes
{
    /// <summary>Second Splash Screen</summary>
    /// <remarks>comments omitted - see Engine for details</remarks>
    /// <author>Daniel Cuccia</author>
    public sealed class SplashScene2 : Scene
    {
        Texture2D splashImage;
        Rectangle rect;
        bool pressed = false;
        Texture2D blank;

        public SplashScene2(Microsoft.Xna.Framework.Game g, string name, SceneManager sm)
            :base(g, name, sm) { }

        public override void Initialize()
        {
            base.animateIn = new SceneAnimateIn_WhiteFade(this, animInOnComplete);
            splashImage = TextureManager.getInstance().GetTexture("sprites//physxSplash");
            blank = TextureManager.getInstance().GetTexture("sprites//blank");
            rect = new Rectangle((int)((device.Viewport.Width * .5f) - (splashImage.Width * .5f)),
                (int)((device.Viewport.Height * .5f) - (splashImage.Height * .5f)),
                splashImage.Width, splashImage.Height);
            isInitialized = true;
            pressed = false;
        }

        private void animInOnComplete()
        {
            base.animateIn = null;
            base.animateOut = new SceneAnimateOut_BlackFade(this, animOutOnComplete);
        }

        private void animOutOnComplete()
        {
            sceneManager.SetCurrentScene("splash3");
        }

        public override void Close()
        {
            isInitialized = false;
            splashImage = null;
            rect = Rectangle.Empty;
            TextureManager.getInstance().ClearLists();
        }

        public override void Input(KeyboardState kb, MouseState ms, GamePadState gp1)
        {
            if (kb.GetPressedKeys().Length > 0)
                pressed = true;

            if (kb.GetPressedKeys().Length == 0 && pressed == true)
                animOutOnComplete();
        }

        public override void Update(GameTime time)
        {
        }

        public override void Render3D(GameTime time)
        {
        }

        public override void RenderDebug3D(GameTime time)
        {
        }

        public override void Render2D(GameTime time, SpriteBatch batch)
        {
            batch.Draw(blank, new Rectangle(0, 0, device.Viewport.Width, device.Viewport.Height), Color.White);
            batch.Draw(splashImage, rect, Color.White);
        }

        public override void RenderDebug2D(GameTime time, SpriteBatch batch)
        {
        }
    }
}
