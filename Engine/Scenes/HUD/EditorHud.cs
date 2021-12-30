using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Engine.Drawing_Objects;
using Engine.Drawing_Objects.Materials;
using Engine.Drawing_Objects.ParticleSystems;
using Engine.Game_Objects;
using Engine.Managers;
using Engine.Managers.Camera;
using Engine.Managers.Factories;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Engine.Scenes.HUD
{
    /// <summary>EditorHud encapsulates all UI elements</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class EditorHud
    {
        #region Member Vars
        EditorScene                 editor;
        /// <summary>reference to the blank texture</summary>
        public Texture2D            blank;
        SpriteFont                  font;
        bool                        showHotkeys     = false;
        /// <summary>main button list</summary>
        public List<SpriteButton>   buttonList      = new List<SpriteButton>();

        bool                        copyDown        = false;
        Object3D                    objectCopy      = null;

        #endregion Member Vars

        #region Initialization

        /// <summary>Default CTOR</summary>
        public EditorHud(EditorScene editor)
        {
            this.editor = editor;
            blank = TextureManager.getInstance().GetTexture("sprites//blank");
            font = FontManager.getInstance().Get("fonts//Arial");
            this.initButtons();
        }

        /// <summary>Load all buttons, initialize all callbacks, etc...</summary>
        private void initButtons()
        {
            float rightPanelX = editor.device.Viewport.Width - 256;
            float worldSectionY = 312 + 16;

            //camera type btn (display only)
            SpriteButton btn = new SpriteButton(GameIDList.Button_Editor_CameraToggle);
            btn.Initialize("sprites//editor//LE_camType");
            btn.Position = new Vector2(390, 12);
            btn.ToggleFrames = false;
            buttonList.Add(btn);

            //goto dev scene
            btn = new SpriteButton();
            btn.Initialize("sprites//blank_square_btn_small", "Dev.\nScene");
            btn.Position = new Vector2(0, editor.device.Viewport.Height - 64);
            btn.setExecution(null, Hud_gotoDevScene);
            buttonList.Add(btn);

            //save level btn
            btn = new SpriteButton(GameIDList.Button_Editor_SaveLevel);
            btn.Initialize("sprites//editor//LE_save");
            btn.Position = new Vector2(24, 34);
            btn.setExecution(null, Hud_verifySave);
            buttonList.Add(btn);

            //load level btn
            btn = new SpriteButton(GameIDList.Button_Editor_LoadLevel);
            btn.Initialize("sprites//editor//LE_load");
            btn.Position = new Vector2(24 + 64 + 4, 34);
            btn.setExecution(null, Hud_verifyLoad);
            buttonList.Add(btn);

            //level filename btn
            btn = new SpriteButton(GameIDList.Button_Editor_LevelFileToggle);
            btn.Initialize("sprites//editor//LE_levelNameBlank");
            btn.Position = new Vector2(0, 0);
            btn.setExecution(null, Hud_incrementLevelFile);
            buttonList.Add(btn);

            //place new btn
            btn = new SpriteButton(GameIDList.Button_Editor_PlaceNewObject);
            btn.Initialize("sprites//editor//LE_placeNew");
            btn.Position = new Vector2(rightPanelX + (256 / 4) - btn.Texture.Width / 4, 24);
            btn.setExecution(null, Hud_placeNewObject);
            buttonList.Add(btn);

            //finalize selected btn
            btn = new SpriteButton(GameIDList.Button_Editor_Finalize);
            btn.Initialize("sprites//editor//LE_finalize");
            btn.Position = new Vector2(rightPanelX + ((256 / 4) * 2) - btn.Texture.Width / 4, 24);
            btn.setExecution(null, Hud_finalizeSelectedObject);
            buttonList.Add(btn);

            //delete selected btn
            btn = new SpriteButton(GameIDList.Button_Editor_DeleteSelected);
            btn.Initialize("sprites//editor//LE_delete");
            btn.Position = new Vector2(rightPanelX + ((256 / 4) * 3) - btn.Texture.Width / 4, 24);
            btn.setExecution(null, Hud_deleteSelectedObject);
            buttonList.Add(btn);

            //model thumbnail scroll left btn
            btn = new SpriteButton(GameIDList.Button_Editor_ThumbnailScrollLeft);
            btn.Initialize("sprites//editor//LE_thumbnailLeft");
            btn.Position = new Vector2(rightPanelX + 18, 92);
            btn.setExecution(null, Hud_thumnailScrollLeft);
            buttonList.Add(btn);

            //model thumbnail scroll right btn
            btn = new SpriteButton(GameIDList.Button_Editor_ThumbnailScrollRight);
            btn.Initialize("sprites//editor//LE_thumbnailRight");
            btn.Position = new Vector2(editor.device.Viewport.Width - 18 - btn.FrameSize.X, 92);
            btn.setExecution(null, Hud_thumnailScrollRight);
            buttonList.Add(btn);

            //model scale X up
            btn = new SpriteButton(GameIDList.Button_Editor_ScaleXUp);
            btn.Initialize("sprites//editor//LE_btnUpSmall");
            btn.Position = new Vector2(rightPanelX + 4, worldSectionY);
            btn.setExecution(Hud_scaleXUp, Hud_scaleXUp);
            buttonList.Add(btn);

            //model scale Y up
            btn = new SpriteButton(GameIDList.Button_Editor_ScaleYUp);
            btn.Initialize("sprites//editor//LE_btnUpSmall");
            btn.Position = new Vector2(rightPanelX + 4 + btn.FrameSize.X, worldSectionY);
            btn.setExecution(Hud_scaleYUp, Hud_scaleYUp);
            buttonList.Add(btn);

            //model scale Z up
            btn = new SpriteButton(GameIDList.Button_Editor_ScaleZUp);
            btn.Initialize("sprites//editor//LE_btnUpSmall");
            btn.Position = new Vector2(rightPanelX + 4 + (btn.FrameSize.X * 2), worldSectionY);
            btn.setExecution(Hud_scaleZUp, Hud_scaleZUp);
            buttonList.Add(btn);

            //model rotate X up
            btn = new SpriteButton(GameIDList.Button_Editor_RotateXUp);
            btn.Initialize("sprites//editor//LE_btnUpSmall");
            btn.Position = new Vector2((btn.FrameSize.X * 4) + rightPanelX + 4, worldSectionY);
            btn.setExecution(Hud_rotateXUp, Hud_rotateXUp);
            buttonList.Add(btn);

            //model rotate Y up
            btn = new SpriteButton(GameIDList.Button_Editor_RotateYUp);
            btn.Initialize("sprites//editor//LE_btnUpSmall");
            btn.Position = new Vector2((btn.FrameSize.X * 4) + rightPanelX + 4 + btn.FrameSize.X, worldSectionY);
            btn.setExecution(Hud_rotateYUp, Hud_rotateYUp);
            buttonList.Add(btn);

            //model rotate Z up
            btn = new SpriteButton(GameIDList.Button_Editor_RotateZUp);
            btn.Initialize("sprites//editor//LE_btnUpSmall");
            btn.Position = new Vector2((btn.FrameSize.X * 4) + rightPanelX + 4 + (btn.FrameSize.X * 2), worldSectionY);
            btn.setExecution(Hud_rotateZUp, Hud_rotateZUp);
            buttonList.Add(btn);

            //model scale X down
            btn = new SpriteButton(GameIDList.Button_Editor_ScaleXDown);
            btn.Initialize("sprites//editor//LE_btnDownSmall");
            btn.Position = new Vector2(rightPanelX + 4, worldSectionY + 28);
            btn.setExecution(Hud_scaleXDown, Hud_scaleXDown);
            buttonList.Add(btn);

            //model scale Y down
            btn = new SpriteButton(GameIDList.Button_Editor_ScaleYDown);
            btn.Initialize("sprites//editor//LE_btnDownSmall");
            btn.Position = new Vector2(rightPanelX + 4 + btn.FrameSize.X, worldSectionY + 28);
            btn.setExecution(Hud_scaleYDown, Hud_scaleYDown);
            buttonList.Add(btn);

            //model scale Z down
            btn = new SpriteButton(GameIDList.Button_Editor_ScaleZDown);
            btn.Initialize("sprites//editor//LE_btnDownSmall");
            btn.Position = new Vector2(rightPanelX + 4 + (btn.FrameSize.X * 2), worldSectionY + 28);
            btn.setExecution(Hud_scaleZDown, Hud_scaleZDown);
            buttonList.Add(btn);

            //model rotate X down
            btn = new SpriteButton(GameIDList.Button_Editor_RotateXDown);
            btn.Initialize("sprites//editor//LE_btnDownSmall");
            btn.Position = new Vector2((btn.FrameSize.X * 4) + rightPanelX + 4, worldSectionY + 28);
            btn.setExecution(Hud_rotateXDown, Hud_rotateXDown);
            buttonList.Add(btn);

            //model rotate Y down
            btn = new SpriteButton(GameIDList.Button_Editor_RotateYDown);
            btn.Initialize("sprites//editor//LE_btnDownSmall");
            btn.Position = new Vector2((btn.FrameSize.X * 4) + rightPanelX + 4 + btn.FrameSize.X, worldSectionY + 28);
            btn.setExecution(Hud_rotateYDown, Hud_rotateYDown);
            buttonList.Add(btn);

            //model rotate Z down
            btn = new SpriteButton(GameIDList.Button_Editor_RotateZDown);
            btn.Initialize("sprites//editor//LE_btnDownSmall");
            btn.Position = new Vector2((btn.FrameSize.X * 4) + rightPanelX + 4 + (btn.FrameSize.X * 2), worldSectionY + 28);
            btn.setExecution(Hud_rotateZDown, Hud_rotateZDown);
            buttonList.Add(btn);

            //model reset world
            btn = new SpriteButton(GameIDList.Button_Editor_ResetWorld);
            btn.Initialize("sprites//editor//LE_btnResetWorld");
            btn.Position = new Vector2(rightPanelX + 199, worldSectionY + 4);
            btn.setExecution(null, Hud_resetWorld);
            buttonList.Add(btn);

            //uniform scale down
            btn = new SpriteButton(GameIDList.Button_Editor_xyzDown);
            btn.Initialize("sprites//editor//LE_xyzDown");
            btn.Position = new Vector2(rightPanelX + 8, worldSectionY + 44);
            btn.setExecution(null, Hud_scaleXyzDown);
            buttonList.Add(btn);

            //uniform scale up
            btn = new SpriteButton(GameIDList.Button_Editor_xyzUp);
            btn.Initialize("sprites//editor//LE_xyzUp");
            btn.Position = new Vector2(rightPanelX + 8 + 32, worldSectionY + 44);
            btn.setExecution(null, Hud_scaleXyzUp);
            buttonList.Add(btn);

            //rotate model 45 X 
            btn = new SpriteButton(GameIDList.Button_Editor_rotate45X);
            btn.Initialize("sprites//editor//LE_add45");
            btn.Position = new Vector2((btn.FrameSize.X * 4) + rightPanelX + 4, worldSectionY + 46);
            btn.setExecution(null, Hud_rotate45X);
            buttonList.Add(btn);

            //rotate model 45 Y 
            btn = new SpriteButton(GameIDList.Button_Editor_rotate45Y);
            btn.Initialize("sprites//editor//LE_add45");
            btn.Position = new Vector2((btn.FrameSize.X * 4) + rightPanelX + 4 + btn.FrameSize.X, worldSectionY + 46);
            btn.setExecution(null, Hud_rotate45Y);
            buttonList.Add(btn);

            //rotate model 45 Z 
            btn = new SpriteButton(GameIDList.Button_Editor_rotate45Z);
            btn.Initialize("sprites//editor//LE_add45");
            btn.Position = new Vector2((btn.FrameSize.X * 4) + rightPanelX + 4 + (btn.FrameSize.X * 2), worldSectionY + 46);
            btn.setExecution(null, Hud_rotate45Z);
            buttonList.Add(btn);

            //collision toggle button
            btn = new SpriteButton(GameIDList.Button_Editor_collision);
            btn.Initialize("sprites//editor//LE_checkBox");
            btn.Position = new Vector2(rightPanelX + 6, 236);
            btn.setExecution(null, Hud_toggleCollision);
            btn.ToggleFrames = false;
            buttonList.Add(btn);

            //logical toggle button
            btn = new SpriteButton(GameIDList.Button_Editor_logical);
            btn.Initialize("sprites//editor//LE_checkBox");
            btn.Position = new Vector2(rightPanelX + 6, 236 + 38);
            btn.setExecution(null, Hud_toggleLogical);
            btn.ToggleFrames = false;
            buttonList.Add(btn);

            //selector is locked or unlocked button
            btn = new SpriteButton(GameIDList.Button_Editor_SelectorActive);
            btn.Initialize("sprites//editor//LE_selectorActive");
            btn.Position = new Vector2(rightPanelX - 64, 24);
            btn.ToggleFrames = false;
            buttonList.Add(btn);

            //snap camera position X
            btn = new SpriteButton(GameIDList.Button_Editor_CamPosX);
            btn.Initialize("sprites//editor//LE_posX");
            btn.Position = new Vector2(440, 30);
            btn.setExecution(null, Hud_camPosX);
            buttonList.Add(btn);

            //snap camera negative X
            btn = new SpriteButton(GameIDList.Button_Editor_CamNegX);
            btn.Initialize("sprites//editor//LE_negX");
            btn.Position = new Vector2(532, 30);
            btn.setExecution(null, Hud_camNegX);
            buttonList.Add(btn);

            //snap camera position Z
            btn = new SpriteButton(GameIDList.Button_Editor_CamPosZ);
            btn.Initialize("sprites//editor//LE_posZ");
            btn.Position = new Vector2(486, 4);
            btn.setExecution(null, Hud_camPosZ);
            buttonList.Add(btn);

            //snap camera negative Z
            btn = new SpriteButton(GameIDList.Button_Editor_CamNegZ);
            btn.Initialize("sprites//editor//LE_negZ");
            btn.Position = new Vector2(486, 54);
            btn.setExecution(null, Hud_camNegZ);
            buttonList.Add(btn);

            //snap camera look down
            btn = new SpriteButton(GameIDList.Button_Editor_LookDown);
            btn.Initialize("sprites//editor//LE_posY");
            btn.Position = new Vector2(486, 30);
            btn.setExecution(null, Hud_lookDown);
            buttonList.Add(btn);

            //create new light
            btn = new SpriteButton(GameIDList.Button_Editor_NewLight);
            btn.Initialize("sprites//editor//LE_newLight");
            btn.Position = new Vector2(1024 - 64, 96);
            btn.setExecution(null, Hud_newLight);
            buttonList.Add(btn);

            //create new trigger
            btn = new SpriteButton(GameIDList.Button_Editor_NewTrigger);
            btn.Initialize("sprites//editor//LE_newTrigger");
            btn.Position = new Vector2(1024 - 64, 96 + 64);
            btn.setExecution(null, Hud_newTrigger);
            buttonList.Add(btn);

            //new mega particle system
            btn = new SpriteButton(GameIDList.Button_Editor_NewParticleSystem);
            btn.Initialize("sprites//editor//LE_newParticleSystem");
            btn.Position = new Vector2(1024 - 64, 96 + 64 + 64);
            btn.setExecution(null, Hud_newParticleSystem);
            buttonList.Add(btn);

            //new enemy spawn location
            btn = new SpriteButton(GameIDList.Button_Editor_NewEnemyLocation);
            btn.Initialize("sprites//editor//LE_newSpawnLocation");
            btn.Position = new Vector2(1024 - 64, 96 + 64 + 64 + 64);
            btn.setExecution(null, Hud_newEnemySpawnPoint);
            buttonList.Add(btn);

            //render to depth toggle
            btn = new SpriteButton(GameIDList.Button_Editor_ToggleDepthRender);
            btn.Initialize("sprites//editor//LE_checkBox");
            btn.Position = new Vector2(1024 + 112, 236);
            btn.setExecution(null, Hud_toggleDepth);
            btn.ToggleFrames = false;
            buttonList.Add(btn);
        }

        #endregion Initialization

        #region API
        /// <summary>All Input logic for HUD items enters here</summary>
        public void Input(KeyboardState kb, MouseState ms)
        {
            if (kb.IsKeyDown(Keys.OemTilde))
                showHotkeys = true;
            else showHotkeys = false;

            MyUtility.ProcessButtonList(ms, buttonList);

            this.processCopyPaste(kb);

            if (kb.IsKeyDown(Keys.Enter) && editor.selectedObject != null)
                Hud_finalizeSelectedObject();
            if (kb.IsKeyDown(Keys.Delete) && editor.selectedObject != null)
                Hud_deleteSelectedObject();
        }

        /// <summary>functionality to copy and paste objects</summary>
        private void processCopyPaste(KeyboardState kb)
        {
            if (kb.IsKeyDown(Keys.LeftControl) && kb.IsKeyDown(Keys.C) && copyDown == false)
            {
                copyDown = true;
                if (editor.selectedObject != null)
                    objectCopy = editor.selectedObject.GetCopy(editor.content);
            }
            else if ((!kb.IsKeyDown(Keys.LeftControl) || !kb.IsKeyDown(Keys.C)) && copyDown == true)
            {
                copyDown = false;
            }
            if (kb.IsKeyDown(Keys.LeftControl) && kb.IsKeyDown(Keys.V) && copyDown == false)
            {
                copyDown = true;
                if (objectCopy != null)
                {
                    editor.selectedObject = objectCopy.GetCopy(editor.content);
                    editor.selectedObject.GenerateBoundingBox();
                    editor.selectedObject.UpdateBoundingBox();
                }
            }
            else if ((!kb.IsKeyDown(Keys.LeftControl) || !kb.IsKeyDown(Keys.V)) && copyDown == true)
            {
                copyDown = false;
            }
        }

        /// <summary>Major Draw Call</summary>
        public void Render2D(SpriteBatch batch)
        {
            float panelX = editor.device.Viewport.Width - 256;
            //level info panel
            batch.Draw(blank, new Rectangle(0, 0, 384, 72), MyColors.AlphaBlack);

            //properties panel
            batch.Draw(blank, new Rectangle((int)panelX, 0, 256, 92), MyColors.AlphaBlack);
            batch.Draw(blank, new Rectangle((int)panelX, 92, 64, 128), MyColors.AlphaBlack);
            batch.Draw(blank, new Rectangle((int)panelX + 64 + 128, 92, 64, 128), MyColors.AlphaBlack);
            batch.Draw(blank, new Rectangle((int)panelX, 220, 256, 172), MyColors.AlphaBlack);
            batch.Draw(blank, new Rectangle(1024 - 64, 96, 64, 64 * 4), MyColors.AlphaBlack);

            //buttons
            foreach (SpriteButton btn in buttonList)
                btn.Draw(batch);

            //additional text
            batch.DrawString(font, "~ : display hotkeys", new Vector2(0, 72), Color.White);
            batch.DrawString(font, "Camera", new Vector2(387, 0), Color.White);
            batch.DrawString(font, "Scale", new Vector2(panelX + 26, 296 + 16), Color.White);
            batch.DrawString(font, "X", new Vector2(panelX + 13, 325 + 16), Color.White);
            batch.DrawString(font, "Y", new Vector2(panelX + 13 + 24, 325 + 16), Color.White);
            batch.DrawString(font, "Z", new Vector2(panelX + 13 + 48, 325 + 16), Color.White);
            batch.DrawString(font, "Rotation", new Vector2(panelX + 118, 296 + 16), Color.White);
            batch.DrawString(font, "X", new Vector2(panelX + 13 + 96, 325 + 16), Color.White);
            batch.DrawString(font, "Y", new Vector2(panelX + 13 + 24 + 96, 325 + 16), Color.White);
            batch.DrawString(font, "Z", new Vector2(panelX + 13 + 48 + 96, 325 + 16), Color.White);
            batch.DrawString(font, "Collsion", new Vector2(panelX + 52, 244), Color.White);
            batch.DrawString(font, "Logical", new Vector2(panelX + 52, 244 + 38), Color.White);
            batch.DrawString(font, "Render to Depth", new Vector2(panelX + 156, 244), Color.White);
            batch.DrawString(font, "Level: " + editor.levelList[editor.currentLevelIndex], new Vector2(32, 9), Color.Black);
            batch.DrawString(font, "Selection", new Vector2(panelX - 54, 9), Color.White);
            batch.DrawString(font, "Asset Browser", new Vector2(panelX + 128 - (font.MeasureString("AssetBrowser").X / 2), 72), Color.White);
            if (editor.selectedObject != null)
            {
                if (editor.selectedObject.GetType() == typeof(SmokeEmitter))
                {
                    batch.DrawString(font, "Particle Count:\n" + (editor.currentHud as MegaParticleHud).ParticleSystem.LivingParticleCount, new Vector2(212, 18), Color.White);
                }
                else batch.DrawString(font, "Selected Object\nTriangle Count:\n" + editor.selectedObject.TriangleCount.ToString(), new Vector2(212, 18), Color.White);
            }

            if (showHotkeys == true)
                this.renderHotkeyList(batch);
        }

        /// <summary>Displays the hotkey list to screen</summary>
        private void renderHotkeyList(SpriteBatch batch)
        {
            Vector2 pos = new Vector2(0, 92);
            batch.DrawString(font, "Camera Move:  WASD", pos, Color.White);
            pos.Y += 12;
            batch.DrawString(font, "Camera Up/Down: QE", pos, Color.White);
            pos.Y += 12;
            batch.DrawString(font, "Camera Toggle:  Z", pos, Color.White);
            pos.Y += 12;
            batch.DrawString(font, "Select:  LeftMouse", pos, Color.White);
            pos.Y += 12;
            batch.DrawString(font, "Select Next: LeftControl + LeftMouse", pos, Color.White);
            pos.Y += 12;
            batch.DrawString(font, "UnSelect:  RightMouse", pos, Color.White);
            pos.Y += 12;
            batch.DrawString(font, "Copy:  Ctrl + C", pos, Color.White);
            pos.Y += 12;
            batch.DrawString(font, "Paste:  Ctrl + V", pos, Color.White);
            pos.Y += 12;
            batch.DrawString(font, "Finalize:  Enter", pos, Color.White);
            pos.Y += 12;
            batch.DrawString(font, "Delete:  Delete", pos, Color.White);
        }

        #endregion API

        #region Button Callbacks
        ///<summary>create a new mega-particle system</summary>
        public void Hud_newParticleSystem()
        {
            editor.selectedObject = null;
            editor.selectedObject = new SmokeEmitter(editor.content);
            editor.selectedObject.GenerateBoundingBox();
            editor.NullParameterHuds();
            editor.CreateHud();
            editor.selectedAlreadyFinalized = false;
        }

        ///<summary>toggle the object to render into the depth texture</summary>
        public void Hud_toggleDepth()
        {
            if (editor.selectedObject != null)
            {
                editor.toggleDepth(!editor.selectedObject.RenderDepth);
            }
        }

        /// <summary>Hud Button callback, copy the selected object</summary>
        public void Hud_copyObject()
        {
            if (editor.selectedObject == null)
                return;
            Object3D drawable = editor.selectedObject;
            editor.selectedObject = null;
            editor.selectedObject = drawable.GetCopy(editor.content);
            editor.selectedObject.GenerateBoundingBox();
            editor.selectedObject.UpdateBoundingBox();
        }

        /// <summary>create a new enemy spawn location</summary>
        public void Hud_newEnemySpawnPoint()
        {
            if(editor.selectedObject != null)
                return;

            EnemySpawnLocation loc = new EnemySpawnLocation();
            loc.Initialize(editor.content);
            loc.GenerateBoundingBox();
            loc.UpdateBoundingBox();
            editor.selectedObject = loc;
            editor.CreateHud();
        }

        /// <summary>Hud Button callback, creates a new light</summary>
        public void Hud_newLight()
        {
            editor.selectedObject = new PointLight();
            ((PointLight)editor.selectedObject).Initialize(editor.content);
            ((PointLight)editor.selectedObject).showBulb = true;
            ((PointLight)editor.selectedObject).WorldMatrix.Position = editor.point.WorldMatrix.Position;
            editor.selectedObject.GenerateBoundingBox();
            editor.selectedObject.UpdateBoundingBox();
            editor.NullParameterHuds();
            editor.CreateHud();
            editor.level.AddFromLevelEditor(editor.selectedObject);
            editor.selectedAlreadyFinalized = true;
        }

        /// <summary>Hud Button callback, creates a new trigger</summary>
        public void Hud_newTrigger()
        {
            editor.NullParameterHuds();
            editor.selectedObject = new Trigger();
            ((Trigger)editor.selectedObject).Initialize(editor.content);
            ((Trigger)editor.selectedObject).ID = editor.level.GetNextTriggerID();
            editor.selectedObject.GenerateBoundingBox();
            editor.selectedObject.UpdateBoundingBox();
            editor.CreateHud();
        }

        /// <summary>HUD Button Callback 
        /// - saves the current level</summary>
        public void Hud_saveLevel()
        {
            try
            {
                LevelXml output = editor.level.GetLevelXml();
                XmlSerializer xmlout = new XmlSerializer(typeof(LevelXml));
                TextWriter writer = new StreamWriter("content//data//" + editor.levelList[editor.currentLevelIndex] + ".xml");
                xmlout.Serialize(writer, output);
                writer.Close();
            }
            catch (Exception e)
            {
                Math_Physics.MyUtility.WriteExceptionInfo(e);
            }
        }

        /// <summary>init the save verification hud</summary>
        public void Hud_verifySave()
        {
            editor.verifyHud = new VerifyHud(editor, "Save current level?", Hud_saveLevel, null);
            editor.verifyHud.Initialize(editor.content);
        }

        /// <summary>ini the load verification hud</summary>
        public void Hud_verifyLoad()
        {
            editor.verifyHud = new VerifyHud(editor, "Load selected level?", Hud_loadLevel, null);
            editor.verifyHud.Initialize(editor.content);
        }

        /// <summary>HUD Button Callback 
        /// - loads the current level file selected</summary>
        public void Hud_loadLevel()
        {
            editor.currentObject = null;
            editor.selectedObject = null;
            editor.toggleSelector(true);

            try
            {
                LevelXml levelInput = new LevelXml();
                TextReader reader = new StreamReader("content//data//" + editor.levelList[editor.currentLevelIndex] + ".xml");
                XmlSerializer xml = new XmlSerializer(typeof(LevelXml));
                levelInput = (LevelXml)xml.Deserialize(reader);
                reader.Close();
                editor.level.LoadLevel(levelInput);
                editor.level.GenerateAllBounding();
                editor.renderer.RegisterLightList(ref editor.level.lightList);
            }
            catch (Exception e)
            {
                if(EngineFlags.drawDebug)
                    Math_Physics.MyUtility.WriteExceptionInfo(e);
            }
        }

        /// <summary>Hud Button Callback 
        /// - increments the current filepath in the display</summary>
        public void Hud_incrementLevelFile()
        {
            editor.currentLevelIndex++;
            if (editor.currentLevelIndex > editor.levelList.Length - 1)
            {
                editor.currentLevelIndex = 0;
            }
        }

        /// <summary>Hud button Callback 
        /// - Whatever object file the hud is currently looking at will be placed in front of the camera</summary>
        public void Hud_placeNewObject()
        {
            if (editor.currentObject == null)
                return;

            editor.selectedObject = editor.currentObject;
            editor.NullParameterHuds();
            editor.CreateHud();

            if (editor.selectedObject.GetType() == typeof(Trigger))
            {
                editor.toggleLogical(true);
                editor.toggleCollide(true);
            }
            if (editor.selectedObject.GetType() == typeof(Platform))
            {
                editor.toggleLogical(true);
                editor.toggleCollide(true);
            }

            editor.selectedObject.GenerateBoundingBox();
            editor.selectedObject.UpdateBoundingBox();
            editor.currentObject = null;
            editor.thumbnail.Initialize(null);
            editor.toggleSelector(false);
            editor.selectedAlreadyFinalized = false;
        }

        /// <summary>Hud button Callback 
        /// - The selected object will placed into the level's primary lists</summary>
        public void Hud_finalizeSelectedObject()
        {
            if (editor.selectedObject != null)
            {
                if (editor.level != null)
                    editor.level.AddFromLevelEditor(editor.selectedObject);
                editor.selectedObject = null;
                editor.toggleSelector(true);
                editor.toggleCollide(false);
                editor.toggleLogical(false);
                editor.NullParameterHuds();
            }
        }

        /// <summary>Hud button Callback 
        /// - The selected object will be deleted</summary>
        public void Hud_deleteSelectedObject()
        {
            if (editor.selectedObject == null)
                return;

            editor.level.DeleteFromEditor(editor.selectedObject);
            editor.selectedObject = null;
            editor.toggleSelector(true);
            editor.toggleCollide(false);
            editor.toggleLogical(false);
            editor.NullParameterHuds();
        }

        /// <summary>Hud button Callback
        /// - scroll the currently viewed model thumbnail to the left (decrement)</summary>
        public void Hud_thumnailScrollLeft()
        {
            editor.currentAssetIndex--;
            if (editor.currentAssetIndex < 0)
                editor.currentAssetIndex = editor.assetContainer.assetList.Count - 1;
            editor.currentObject = EditorAssetCreator.GetAsset(editor.content, editor.assetContainer.assetList[editor.currentAssetIndex]);
            editor.thumbnail.Initialize(editor.currentObject);
        }

        /// <summary>Hud button Callback
        /// - scroll the currently viewed model thumbnail to the right (increment)</summary>
        public void Hud_thumnailScrollRight()
        {
            editor.currentAssetIndex++;
            if (editor.currentAssetIndex > editor.assetContainer.assetList.Count -1)
                editor.currentAssetIndex = 0;
            editor.currentObject = EditorAssetCreator.GetAsset(editor.content, editor.assetContainer.assetList[editor.currentAssetIndex]);
            editor.thumbnail.Initialize(editor.currentObject);
        }

        #region world transformations
        /// <summary>Hud button callback
        /// - step the current model X scale up</summary>
        public void Hud_scaleXUp()
        {
            if (editor.selectedObject != null)
            {
                editor.selectedObject.WorldMatrix.sX += EditorScene.scaleStep;
            }
        }
        /// <summary>Hud button callback
        /// - step the current model Y scale up</summary>
        public void Hud_scaleYUp()
        {
            if (editor.selectedObject != null)
            {
                editor.selectedObject.WorldMatrix.sY += EditorScene.scaleStep;
            }
        }
        /// <summary>Hud button callback
        /// - step the current model Z scale up</summary>
        public void Hud_scaleZUp()
        {
            if (editor.selectedObject != null)
            {
                editor.selectedObject.WorldMatrix.sZ += EditorScene.scaleStep;
            }
        }
        /// <summary>Hud button callback
        /// - step the current model X rotation up</summary>
        public void Hud_rotateXUp()
        {
            if (editor.selectedObject != null)
            {
                editor.selectedObject.WorldMatrix.rX += EditorScene.rotationStep;
            }
        }
        /// <summary>Hud button callback
        /// - step the current model Y rotation up</summary>
        public void Hud_rotateYUp()
        {
            if (editor.selectedObject != null)
            {
                editor.selectedObject.WorldMatrix.rY += EditorScene.rotationStep;
            }
        }
        /// <summary>Hud button callback
        /// - step the current model Z rotation up</summary>
        public void Hud_rotateZUp()
        {
            if (editor.selectedObject != null)
            {
                editor.selectedObject.WorldMatrix.rZ += EditorScene.rotationStep;
            }
        }
        /// <summary>Hud button callback
        /// - step the current model X scale down</summary>
        public void Hud_scaleXDown()
        {
            if (editor.selectedObject != null)
            {
                editor.selectedObject.WorldMatrix.sX -= EditorScene.scaleStep;
            }
        }
        /// <summary>Hud button callback
        /// - step the current model Y scale down</summary>
        public void Hud_scaleYDown()
        {
            if (editor.selectedObject != null)
            {
                editor.selectedObject.WorldMatrix.sY -= EditorScene.scaleStep;
            }
        }
        /// <summary>Hud button callback
        /// - step the current model Z scale down</summary>
        public void Hud_scaleZDown()
        {
            if (editor.selectedObject != null)
            {
                editor.selectedObject.WorldMatrix.sZ -= EditorScene.scaleStep;
            }
        }
        /// <summary>Hud button callback
        /// - step the current model X rotation down</summary>
        public void Hud_rotateXDown()
        {
            if (editor.selectedObject != null)
            {
                editor.selectedObject.WorldMatrix.rX -= EditorScene.rotationStep;
            }
        }
        /// <summary>Hud button callback
        /// - step the current model Y rotation down</summary>
        public void Hud_rotateYDown()
        {
            if (editor.selectedObject != null)
            {
                editor.selectedObject.WorldMatrix.rY -= EditorScene.rotationStep;
            }
        }
        /// <summary>Hud button callback
        /// - step the current model Z rotation down</summary>
        public void Hud_rotateZDown()
        {
            if (editor.selectedObject != null)
            {
                editor.selectedObject.WorldMatrix.rZ -= EditorScene.rotationStep;
            }
        }
        /// <summary>Hud button callback
        /// - reset the scale and rotation of the current model</summary>
        public void Hud_resetWorld()
        {
            if (editor.selectedObject != null)
            {
                editor.selectedObject.WorldMatrix.Zero();
            }
        }
        /// <summary>Hud button callback
        /// - uniformly scale the currently model up</summary>
        public void Hud_scaleXyzUp()
        {
            if (editor.selectedObject != null)
            {
                editor.selectedObject.WorldMatrix.Scale += new Vector3(EditorScene.scaleStep);
            }
        }
        /// <summary>Hud button callback
        /// - uniformly scale the currently model down</summary>
        public void Hud_scaleXyzDown()
        {
            if (editor.selectedObject != null)
            {
                editor.selectedObject.WorldMatrix.Scale -= new Vector3(EditorScene.scaleStep);
            }
        }
        /// <summary>Hud button callback
        /// - add 45 degrees to current model rotation X</summary>
        public void Hud_rotate45X()
        {
            if (editor.selectedObject != null)
            {
                editor.selectedObject.WorldMatrix.rX += 45f;
            }
        }
        /// <summary>Hud button callback
        /// - add 45 degrees to current model rotation Y</summary>
        public void Hud_rotate45Y()
        {
            if (editor.selectedObject != null)
            {
                editor.selectedObject.WorldMatrix.rY += 45f;
            }
        }
        /// <summary>Hud button callback
        /// - add 45 degrees to current model rotation Z</summary>
        public void Hud_rotate45Z()
        {
            if (editor.selectedObject != null)
            {
                editor.selectedObject.WorldMatrix.rZ += 45f;
            }
        }
        #endregion world transformations

        /// <summary>Hud button callback
        /// - toggle this model's ability to collide with player objects</summary>
        public void Hud_toggleCollision()
        {
            if (editor.selectedObject != null)
            {
                if (editor.selectedObject.GetType() == typeof(Trigger) ||
                    editor.selectedObject.GetType() == typeof(ParticleSystem) ||
                    editor.selectedObject.GetType() == typeof(EnemySpawnLocation))
                    return;
            }

            bool outcome = ! editor.selectedObject.Collidable;
            if (editor.selectedObject != null)
            {
                editor.level.DeleteFromEditor(editor.selectedObject);
                editor.selectedObject.Collidable = !editor.selectedObject.Collidable;
                editor.level.AddFromLevelEditor(editor.selectedObject);
            }

            foreach (SpriteButton btn in buttonList)
            {
                if (btn.ID == GameIDList.Button_Editor_collision)
                {
                    if (outcome == false)
                        btn.CurrentFrameX = 0;
                    else btn.CurrentFrameX = 1;
                    return;
                }
            }
        }

        /// <summary>Hud button callback
        /// - toggle this model's ability to generate callbacks with integer ID when collision occurs
        /// - use for triggers, or any special logic stuff, but not for bullets or player physics, which will be coded in anyway</summary>
        public void Hud_toggleLogical()
        {
            //TODO - WHY WAS THIS LEFT EMPTY?
            //I DON'T KNOW BUT ITS WORKING!! OH WELL!
        }

        /// <summary>Switch to Dev scene - Note: unsaved data will be lost</summary>
        public void Hud_gotoDevScene()
        {
            editor.sceneManager.SetCurrentScene("dev");
        }

        /// <summary>Snap the camera to Positive X centered on the selected object</summary>
        public void Hud_camPosX()
        {
            editor.camera.SmoothStepTo(editor.point.WorldMatrix.Position,
                new Vector3(editor.point.WorldMatrix.X - 600, editor.camera.Position.Y, editor.point.WorldMatrix.Z),
                CameraDefaultCallback);

        }

        /// <summary>Snap the camera to Negative X centered on the selected object</summary>
        public void Hud_camNegX()
        {
            editor.camera.SmoothStepTo(editor.point.WorldMatrix.Position,
                new Vector3(editor.point.WorldMatrix.X + 600, editor.camera.Position.Y, editor.point.WorldMatrix.Z),
                CameraDefaultCallback);
        }

        /// <summary>Snap the camera to Positive Z centered on the selected object</summary>
        public void Hud_camPosZ()
        {
            editor.camera.SmoothStepTo(editor.point.WorldMatrix.Position,
                new Vector3(editor.point.WorldMatrix.X, editor.camera.Position.Y, editor.point.WorldMatrix.Z - 600),
                CameraDefaultCallback);
        }

        /// <summary>Snap the camera to Negative Z centered on the selected object</summary>
        public void Hud_camNegZ()
        {
            editor.camera.SmoothStepTo(editor.point.WorldMatrix.Position,
                new Vector3(editor.point.WorldMatrix.X, editor.camera.Position.Y, editor.point.WorldMatrix.Z + 600),
                CameraDefaultCallback);
        }

        /// <summary>Snap the camera to Negative Y centered on the selected object</summary>
        public void Hud_lookDown()
        {
            editor.camera.SmoothStepTo(editor.point.WorldMatrix.Position,
                new Vector3(editor.point.WorldMatrix.X, editor.point.WorldMatrix.Y + 600, editor.point.WorldMatrix.Z - 1),
                CameraDefaultCallback);
        }

        /// <summary>After the camera gets to where it was going, this is called</summary>
        public void CameraDefaultCallback()
        {
            editor.camera.Behavior = new StationaryCamera();
        }

        #endregion Button Callbacks
    }
}
