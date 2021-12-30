using System;
using System.Collections.Generic;
using Engine.Drawing_Objects;
using Engine.Drawing_Objects.Materials;
using Engine.Drawing_Objects.ParticleSystems;
using Engine.Managers;
using Engine.Managers.Camera;
using Engine.Scenes;
using Engine.Scenes.IScene_Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Requiem.Scenes.SceneAnimation;
using Engine.Math_Physics;

namespace Requiem.Scenes
{
    /// <summary>Main Title Screen</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class SplashScene3 : Scene
    {
        bool pressed = false;

        OpeningFire fire;
        StaticObject3D title;
        Quad3D quad;

        Camera camera;

        List<PointLight> lightList;

        float time = 0;

        public SplashScene3(Microsoft.Xna.Framework.Game g, string name, SceneManager sm)
            :base(g, name, sm) { }

        public override void Initialize()
        {
            base.animateIn = new SceneAnimateIn_BlackFade(this, animInOnComplete);
            camera = new Camera(device.Viewport);
            camera.Initialize(new StationaryCamera());
            renderer.Camera = camera;
            camera.Position = new Vector3(0f, 70f, -700f);
            camera.LookAtTarget = new Vector3(0f, 40f, 0f);

            fire = new OpeningFire(content);
            title = new StaticObject3D();
            title.Initialize(content, "models//requiemTitleModel");
            title.GenerateBoundingBox();
            title.RenderDepth = true;
            NormalMappedMaterial mat = new NormalMappedMaterial(title, 
                TextureManager.getInstance().GetTexture("textures//marble_diffuse_1024"), 
                TextureManager.getInstance().GetTexture("textures//marble_normal_1024"));
            mat.Specular = 0f;
            mat.Shine = 0f;
            title.Material = mat;
            title.WorldMatrix.rY += 180f;
            title.WorldMatrix.UniformScale = 2f;
            title.WorldMatrix.Z -= 70f;
            title.UpdateBoundingBox();

            quad = new Quad3D(Quad3DOrigin.CENTER);
            quad.Initialize(TextureManager.getInstance().GetTexture("textures//pressAnyButton"));
            quad.GenerateBoundingBox();
            quad.WorldMatrix.Z += 100f;
            quad.WorldMatrix.UniformScale = .333f;
            quad.WorldMatrix.Y -= 250f;

            quad.WorldMatrix.rY = 180 + MathHelper.ToDegrees((float)Math.Atan2(quad.WorldMatrix.X - camera.Position.X,
                            quad.WorldMatrix.Z - camera.Position.Z));

            loadLights();
            renderer.RegisterLightList(ref lightList);

            renderer.BloomSettings.BloomSaturation = 1f;
            renderer.BloomSettings.BloomThreshold = .5f;
            renderer.BloomSettings.BlurAmount = 5f;
            renderer.ClearColor = Color.Black;

            isInitialized = true;
            pressed = false;
        }

        private void loadLights()
        {
            lightList = new List<PointLight>();
            //yellows
            PointLight light = new PointLight();
            light.Initialize(content);
            light.color = new Vector4(1f, .966f, 0f, 1f);
            light.WorldMatrix.Position = new Vector3(-300f, -50f, -25f);
            light.falloff = 300f;
            lightList.Add(light);

            light = new PointLight();
            light.Initialize(content);
            light.color = new Vector4(1f, .966f, 0f, 1f);
            light.WorldMatrix.Position = new Vector3(300f, -50f, -25f);
            light.falloff = 300f;
            lightList.Add(light);

            light = new PointLight();
            light.Initialize(content);
            light.color = new Vector4(1f, .966f, 0f, 1f);
            light.WorldMatrix.Position = new Vector3(0f, -50f, -25f);
            light.falloff = 300f;
            lightList.Add(light);

            light = new PointLight();
            light.Initialize(content);
            light.color = new Vector4(1f, 0f, 0f, 1f);
            light.WorldMatrix.Position = new Vector3(-300f, 300f, -25f);
            light.falloff = 300f;
            lightList.Add(light);

            light = new PointLight();
            light.Initialize(content);
            light.color = new Vector4(1f, 0f, 0f, 1f);
            light.WorldMatrix.Position = new Vector3(300f, 300f, -25f);
            light.falloff = 300f;
            lightList.Add(light);

            light = new PointLight();
            light.Initialize(content);
            light.color = new Vector4(1f, 0f, 0f, 1f);
            light.WorldMatrix.Position = new Vector3(0f, 300f, -25f);
            light.falloff = 300f;
            lightList.Add(light);
        }

        private void animInOnComplete()
        {
            base.animateIn = null;
        }

        private void animOutOnComplete()
        {
            sceneManager.SetCurrentScene("main");
        }

        public override void Close()
        {
            isInitialized = false;
            fire = null;
            title = null;
            time = 0f;
            renderer.UnRegisterLightList();
            if (lightList != null)
            {
                lightList.Clear();
                lightList = null;
            }
            camera = null;
            pressed = false;

            renderer.BloomSettings = BloomSettings.PresetSettings[5];
            renderer.ClearColor = Color.DarkSlateGray;

            TextureManager.getInstance().ClearLists();
        }

        public override void Input(KeyboardState kb, MouseState ms, GamePadState gp)
        {
            if (kb.GetPressedKeys().Length > 0)
                pressed = true;

            if (MyUtility.GamePadPressedKeyCount(gp) > 0)
                pressed = true;
            

            if (kb.GetPressedKeys().Length == 0 && pressed == true)
            {
                exitScene();
            }
            if (MyUtility.GamePadPressedKeyCount(gp) == 0 && pressed == true)
            {
                exitScene();
            }
        }

        private void exitScene()
        {
            pressed = false;
            if (animateOut == null)
                animateOut = new TitleSceneAnimOut(this, animOutOnComplete);
        }

        public override void Update(GameTime time)
        {
            this.time += time.ElapsedGameTime.Milliseconds;
            camera.Update(time);

            fire.Update(ref camera, time);
            title.Update(ref camera, time);

            if (this.animateOut == null)
            {
                renderer.BloomSettings.BlurAmount = 1 + (float)Math.Abs((Math.Sin(time.TotalGameTime.TotalSeconds * 0.98f) * 30f));
                renderer.BloomSettings.BloomThreshold = .2f + (float)Math.Abs((Math.Sin(time.TotalGameTime.TotalSeconds * 0.98f) * 1f));
                if (renderer.BloomSettings.BloomThreshold > 1.0f)
                    renderer.BloomSettings.BloomThreshold = 1f;
            }
        }

        public override void Render3D(GameTime time)
        {
            fire.Render();
            title.Render();
            quad.Render();
        }

        public override void RenderDebug3D(GameTime time) { }
        public override void Render2D(GameTime time, SpriteBatch batch) { }
        public override void RenderDebug2D(GameTime time, SpriteBatch batch) { }
    }
}
