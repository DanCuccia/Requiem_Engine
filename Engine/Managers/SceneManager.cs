using System;
using System.Collections.Generic;
using Engine.Managers.Factories;
using Engine.Math_Physics;
using Engine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Engine.Managers
{
    /// <summary>The Major head honcho of the Game
    /// This manager controls what scene has focus</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class SceneManager
    {
        #region Member Variables

        List<Scene>             sceneList;
        Scene                   currentScene;
        Microsoft.Xna.Framework.Game thisGame;
        Scene                   overlapScene;
        AudioManager            audio;
        SpriteBatch             spritebatch;
        Renderer                renderer;
        GraphicsDeviceManager   deviceManager;
        FPSDisplay              fps;
        
        #endregion Member Variables

        #region Initialization

        /// <summary>Default CTOR</summary>
        /// <param name="g">the game</param>
        /// <param name="batch">game spriteBatch</param>
        /// <param name="deviceManager">game device manager</param>
        public SceneManager(Microsoft.Xna.Framework.Game g, SpriteBatch batch, GraphicsDeviceManager deviceManager)
        {
            sceneList = new List<Scene>();
            currentScene = null;
            thisGame = g;
            this.spritebatch = batch;
            this.deviceManager = deviceManager;
            audio = AudioManager.Instance;

            this.AddScene(new EditorScene(thisGame, "editor", this));
            this.AddScene(new DevScene(thisGame, "dev", this));
            this.AddScene(new VideoScene(thisGame, "video", this));

            this.SetCurrentScene("dev");

            TriggerFactory triggerFactory = TriggerFactory.getInstance();
            triggerFactory.Initialize(g.Content, this);

            renderer = Renderer.getInstance();

            if (EngineFlags.drawDevelopment == true)
                fps = new FPSDisplay();
        }

        /// <summary>dump all scenes</summary>
        public void Empty()
        {
            sceneList.Clear();
        }

        #endregion Initialization

        #region API

        /// <summary>get the current state</summary>
        /// <returns>the current state, null or not</returns>
        public Scene GetCurrentScene()
        {
            return currentScene;
        }

        /// <summary>Change the current state</summary>
        /// <param name="name">id of the new state to enter</param>
        /// <param name="unloadCurrent">whether or not to unload the current content</param>
        public void SetCurrentScene(String name, bool unloadCurrent = true)
        {
            if (overlapScene != null)
            {
                overlapScene.Close();
                overlapScene = null;
            }

            for (int index = 0; index < sceneList.Count; index++)
            {
                if (sceneList[index].name == name)
                {
                    if (unloadCurrent)
                    {
                        this.Close();
                    }

                    currentScene = sceneList[index];
                    if (!currentScene.isInitialized)
                        currentScene.Initialize();

                    return;
                }
            }
        }

        /// <summary>Open a new state above the current state, updating and drawing both</summary>
        /// <param name="name">the id of the state to show</param>
        public void ShowOverlapScene(String name)
        {
            for (int i = 0; i < sceneList.Count; i++)
            {
                if (sceneList[i].name == name)
                {
                    overlapScene = sceneList[i];
                    if (overlapScene.isInitialized == false)
                        overlapScene.Initialize();
                    break;
                }
            }
        }

        /// <summary>Close the overlapping state, if any</summary>
        /// <param name="unload">whether or not you want to unload overlap state's content</param>
        public void CloseOverlapState(bool unload = true)
        {
            if (overlapScene != null)
            {
                if (unload)
                    overlapScene.Close();
                overlapScene = null;
            }
        }

        /// <summary>Add a new state in the master state list </summary>
        /// <param name="s">a contructed state</param>
        public void AddScene(Scene s)
        {
            if (s == null)
                return;

            foreach (Scene scene in sceneList)
            {
                if (scene.name == s.name)
                {
                    return;
                }
            }

            s.content = thisGame.Content;
            s.sceneManager = this;
            sceneList.Add(s);
        }

        /// <summary>remove a scene from the list</summary>
        /// <param name="s">scene object</param>
        public void RemoveScene(Scene s)
        {
            if (s != null)
                sceneList.Remove(s);
        }

        /// <summary>remove a scene by name</summary>
        /// <param name="name">name of the scene you want to remove</param>
        public void RemoveScene(string name)
        {
            Scene s = null;
            foreach (Scene scene in sceneList)
            {
                if (scene.name == name)
                {
                    s = scene;
                    break;
                }
            }
            if (s != null)
                sceneList.Remove(s);
        }

        /// <summary>Retrieve a state by id name from the list</summary>
        /// <param name="name">id of the state</param>
        /// <returns>a constructed state out of the master state list</returns>
        public Scene GetScene(String name)
        {
            for (int index = 0; index < sceneList.Count; index++)
            {
                if (sceneList[index].name == name)
                {
                    return sceneList[index];
                }
            }
            return null;
        }

        /// <summary>Total Game Shutdown</summary>
        public void ForceClosed()
        {
            Close();
            thisGame.Exit();
        }

        /// <summary>Init the current state</summary>
        private void initialize()
        {
            if (currentScene != null &&
                currentScene.isInitialized == false)
            {
                currentScene.Initialize();
                currentScene.isInitialized = true;
            }
        }

        /// <summary>Main user-input call</summary>
        /// <param name="kb">current keyboard state</param>
        /// <param name="ms">current mouse state</param>
        /// <param name="gp1">gamepad player 1 state</param>
        public void Input(KeyboardState kb, MouseState ms, GamePadState gp1)
        {
            //ctrl+alt+shift+] = game hard reset, and jump to Dev Scene
            if (kb.IsKeyDown(Keys.LeftShift) && kb.IsKeyDown(Keys.LeftControl) && 
                kb.IsKeyDown(Keys.LeftAlt) && kb.IsKeyDown(Keys.OemCloseBrackets))
            {
                this.HardResetToDevScene();
            }
            if (kb.IsKeyDown(Keys.F9))
            {
                EngineFlags.drawDebug = EngineFlags.drawDevelopment = false;
            }
            else if (kb.IsKeyDown(Keys.F10))
            {
                EngineFlags.drawDebug = EngineFlags.drawDevelopment = true;
            }

            if (overlapScene != null && overlapScene.isInitialized == true)
            {
                    overlapScene.Input(kb, ms, gp1);
                    return;
            }

            if (currentScene != null && currentScene.isInitialized == true)
                currentScene.Input(kb, ms, gp1);
        }

        /// <summary>Main update call</summary>
        public void Update(GameTime time)
        {
            audio.Update(time);
            if (fps != null)
                fps.Update(time);

            if (overlapScene != null && overlapScene.isInitialized == true)
            {
                if (overlapScene.animateIn != null)
                {
                    overlapScene.animateIn.OnAnimateIn(time);
                }
                if (overlapScene.animateOut != null)
                {
                    overlapScene.animateOut.OnAnimateOut(time);
                }
                else
                {
                    overlapScene.Update(time);
                }
            }

            if (currentScene != null && currentScene.isInitialized == true && overlapScene == null)
            {
                if (currentScene.animateIn != null)
                {
                    currentScene.animateIn.OnAnimateIn(time);
                }
                if (currentScene.animateOut != null)
                {
                    currentScene.animateOut.OnAnimateOut(time);
                }
                else
                {
                    currentScene.Update(time);
                }
            }

            renderer.Update(time);
        }

        /// <summary>Main 3D draw call</summary>
        public void Render3D(GameTime time)
        {
            //queue up
            if (currentScene != null && currentScene.isInitialized == true)
            {
                currentScene.Render3D(time);
                if (EngineFlags.drawDebug == true)
                    currentScene.RenderDebug3D(time);
                if (currentScene.animateIn != null)
                    currentScene.animateIn.Render3D();
                if (currentScene.animateOut != null)
                    currentScene.animateOut.Render3D();
            }

            //queue up
            if (overlapScene != null && overlapScene.isInitialized == true)
            {
                overlapScene.Render3D(time);
                if (EngineFlags.drawDebug == true)
                    overlapScene.RenderDebug3D(time);
                if (overlapScene.animateIn != null)
                    overlapScene.animateIn.Render3D();
                if (overlapScene.animateOut != null)
                    overlapScene.animateOut.Render3D();
            }

            this.thisGame.GraphicsDevice.BlendState = BlendState.Opaque;
            this.thisGame.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            //begin rendering process
            renderer.DrawDiffuseTexture();
            renderer.DrawMegaParticles();
            renderer.ProcessHDR();
        }

        /// <summary>Main 2D draw call</summary>
        public void Render2D(GameTime time, SpriteBatch batch)
        {
            if (currentScene != null && currentScene.isInitialized == true)
            {
                currentScene.Render2D(time, batch);
                if (currentScene.animateIn != null)
                    currentScene.animateIn.Render2D(batch);
                if (currentScene.animateOut != null)
                    currentScene.animateOut.Render2D(batch);
            }

            if (overlapScene != null && overlapScene.isInitialized == true)
            {
                overlapScene.Render2D(time, batch);
                if (overlapScene.animateIn != null)
                    overlapScene.animateIn.Render2D(batch);
                if (overlapScene.animateOut != null)
                    overlapScene.animateOut.Render2D(batch);
            }

            if (EngineFlags.drawDebug == true)
            {
                if (overlapScene != null)
                    overlapScene.RenderDebug2D(time, batch);
                else
                    currentScene.RenderDebug2D(time, batch);
            }
            if (fps != null)
                fps.Render2D(batch);
        }

        /// <summary>unload current state's content</summary>
        public void Close()
        {
            if (currentScene != null &&
                currentScene.isInitialized == true)
            {
                currentScene.Close();
                currentScene.isInitialized = false;
            }
        }

        /// <summary>unload all assets and jump to dev scene</summary>
        public void HardResetToDevScene()
        {
            TextureManager.getInstance().ClearLists();
            EffectManager.getInstance().ClearAll();
            AudioManager.Instance.ClearAll();
            this.SetCurrentScene("dev");
        }

        /// <summary>default trigger callback</summary>
        public void CB_Null() { }

        #endregion API

        #region Mutators
        /// <summary>major program game pointer</summary>
        public Microsoft.Xna.Framework.Game Game
        {
            get { return this.thisGame; }
        }
        /// <summary>main 2D spritebatch object</summary>
        public SpriteBatch SpriteBatch
        {
            get { return this.spritebatch; }
        }
        /// <summary>program device manager object</summary>
        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get { return this.deviceManager; }
        }

        #endregion Mutators
    }
}
