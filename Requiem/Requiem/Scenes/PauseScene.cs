using System;
using System.Collections.Generic;
using Engine.Drawing_Objects;
using Engine.Managers;
using Engine.Math_Physics;
using Engine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Requiem.Scenes.SceneAnimation;

namespace Requiem.Scenes
{
    /// <summary>the pause scene during gameplay</summary>
    /// <author>Gabriel Dubois, Daniel Cuccia</author>
    public sealed class PauseScene : Scene
    {
        Sprite background;
        Sprite lable;
        Sprite aButton;
        Sprite bButton;
        List<SpriteButton> buttonList;

        public PauseScene(Game g, String name, SceneManager sm)
            : base(g, name, sm) { }

        public override void Initialize()
        {
            Rectangle rect = sceneManager.Game.Window.ClientBounds;
            buttonList = new List<SpriteButton>();

            background = new Sprite();
            background.Initialize("Sprites/PauseBackground", new Point(1, 1));
            background.Position = new Vector2(rect.Width * .5f, rect.Height * .5f);

            lable = new Sprite();
            lable.Initialize("Sprites/PauseLable", new Point(1, 1));
            lable.Position = new Vector2(background.PositionX + 50, 100);

            SpriteButton s = new SpriteButton();
            s.InitializeSpriteButton("sprites//ResumeButton", new Point(1, 1));
            s.Position = new Vector2(background.PositionX - 200, rect.Height * .5f);
            s.setExecution(null, ResumeGame);
            buttonList.Add(s);

            aButton = new Sprite();
            aButton.Initialize("Sprites//AButton", new Point(1, 1));
            aButton.Position = new Vector2(s.Position.X + 100, s.Position.Y + 60);
            aButton.Scale = .2f;

            s = new SpriteButton();
            s.InitializeSpriteButton("sprites//QuitButton", new Point(1, 1));
            s.Position = new Vector2(background.PositionX + 100, rect.Height * .5f);
            s.setExecution(null, QuitGame);
            buttonList.Add(s);

            bButton = new Sprite();
            bButton.Initialize("Sprites//BButton", new Point(1, 1));
            bButton.Position = new Vector2(s.Position.X + 100, s.Position.Y + 60);
            bButton.Scale = .2f;

            isInitialized = true;
        }

        private void ResumeGame()
        {
            base.sceneManager.CloseOverlapState();
        }

        private void QuitGame()
        {
            sceneManager.CloseOverlapState();
            base.sceneManager.SetCurrentScene("main");
            AudioManager.Instance.StopAll();
            AudioManager.Instance.ClearAll();
            AudioManager.Instance.LoadSound("audio//AshielfPi", "music");
            AudioManager.Instance.Play2DSound("music");
        }

        public override void Close() 
        {
            lable = null;
            buttonList.Clear();
            buttonList = null;
            background = null;
            isInitialized = false;
        }

        public override void Update(GameTime time)
        {
            background.Update(time);
            lable.Update(time);
            aButton.Update(time);
            bButton.Update(time);
            foreach (SpriteButton s in buttonList)
            {
                s.Update(time);
            }
        }

        public override void Input(KeyboardState kb, MouseState ms, GamePadState gp1)
        {
            MyUtility.ProcessButtonList(ms, buttonList);

            if (gp1.IsButtonDown(Buttons.A))
            {
                ResumeGame();
            }

            if (gp1.IsButtonDown(Buttons.B))
            {
                QuitGame();
            }
        }

        public override void Render2D(GameTime time, SpriteBatch batch)
        {
            background.Draw(batch);
            lable.Draw(batch);
            aButton.Draw(batch);
            bButton.Draw(batch);
            foreach (SpriteButton s in buttonList)
            {
                s.Draw(batch);
            }
        }

        public override void RenderDebug2D(GameTime time, SpriteBatch batch) { }
        public override void Render3D(GameTime time) { }
        public override void RenderDebug3D(GameTime time) { }
    }
}
