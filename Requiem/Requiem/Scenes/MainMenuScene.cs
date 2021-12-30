using System.Collections.Generic;
using Engine;
using Engine.Drawing_Objects;
using Engine.Managers;
using Engine.Managers.Camera;
using Engine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Engine.Math_Physics;

namespace Requiem.Scenes
{
    /// <summary>Main Menu Scene</summary>
    /// <remarks>Function comments omitted - see Engine file for details</remarks>
    /// <author>Daniel Cuccia</author>
    public sealed class MainMenuScene : Scene
    {
        Sprite background;
        Sprite title;
        Sprite aButton;
        Sprite bButton;

        List<SpriteButton> buttonList;

        //hacks to fix a bug (coming from the pause menu, 'B' is pressed, which is also what is pressed to quit the game from here
        bool canTakeInput = false;

        public MainMenuScene(Microsoft.Xna.Framework.Game g, string name, SceneManager sm)
            : base(g, name, sm) { }

        public override void Initialize()
        {
            buttonList = new List<SpriteButton>();
            loadDrawables();
            animateIn = new Engine.Scenes.IScene_Animations.SceneAnimateIn_WhiteFade(this, fadeInComplete);
            isInitialized = true;

            GamePadState gp = GamePad.GetState(PlayerIndex.One);
            bool gpIsPressed = false;
            if(gp.IsConnected)
            {
                if (MyUtility.GamePadPressedKeyCount(gp) > 0)
                    gpIsPressed = true;
            }
            if (Keyboard.GetState().GetPressedKeys().Length > 0)
                canTakeInput = false;
            else
            {
                if(gpIsPressed == false)
                    canTakeInput = true;
            }
        }

        private void fadeInComplete() { }

        private void loadDrawables()
        {
            Rectangle rect = sceneManager.Game.Window.ClientBounds;

            background = new Sprite();
            background.Initialize("Sprites//Menu_main", new Point(1, 1));
            background.Position = new Vector2(rect.Width * .5f, rect.Height * .5f);

            title = new Sprite();
            title.Initialize("Sprites//Title_main", new Point(1, 1));
            title.Position = new Vector2(background.PositionX + 50, 150);

            SpriteButton s = new SpriteButton();
            s.Initialize("sprites//Start_main", new Point(1, 1));
            s.Position = new Vector2(background.PositionX - 400, rect.Height * .5f);
            s.setExecution(null, StartGame);
            buttonList.Add(s);

            aButton = new Sprite();
            aButton.Initialize("Sprites//AButton", new Point(1, 1));
            aButton.Position = new Vector2(s.Position.X + 150, s.Position.Y + 150);
            aButton.Scale = .2f;

            s = new SpriteButton();
            s.Initialize("sprites//Quit_main", new Point(1, 1));
            s.Position = new Vector2(background.PositionX + 100, rect.Height * .5f - 20);
            s.setExecution(null, QuitGame);
            buttonList.Add(s);

            bButton = new Sprite();
            bButton.Initialize("Sprites//BButton", new Point(1, 1));
            bButton.Position = new Vector2(s.Position.X + 125, s.Position.Y + 160);
            bButton.Scale = .2f;
        }

        public override void Close()
        {
            buttonList.Clear();
            buttonList = null;
            AudioManager.Instance.StopAll();
            AudioManager.Instance.ClearAll();
            canTakeInput = false;
        }

        public override void Input(KeyboardState kb, MouseState ms, GamePadState gp1)
        {
            if (canTakeInput == false)
            {
                if (kb.GetPressedKeys().Length == 0 &&
                    MyUtility.GamePadPressedKeyCount(gp1) == 0)
                    canTakeInput = true;
            }

            MyUtility.ProcessButtonList(ms, buttonList);

            if (canTakeInput == true)
            {
                if (gp1.IsButtonDown(Buttons.A))
                    StartGame();
                if (gp1.IsButtonDown(Buttons.B))
                    QuitGame();
            }
        }

        /// <summary>Update</summary>
        public override void Update(GameTime time)
        {
            background.Update(time);
            title.Update(time);
            aButton.Update(time);
            bButton.Update(time);
            foreach (SpriteButton s in buttonList)
            {
                s.Update(time);
            }
        }

        public override void Render2D(GameTime time, SpriteBatch batch)
        {
            background.Draw(batch);
            title.Draw(batch);
            aButton.Draw(batch);
            bButton.Draw(batch);

            foreach (SpriteButton s in buttonList)
            {
                s.Draw(batch);
            }
        }

        public void StartGame()
        {
            base.sceneManager.SetCurrentScene("level1e");
        }

        public void QuitGame()
        {
            base.sceneManager.ForceClosed();
        }

        public override void RenderDebug2D(GameTime time, SpriteBatch batch) { }
        public override void Render3D(GameTime time) { }
        public override void RenderDebug3D(GameTime time) { }
    }
}
