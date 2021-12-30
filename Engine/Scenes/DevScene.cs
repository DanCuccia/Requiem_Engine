using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Engine.Drawing_Objects;
using Engine.Managers;
using Engine.Math_Physics;
using Engine.Scenes.IScene_Animations;
#pragma warning disable 1591

namespace Engine.Scenes
{
    /// <summary>The scene that we use, with dev buttons to take us where we need to go, faster</summary>
    /// <author>Daniel Cuccia</author>
    /// <remarks>No comments in this file. you'll find descriptions in base class</remarks>
    public sealed class DevScene : Scene
    {
        Sprite title;
        List<SpriteButton> buttonList = new List<SpriteButton>();

        public DevScene(Microsoft.Xna.Framework.Game g, String name, SceneManager sm)
            :base(g, name, sm) { }

        public override void Initialize()
        {
            title = new Sprite();
            title.Initialize("sprites//devSceneTitle", new Point(1, 1));
            title.Position = new Vector2(device.Viewport.Width - title.FrameSize.X/2, title.FrameSize.Y/2);
            loadButtons();
            base.animateIn = new SceneAnimateIn_BlackFade(this, null);
            this.isInitialized = true;
            sceneManager.Game.IsMouseVisible = true;
        }

        private void loadButtons()
        {
            SpriteButton btn = new SpriteButton();
            btn.Initialize("sprites//blank_square_btn", "Level Editor");
            btn.setExecution(null, this.goto_levelEditor);
            btn.Position = new Vector2(30, 200);
            buttonList.Add(btn);

            //level 1

            Vector2 pos = new Vector2(300, 150);
            float origX = pos.X, origY = pos.Y;

            btn = new SpriteButton();
            btn.Initialize("sprites//blank_square_btn", "Level 1 - a");
            btn.setExecution(null, this.goto_level1a);
            btn.Position = pos;
            buttonList.Add(btn);

            pos.Y += 128;

            btn = new SpriteButton();
            btn.Initialize("sprites//blank_square_btn", "Level 1 - b");
            btn.setExecution(null, this.goto_level1b);
            btn.Position = pos;
            buttonList.Add(btn);

            pos.Y += 128;

            btn = new SpriteButton();
            btn.Initialize("sprites//blank_square_btn", "Level 1 - c");
            btn.setExecution(null, this.goto_level1c);
            btn.Position = pos;
            buttonList.Add(btn);

            pos.Y += 128;

            btn = new SpriteButton();
            btn.Initialize("sprites//blank_square_btn", "Level 1 - d");
            btn.setExecution(null, this.goto_level1d);
            btn.Position = pos;
            buttonList.Add(btn);

            pos.Y += 128;

            btn = new SpriteButton();
            btn.Initialize("sprites//blank_square_btn", "Level 1 - e");
            btn.setExecution(null, this.goto_level1e);
            btn.Position = pos;
            buttonList.Add(btn);
        }

        public override void Close()
        {
            title = null;
            buttonList.Clear();
            TextureManager.getInstance().ClearLists();
        }

        public override void Input(KeyboardState kb, MouseState ms, GamePadState gp1)
        {
            MyUtility.ProcessButtonList(ms, buttonList);
        }

        public override void Update(GameTime time)
        {
            foreach (SpriteButton btn in buttonList)
                btn.Update(time);
        }

        public override void Render3D(GameTime time)
        {        }

        public override void RenderDebug3D(GameTime time)
        {        }

        public override void Render2D(GameTime time, SpriteBatch batch)
        {
            title.Draw(batch);
            foreach (SpriteButton btn in buttonList)
                btn.Draw(batch);
        }

        public override void RenderDebug2D(GameTime time, SpriteBatch batch)
        {        }

        #region CallBacks

        public void goto_levelEditor()
        {
            this.animateOut = new SceneAnimateOut_BlackFade(this, animateOut_gotoEditor);
        }

        #region level 1
        public void goto_level1a()
        {
            base.sceneManager.SetCurrentScene("level1a");
        }
        public void goto_level1b()
        {
            base.sceneManager.SetCurrentScene("level1b");
        }
        public void goto_level1c()
        {
            base.sceneManager.SetCurrentScene("level1c");
        }
        public void goto_level1d()
        {
            base.sceneManager.SetCurrentScene("level1d");
        }
        public void goto_level1e()
        {
            base.sceneManager.SetCurrentScene("level1e");
        }
        #endregion

        public void animateIn_onComplete()
        {
            this.animateIn = null;
        }
        public void animateOut_gotoEditor()
        {
            animateOut = null;
            base.sceneManager.SetCurrentScene("editor");
        }
        #endregion CallBacks
    }
}
