using System.Collections.Generic;
using Engine.Drawing_Objects;
using Engine.Game_Objects.PreFabs;
using Engine.Managers;
using Engine.Managers.Camera;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Engine.Scenes.HUD
{
    /// <summary>Creates a small viewport with an orbiting camera,
    /// using to display a 3d thumbnail of a model inside the level editor</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class ThumbnailRenderer
    {
        Camera camera;
        EditorScene editor;//ref
        Renderer renderer;//ref
        Object3D drawable;
        List<SpriteButton> buttonList;
        SpriteFont font;
        Vector2 txtPos1;
        Vector2 txtPos2;
        /// <summary>OrbitCamera uses a worldMatrix as origin, 
        /// so we have a dummy one here for it to look at</summary>
        WorldMatrix dummyWorld = new WorldMatrix();

        /// <summary>Default CTOR</summary>
        /// <param name="editor">pointer to the level editor</param>
        public ThumbnailRenderer(EditorScene editor)
        {
            this.editor = editor;
            this.renderer = Renderer.getInstance();
            camera = new Camera(new Viewport(64 + editor.device.Viewport.Width - 256, 92, 128, 128));
            camera.Initialize(new OrbitCamera(ref camera, ref dummyWorld));
            //camera.Behavior = new OrbitCamera(ref camera, ref dummyWorld);
            font = Renderer.getInstance().GameFont;
            this.loadButtons();
        }

        /// <summary>load buttons</summary>
        private void loadButtons()
        {
            buttonList = new List<SpriteButton>();

            SpriteButton btn = new SpriteButton();
            btn.Initialize("sprites//editor//LE_btnDownSmall");
            btn.Position = new Vector2(64 + editor.device.Viewport.Width - 256, 92 + 128 - btn.FrameSize.Y);
            btn.setExecution(null, camDistBack);
            buttonList.Add(btn);

            btn = new SpriteButton();
            btn.Initialize("sprites//editor//LE_btnUpSmall");
            btn.Position = new Vector2(64 + editor.device.Viewport.Width - 256, 92 + 128 - (btn.FrameSize.Y*2) - 12);
            btn.setExecution(null, camDistForward);
            buttonList.Add(btn);

            txtPos1 = new Vector2(64 + editor.device.Viewport.Width - 256 + 3, 92 + 128 - (btn.FrameSize.Y * 2) - 1);

            btn = new SpriteButton();
            btn.Initialize("sprites//editor//LE_btnUpSmall");
            btn.Position = new Vector2(editor.device.Viewport.Width - 64 - btn.FrameSize.X + 1, 92 + 128 - (btn.FrameSize.Y * 2) - 12);
            btn.setExecution(null, camHeightUp);
            buttonList.Add(btn);

            txtPos2 = new Vector2(editor.device.Viewport.Width-64 - font.MeasureString("Height").X, 
                92 + 128 - (btn.FrameSize.Y * 2) - 1);

            btn = new SpriteButton();
            btn.Initialize("sprites//editor//LE_btnDownSmall");
            btn.Position = new Vector2(editor.device.Viewport.Width - 64 - btn.FrameSize.X, 92 + 128 - btn.FrameSize.Y);
            btn.setExecution(null, camHeightDown);
            buttonList.Add(btn);
        }

        private void camDistBack()
        {
            if (camera.Behavior.GetType() == typeof(OrbitCamera))
            {
                (camera.Behavior as OrbitCamera).OrbitDistanace += 50f;
            }
            //camera.OrbitDistance += 50f;
        }

        private void camDistForward()
        {
            if (camera.Behavior.GetType() == typeof(OrbitCamera))
            {
                OrbitCamera oc = (camera.Behavior as OrbitCamera);
                oc.OrbitDistanace -= 50f;
                if (oc.OrbitDistanace <= 0)
                    oc.OrbitDistanace = 50f;
            }
                
            //camera.OrbitDistance -= 50f;
            //if (camera.OrbitDistance < 0)
            //    camera.OrbitDistance = 50f;
        }

        private void camHeightUp()
        {
            if (camera.Behavior.GetType() == typeof(OrbitCamera))
            {
                OrbitCamera oc = (camera.Behavior as OrbitCamera);
                oc.OrbitHeight += 20f;
            }
        }

        private void camHeightDown()
        {
            if (camera.Behavior.GetType() == typeof(OrbitCamera))
            {
                OrbitCamera oc = (camera.Behavior as OrbitCamera);
                oc.OrbitHeight -= 20f;
            }
        }

        /// <summary>Initialize this renderer to display an Object3D</summary>
        /// <param name="obj">what to draw</param>
        public void Initialize(Object3D obj)
        {
            drawable = obj;
            //camera.Behavior = new OrbitCamera(ref camera, ref w);
        }

        /// <summary>main update call</summary>
        /// <param name="time">current game time</param>
        public void Update(GameTime time)
        {
            //if (drawable == null && camera.Behavior.GetType() != typeof(StationaryCamera))
            //    camera.Behavior = new StationaryCamera(ref camera);

            camera.Update(time);

            if (drawable != null)
                drawable.Update(ref camera, time);
        }

        /// <summary>Process input</summary>
        public void Input(KeyboardState kb, MouseState ms)
        {
            MyUtility.ProcessButtonList(ms, buttonList);
        }

        /// <summary>Because this viewport is completely seperated, we skip material batching
        /// and implicitly render our model when this is called</summary>
        public void Render3D()
        {
            if (drawable == null)
                return;

            Camera origCam = renderer.Camera;
            Viewport origViewport = renderer.Device.Viewport;

            renderer.Camera = camera;
            renderer.Device.Viewport = camera.Viewport;

            render();

            renderer.Camera = origCam;
            renderer.Device.Viewport = origViewport;
        }

        /// <summary>draw 2D images</summary>
        /// <param name="batch"></param>
        public void Render2D(SpriteBatch batch)
        {
            foreach (SpriteButton btn in buttonList)
                btn.Draw(batch);

            batch.DrawString(font, "Dist", txtPos1, Color.White);
            batch.DrawString(font, "Height", txtPos2, Color.White);
        }

        /// <summary>Apply shader, update its values, apply technique, and finally draw the model
        /// - this essentially is skipping material batch automation</summary>
        private void render()
        {
            if (drawable == null)
                return;

            if (drawable.Material != null)
            {
                drawable.Material.InitializeShader();
                drawable.Material.PreRenderUpdate();
                drawable.Material.ApplyTechnique();
                drawable.RenderImplicit(drawable.Material.Effect);
            }
            else// if (drawable.GetType() == typeof(PreFabrication))
            {
                (drawable as PreFabrication).BatchSkipRender();
            }
        }
    }
}
