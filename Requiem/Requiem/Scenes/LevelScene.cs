using Engine.Game_Objects;
using Engine.Managers;
using Engine.Managers.Camera;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Requiem.Entities;
using Requiem.Movement;
using Requiem.Spells;
using Requiem.Entities.Enemy;
using StillDesign.PhysX;
using Microsoft.Xna.Framework.Graphics;
using Requiem.UI.HUD;

namespace Requiem.Scenes
{
    /// <summary>This abstract extension of Scene is what each Level derives from,
    /// we encapsulate all common objects that each level needs in order to run</summary>
    public abstract class LevelScene : Engine.Scenes.Scene
    {
        public Level level;
        public Camera camera;
        public Player player;
        protected SpellManager spellManager = SpellManager.GetInstance();
        protected EnemyManager enemyManager = EnemyManager.Instance;
        protected HeadsUpDisplay hud;

        /// <summary>Default CTOR</summary>
        public LevelScene(Game g, string name, SceneManager sm)
            : base(g, name, sm) { }

        /// <summary>Completely builds the base LevelScene</summary>
        /// <param name="filename">filepath to the level xml file, starting with "content//..."</param>
        protected void SceneInitialize(string filename)
        {
            this.LoadLevel(filename);
            this.LoadPlayer();
            this.CreateCamera();
            player.PlayerCamera = camera;

            hud = new HeadsUpDisplay(player);
            hud.Initialize(sceneManager.Game.Window.ClientBounds);
        }

        /// <summary>create and load the player</summary>
        private void LoadPlayer()
        {
            player = new Player();
            player.Initialize(content);
            player.WorldMatrix.Position = level.currentSpawnLocation;
            player.WorldMatrix.Y += 100f;
            player.zLock = level.currentSpawnLocation.Z;
            player.Movement.ForceReset();
            player.GenerateBoundingBox();
        }

        /// <summary>create the main camera for the level</summary>
        private void CreateCamera()
        {
            camera = new Camera(base.device.Viewport);
            WorldMatrix w = player.WorldMatrix;
            camera.Initialize(new AxisLockCamera(ref camera, ref w, new Vector3(0, 0, 1), new Vector3(0, 200, 0), 500));
            renderer.Camera = camera;
        }

        /// <summary>Load the level object (must call this from derived levels</summary>
        /// <param name="levelFilename">filepath to the level eg.(content//data//Level1//Level1.xml)</param>
        private void LoadLevel(string levelFilename)
        {
            level = new Level();
            level.Initialize(content, device, ref camera);
            level.LoadFromFile(levelFilename);
            enemyManager.LoadFromLevel(level);
        }

        /// <summary>release all base level objects for garbage collection</summary>
        public override void Close()
        {
            level.Release();
            level = null;
            camera = null;
            enemyManager.Release();
        }

        public override void Input(KeyboardState kb, MouseState ms, GamePadState gp)
        {
            camera.Input(kb, ms);
            player.Input(kb, ms, gp);

            if (kb.IsKeyDown(Keys.Escape) || gp.IsButtonDown(Buttons.Start))
            {
                sceneManager.ShowOverlapScene("pause");
            }
        }

        /// <summary>update the base level objects</summary>
        /// <param name="time"></param>
        public override void Update(GameTime time)
        {
            if (player != null)
            {
                player.Movement.Update(time);
                player.Update(ref camera, time);
                player.UpdateBoundingBox();
            }
            if (level != null)
            {
                level.Update(ref camera, time);
                level.TestTriggers(player.OBB);
            }

            if(camera != null)
                camera.Update(time);

            if(spellManager != null)
                spellManager.Update(camera, time);

            if (spellManager.CheckCollisions(player))
            {
                camera.Shake(1.5f, 500f);
            }

            if (enemyManager != null)
            {
                enemyManager.Update(ref camera, time);

                foreach (Enemy e in enemyManager.ActiveEnemyList)
                {
                    spellManager.CheckCollisions(e);
                }
            }

            if (hud != null)
                hud.Update();
        }

        /// <summary>render all base level objects</summary>
        public override void Render3D(GameTime time)
        {
            if(level != null)
                level.Render3D();

            if(player != null)
                player.Render();

            if (spellManager != null)
                spellManager.Draw();

            if (enemyManager != null)
                enemyManager.Render3D();
        }

        /// <summary>render all 3D debugging</summary>
        public override void RenderDebug3D(GameTime time)
        {
            if(level != null)
                level.RenderDebug3D();

            if (player != null)
                player.RenderDebug();

            PhysXEngine.Instance.Render3D();

            if(spellManager != null)
                spellManager.DrawDebug();

            if (enemyManager != null)
                enemyManager.RenderDebug3D();
        }

        public override void Render2D(GameTime time, SpriteBatch batch)
        {
            hud.Draw(batch);
        }

        public override void RenderDebug2D(GameTime time, SpriteBatch batch)
        { }

        /// <summary>callback when the player reaches his spawn point after dying</summary>
        public void PlayerDies()
        {
            WorldMatrix w = player.WorldMatrix;
            Actor a = player.Actor;
            player.Movement = new MoveTo(ref w, ref a, level.currentSpawnLocation, ReInitPlayerMovement);
            player.Actor.Sleep();
        }

        private void ReInitPlayerMovement()
        {
            WorldMatrix w = player.WorldMatrix;
            Actor a = player.Actor;
            player.Movement = new MovementXAxis(ref w, ref a);
            player.Actor.WakeUp();
            player.Movement.ForceReset();
        }
    }
}
