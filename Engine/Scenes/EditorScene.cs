using System;
using System.IO;
using System.Xml.Serialization;
using Engine.Drawing_Objects;
using Engine.Drawing_Objects.Materials;
using Engine.Drawing_Objects.ParticleSystems;
using Engine.Game_Objects;
using Engine.Game_Objects.PlatformBehaviors;
using Engine.Managers;
using Engine.Managers.Camera;
using Engine.Math_Physics;
using Engine.Scenes.HUD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#pragma warning disable 1591

namespace Engine.Scenes
{
    /// <summary>This is the Level Editor, used to load, edit, create and update levels,
    /// note: you'll notice that almost all member vars are public, this is because many HUD's need easy
    /// access to it's variables</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class EditorScene : Scene
    {
        #region Member Variables

        public Camera                   camera;

        Grid                            grid;
        public EditorHud                hud;
        public ThumbnailRenderer        thumbnail;
        public HudBase                  currentHud;
        public HudBase                  platformHud;
        public HudBase                  verifyHud;

        Keys                            pressedKey              = Keys.None;

        public Object3D                 currentObject           = null;
        Rectangle                       allowablePickZone       = Rectangle.Empty;
        public bool                     selectorActive          = true;
        public Object3D                 selectedObject          = null;
        public bool                     pickReady               = true;
        public bool                     selectedAlreadyFinalized = false;

        public const float              scaleStep               = .1f;
        public const float              rotationStep            = 2f;

        public EditorAssetList          assetContainer;
        public int                      currentAssetIndex       = -1;

        public Level                    level;
        public string[]                 levelList = 
        { "Level1//level1a", "Level1//level1b", "Level1//level1c", "Level1//level1d", "Level1//level1e" };
        public int                      currentLevelIndex = 0;

        /// <summary>The main crosshairs locator object</summary>
        public Point3D                  point                   = new Point3D();

        #endregion Member Variables

        #region Initialization & Closing

        /// <summary>Default CTOR</summary>
        /// <param name="g">major game pointer</param>
        /// <param name="name">name of this scene</param>
        /// <param name="sm">pointer to the scene manager</param>
        public EditorScene(Microsoft.Xna.Framework.Game g, String name, SceneManager sm)
            :base(g, name, sm) { }

        /// <summary>All major scene initialization logic</summary>
        public override void Initialize()
        {
            worldSetup();
            grid = new Grid();
            hud = new EditorHud(this);
            thumbnail = new ThumbnailRenderer(this);
            loadAssetList();

            point.Initialize(Vector3.Zero);
            point.GenerateBoundingBox();

            level = new Level();
            level.Initialize(content, device, ref camera);
            renderer.RegisterLightList(ref level.lightList);

            allowablePickZone = new Rectangle(0, 96, 960, 640);

            EngineFlags.forceBulbs = true;

            isInitialized = true;
        }

        /// <summary>create the camera, and set our current camera to the Renderer</summary>
        private void worldSetup()
        {
            camera = new Camera(device.Viewport);
            camera.Initialize(new FreeCamera(ref camera));
            camera.Position = new Vector3(1000, 500, 1000);
            camera.Yaw = MathHelper.ToRadians(-55);
            camera.Pitch = MathHelper.ToRadians(-75);
            Renderer.getInstance().Camera = camera;
            sceneManager.Game.IsMouseVisible = false;
        }
        
        /// <summary>Loads the XML list of user-created objects from xml</summary>
        private void loadAssetList()
        {
            try
            {
                EditorAssetList editorAssetList = new EditorAssetList();
                TextReader reader = new StreamReader("content//data//EditorModelList.xml");
                XmlSerializer xml = new XmlSerializer(typeof(EditorAssetList));
                editorAssetList = (EditorAssetList)xml.Deserialize(reader);
                reader.Close();
                this.assetContainer = editorAssetList;
            }
            catch (Exception e)
            {
                if (EngineFlags.drawDebug)
                    Math_Physics.MyUtility.DumpException(e);
            }
        }

        /// <summary>Major scene cloing logic, releases assets and deallocate memory</summary>
        public override void Close()
        {
            camera = null;
            grid = null;
            hud = null;
            level.Release(); 
            level = null;
            currentHud = null;
            verifyHud = null;

            pressedKey = Keys.None;
            thumbnail = null;
            isInitialized = false;
            
            currentObject = null;
            selectedObject = null;
            selectorActive = true;

            EngineFlags.forceBulbs = false;
            renderer.UnRegisterLightList();
            allowablePickZone = Rectangle.Empty;
            assetContainer = null;
            TextureManager.getInstance().ClearLists();
            EffectManager.getInstance().ClearAll();
        }

        #endregion Initialization & Closing

        #region Run-Time

        /// <summary>All major scene input logic, called first before Update</summary>
        /// <param name="kb">current keyboard state</param>
        /// <param name="ms">current mouse state</param>
        /// <param name="gp1">current gamepad1 state</param>
        public override void Input(KeyboardState kb, MouseState ms, GamePadState gp1)
        {
            cameraInput(kb, ms);
            if (verifyHud != null)
            {
                verifyHud.Input(ms);
                return;
            }
            if(hud != null)
                hud.Input(kb, ms);
            if(thumbnail != null)
                thumbnail.Input(kb, ms);
            if (currentHud != null)
                currentHud.Input(ms);
            if (platformHud != null)
                platformHud.Input(ms);

            #region Release
            if (ms.RightButton == ButtonState.Pressed)
            {
                selectedObject = null;
                toggleSelector(true);
                toggleCollide(false);
                toggleLogical(false);
                this.NullParameterHuds();
            }
            #endregion Release 

            #region 3D pick
            if (isWithinAllowableZone(ms))
            {
                //if we grabbed the point, we will not pick an object
                if (!point.MoveWithMouse(MyUtility.GetMouseRay(device.Viewport, camera), ms))
                {
                    if (selectorActive == true &&
                        selectedObject == null &&
                        ms.LeftButton == ButtonState.Pressed &&
                        kb.IsKeyUp(Keys.LeftControl) &&
                        pickReady == true)
                    {
                        selectedObject = this.pickPlacedObject();
                        if (selectedObject != null)
                        {
                            toggleCollide(selectedObject.Collidable);
                            toggleDepth(selectedObject.RenderDepth);
                            if (selectedObject.GetType() == typeof(Trigger))
                                toggleLogical(true);
                            this.CreateHud();
                            pickReady = false;
                            selectedAlreadyFinalized = true;
                        }
                        else NullParameterHuds();
                    }

                    if ( selectedObject != null &&
                        ms.LeftButton == ButtonState.Pressed &&
                        kb.IsKeyDown(Keys.LeftControl) &&
                        pickReady == true)
                    {
                        selectedObject = this.pickNextDeepestObject();
                        if (selectedObject != null)
                        {
                            toggleCollide(selectedObject.Collidable);
                            toggleDepth(selectedObject.RenderDepth);
                            if (selectedObject.GetType() == typeof(Trigger))
                                toggleLogical(true);
                            this.CreateHud();
                            pickReady = false;
                            selectedAlreadyFinalized = true;
                        }
                        else NullParameterHuds();
                    }
                }
            }
            if (pickReady == false && ms.LeftButton == ButtonState.Released)
                pickReady = true;
            #endregion 3D pick
        }

        /// <summary>determines whether the mouse is within any of the huds</summary>
        /// <returns>false: the mouse is with a hud, true: the mouse is within the environment</returns>
        private bool isWithinAllowableZone(MouseState ms)
        {
            if (! MyMath.IsWithin(ms.X, ms.Y, allowablePickZone))
                return false;
            if (currentHud != null)
                if (MyMath.IsWithin(ms.X, ms.Y, currentHud.Rect))
                    return false;

            return true;
        }

        /// <summary>Get the first object closest to the camera via mouse ray picking</summary>
        private Object3D pickPlacedObject()
        {
            Object3D closest = null;
            float near, far;
            float nearest = float.MaxValue;
            Ray ray = MyUtility.GetMouseRay(device.Viewport, camera);
            Vector3 foundPosition = point.WorldMatrix.Position;

            //search collidable list
            foreach (Object3D obj in level.collidableList)
            {
                if (obj.OBB == null)
                    continue;
                if(obj.OBB.Intersects(ray, out near, out far) != -1)
                {
                    if (near < nearest)
                    {
                        closest = obj;
                        nearest = near;
                        foundPosition = obj.WorldMatrix.Position;
                    }
                }
            }
            //search doodad list
            foreach (Object3D obj in level.dooDadList)
            {
                if (obj.OBB == null)
                    continue;
                if (obj.OBB.Intersects(ray, out near, out far) != -1)
                {
                    if (near < nearest)
                    {
                        closest = obj;
                        nearest = near;
                        foundPosition = obj.WorldMatrix.Position;
                    }
                }
            }
            //search trigger list
            foreach (Object3D obj in level.triggerList)
            {
                if (obj.OBB.Intersects(ray, out near, out far) != -1)
                {
                    if (near < nearest)
                    {
                        closest = obj;
                        nearest = near;
                        foundPosition = obj.WorldMatrix.Position;
                    }
                }
            }
            //search light list
            foreach (Object3D obj in level.lightList)
            {
                if (obj.OBB.Intersects(ray, out near, out far) != -1)
                {
                    if (near < nearest)
                    {
                        closest = obj;
                        nearest = near;
                        foundPosition = obj.WorldMatrix.Position;
                    }
                }
            }
            //search enemy spawn point list
            foreach (Object3D obj in level.enemySpawnPointList)
            {
                if (obj.OBB.Intersects(ray, out near, out far) != -1)
                {
                    if (near < nearest)
                    {
                        closest = obj;
                        nearest = near;
                        foundPosition = obj.WorldMatrix.Position;
                    }
                }
            }

            if (closest != null)
            {
                toggleSelector(false);
                point.WorldMatrix.Position = foundPosition;
            }

            return closest;
        }

        /// <summary>From whatever distance is it to the current object, get the next deepest one</summary>
        private Object3D pickNextDeepestObject()
        {
            if (selectedObject == null)
            {
                return pickPlacedObject();
            }

            Object3D output = null;
            float near, far, current, closest;
            Ray ray = MyUtility.GetMouseRay(device.Viewport, camera);

            current = closest = float.MaxValue;
            if (selectedObject.OBB.Intersects(ray, out near, out far) != -1)
                current = near;

            //search collidable list
            foreach (Object3D obj in level.collidableList)
            {
                if (obj.OBB == null)
                    continue;
                if (obj.OBB.Intersects(ray, out near, out far) != -1)
                {
                    if (near > current && near < closest)
                    {
                        closest = near;
                        output = obj;
                    }
                }
            }
            //search doodad list
            foreach (Object3D obj in level.dooDadList)
            {
                if (obj.OBB == null)
                    continue;
                if (obj.OBB.Intersects(ray, out near, out far) != -1)
                {
                    if (near > current && near < closest)
                    {
                        closest = near;
                        output = obj;
                    }
                }
            }
            //search trigger list
            foreach (Object3D obj in level.triggerList)
            {
                if (obj.OBB.Intersects(ray, out near, out far) != -1)
                {
                    if (near > current && near < closest)
                    {
                        closest = near;
                        output = obj;
                    }
                }
            }
            //search light list
            foreach (Object3D obj in level.lightList)
            {
                if (obj.OBB.Intersects(ray, out near, out far) != -1)
                {
                    if (near > current && near < closest)
                    {
                        closest = near;
                        output = obj;
                    }
                }
            }
            //search enemy spawn point list
            foreach (Object3D obj in level.enemySpawnPointList)
            {
                if (obj.OBB.Intersects(ray, out near, out far) != -1)
                {
                    if (near > current && near < closest)
                    {
                        closest = near;
                        output = obj;
                    }
                }
            }

            if (output != null)
            {
                toggleSelector(false);
                point.WorldMatrix.Position = output.WorldMatrix.Position;
            }

            return output;
        }

        /// <summary>after something is picked from the level, this creates the proper Hud</summary>
        public void CreateHud()
        {
            if (selectedObject == null)
                return;

            if (selectedObject.GetType() == typeof(EnemySpawnLocation))
            {
                currentHud = new EnemySpawnPointHud(this, selectedObject as EnemySpawnLocation);
                currentHud.Initialize(content);
                return;
            }
            else if (selectedObject.GetType() == typeof(PointLight))
            {
                currentHud = new LightHud(this, selectedObject as PointLight);
                currentHud.Initialize(content);
                return;
            }
            else if (selectedObject.GetType() == typeof(Platform))
            {
                platformHud = (selectedObject as Platform).Behavior.GetHud(content, this);
            }
            else if (selectedObject.GetType() == typeof(SmokeEmitter))
            {
                currentHud = new MegaParticleHud(this, selectedObject as SmokeEmitter);
                currentHud.Initialize(content);
                return;
            }
            else if (selectedObject.GetType() == typeof(Trigger))
            {
                currentHud = new TriggerHud(this, selectedObject as Trigger);
                currentHud.Initialize(content);
                return;
            }

            if (selectedObject.Material != null)
            {
                if (selectedObject.GetType() != typeof(PointLight) &&
                    selectedObject.GetType() != typeof(Trigger) &&
                    selectedObject.Material.GetType() != typeof(ParallaxOcclusionMaterial) &&
                    selectedObject.Material.GetType() != typeof(WaterMaterial))
                {
                    currentHud = new LightingParametersHud(this, selectedObject.Material);
                    currentHud.Initialize(content);
                }
                else if (selectedObject.Material.GetType() == typeof(WaterMaterial))
                {
                    currentHud = new WaterHud(this, selectedObject.Material as WaterMaterial);
                    currentHud.Initialize(content);
                    return;
                }
                else if (selectedObject.Material.GetType() == typeof(ParallaxOcclusionMaterial))
                {
                    currentHud = new POMHud(this, selectedObject.Material);
                    currentHud.Initialize(content);
                    return;
                }
            }
        }

        /// <summary>All major update logic in here</summary>
        /// <param name="time">current game time values</param>
        public override void Update(GameTime time)
        {
            camera.Update(time);
            thumbnail.Update(time);

            point.EditRadiusPerCamera(camera.Position);
            point.Update(ref camera, time);
            point.UpdateBoundingBox();

            if (selectedObject != null)
            {
                selectedObject.WorldMatrix.Position = point.WorldMatrix.Position;

                if (selectedObject.GetType() == typeof(Platform))
                    platformSpecific(point.WorldMatrix.Position);

                if (selectedAlreadyFinalized == false)
                    selectedObject.Update(ref camera, time);

                selectedObject.UpdateBoundingBox();
            }

            if (currentHud != null)
                currentHud.Update(time);
            if (platformHud != null)
                platformHud.Update(time);

            level.Update(ref camera, time);
            foreach (Object3D obj in level.enemySpawnPointList)
                obj.Update(ref camera, time);
        }

        private void platformSpecific(Vector3 position)
        {
            Platform p = selectedObject as Platform;
            if (p.Behavior.GetType() == typeof(IHorizontalPlatform))
            {
                (p.Behavior as IHorizontalPlatform).center = position.X;
            }
            else if (p.Behavior.GetType() == typeof(IVerticalPlatform))
            {
                (p.Behavior as IVerticalPlatform).center = position.Y;
            }
        }

        /// <summary>Render all 3D objects here</summary>
        /// <param name="time">current game timing values</param>
        public override void Render3D(GameTime time)
        {
            grid.Render();

            point.Render();
            if (selectedObject != null)
            {
                if(selectedAlreadyFinalized == false)
                    selectedObject.Render();
                selectedObject.RenderDebug();
            }

            if (level != null)
            {
                level.Render3D();
                foreach (Object3D obj in level.enemySpawnPointList)
                {
                    obj.Render();
                    obj.RenderDebug();
                }
            }

            if (currentHud != null)
                currentHud.Render3D();
            if (platformHud != null)
                platformHud.Render3D();

            thumbnail.Render3D();

            //PhysXEngine.Instance.Render3D();
        }

        /// <summary>Render all 2D spritebatch items</summary>
        /// <param name="time">current game timing values</param>
        /// <param name="batch">xna spritebatch to draw with</param>
        public override void Render2D(GameTime time, SpriteBatch batch)
        {
            hud.Render2D(batch);
            thumbnail.Render2D(batch);
            if (currentHud != null)
                currentHud.Render2D(batch);
            if (platformHud != null)
                platformHud.Render2D(batch);
            if (verifyHud != null)
                verifyHud.Render2D(batch);
        }

        /// <summary>Draw all 2D debugging stuff in here</summary>
        /// <param name="time">current game timing values</param>
        /// <param name="batch">xna spritebatch to draw with</param>
        public override void RenderDebug2D(GameTime time, SpriteBatch batch) { }
        
        /// <summary>Draw all 3D debug in here</summary>
        /// <param name="time">current game timing values</param>
        public override void RenderDebug3D(GameTime time) { }

        #endregion Run-Time

        #region Misc

        /// <summary>Camera input logic, hotkey: Z will toggle between free and fixed camera modes,
        /// then update any UI stuff accordingly</summary>
        private void cameraInput(KeyboardState kb, MouseState ms)
        {
            if (camera == null)
                return;
            if (kb.IsKeyDown(Keys.Z))
                pressedKey = Keys.Z;
            if (kb.IsKeyUp(Keys.Z) && pressedKey == Keys.Z)
            {
                pressedKey = Keys.None;
                if (camera.Behavior.GetType() == typeof(StationaryCamera))
                {
                    Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
                    camera.Behavior = new FreeCamera(ref camera);
                    sceneManager.Game.IsMouseVisible = false;
                    foreach (SpriteButton btn in hud.buttonList)
                    {
                        if (btn.ID == GameIDList.Button_Editor_CameraToggle)
                        {
                            btn.CurrentFrameX = 0;
                            return;
                        }
                    }
                }
                else if (camera.Behavior.GetType() == typeof(FreeCamera))
                {
                    camera.Behavior = new StationaryCamera();
                    sceneManager.Game.IsMouseVisible = true;
                    
                    foreach (SpriteButton btn in hud.buttonList)
                    {
                        if (btn.ID == GameIDList.Button_Editor_CameraToggle)
                        {
                            btn.CurrentFrameX = 1;
                            return;
                        }
                    }
                }
            }
            camera.Input(kb, ms);
        }

        /// <summary>manually update the logical button</summary>
        public void toggleLogical(bool value)
        {
            if (selectedObject != null)
            {
                if (selectedObject.GetType() == typeof(Trigger) && value == false)
                    return;
            }
            foreach (SpriteButton btn in hud.buttonList)
            {
                if (btn.ID == GameIDList.Button_Editor_logical)
                {
                    if (value == true)
                        btn.CurrentFrameX = 1;
                    else btn.CurrentFrameX = 0;
                    return;
                }
            }
        }

        /// <summary>Toggle the selector active boolean value, allowing to select other objects</summary>
        public void toggleSelector(bool active)
        {
            selectorActive = active;
            foreach (SpriteButton btn in hud.buttonList)
            {
                if (btn.ID == GameIDList.Button_Editor_SelectorActive)
                {
                    if (active == true)
                        btn.CurrentFrameX = 1;
                    else btn.CurrentFrameX = 0;
                }
            }
        }

        /// <summary>updates the collidable btn</summary>
        public void toggleCollide(bool collidable)
        {
            if (selectedObject != null)
            {
                selectedObject.Collidable = collidable;
            }
            foreach (SpriteButton btn in hud.buttonList)
            {
                if (btn.ID == GameIDList.Button_Editor_collision)
                {
                    if (collidable == true)
                        btn.CurrentFrameX = 1;
                    else btn.CurrentFrameX = 0;
                    return;
                }
            }
        }

        /// <summary>updates the render to depth value</summary>
        public void toggleDepth(bool depth)
        {
            if (selectedObject == null)
                return;
            if (selectedObject.GetType() == typeof(EnemySpawnLocation))
                return;

            selectedObject.RenderDepth = depth;
            
            foreach (SpriteButton btn in hud.buttonList)
            {
                if (btn.ID == GameIDList.Button_Editor_ToggleDepthRender)
                {
                    if (depth == true)
                        btn.CurrentFrameX = 1;
                    else btn.CurrentFrameX = 0;
                    return;
                }
            }
        }

        /// <summary>set all current huds to null, reset all picking values</summary>
        public void NullParameterHuds()
        {
            currentHud = null;
            platformHud = null;
            toggleDepth(false);
            toggleCollide(false);
            toggleLogical(false);
            toggleSelector(true);
        }

        #endregion Misc
    }
}
