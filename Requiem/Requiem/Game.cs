using Engine;
using Engine.Managers;
using Engine.Managers.Factories;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Requiem.Scenes;
using Requiem.Scenes.Level1;
using Requiem.Spells;
using Requiem.Entities.Enemy;
using Engine.Managers.Camera;
using System.Threading.Tasks;

namespace Requiem
{
    /// <summary>This is the main type for your game</summary>
    /// <author>Daniel Cuccia</author>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SceneManager sceneManager;
        Renderer renderer;

        /// <summary>Game Constructor</summary>
        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Window.Title = "Requiem";
            Window.AllowUserResizing = false;

            if (EngineFlags.runLowDef)
            {
                graphics.PreferredBackBufferWidth = 1024;
                graphics.PreferredBackBufferHeight = 768;
            }
            else
            {
                graphics.PreferredBackBufferWidth = 1280;
                graphics.PreferredBackBufferHeight = 800;
            }
            graphics.ApplyChanges();

            // !!!! --- UnComment this Line to unlock the max refresh rate --- !!!! //
            //IsFixedTimeStep = graphics.SynchronizeWithVerticalRetrace = false;

            IsMouseVisible = true;
        }

        /// <summary>Allows the game to perform any initialization it needs to before starting to run.
        /// this is where the Singletons are initialized, and scenemanager is populated</summary>
        protected override void Initialize()
        {
            base.Initialize();

            EffectManager effectManager = EffectManager.getInstance();
            effectManager.Initialize(Content);

            renderer = Renderer.getInstance();
            renderer.Initialize(graphics.GraphicsDevice, Content, System.Environment.TickCount);

            PhysXEngine physx = PhysXEngine.Instance;
            physx.Initialize(graphics.GraphicsDevice, Content);

            AudioManager audio = AudioManager.Instance;
            audio.Initialize(Content);

            TextureManager texFactory = TextureManager.getInstance();
            texFactory.Initialize(Content);

            MaterialBinder matBinder = MaterialBinder.getInstance();
            matBinder.Initialize(Content);

            FontManager fontFactory = FontManager.getInstance();
            fontFactory.Initialize(Content);

            sceneManager = new SceneManager(this, spriteBatch, graphics);
            sceneManager.AddScene(new SplashScene1(this, "splash1", sceneManager));
            sceneManager.AddScene(new SplashScene2(this, "splash2", sceneManager));
            sceneManager.AddScene(new SplashScene3(this, "splash3", sceneManager));
            sceneManager.AddScene(new CreditsScene(this, "credits", sceneManager));
            
            sceneManager.AddScene(new Level1a(this, "level1a", sceneManager));
            sceneManager.AddScene(new Level1b(this, "level1b", sceneManager));
            sceneManager.AddScene(new Level1c(this, "level1c", sceneManager));
            sceneManager.AddScene(new Level1d(this, "level1d", sceneManager));
            sceneManager.AddScene(new Level1e(this, "level1e", sceneManager));

            sceneManager.AddScene(new PauseScene(this, "pause", sceneManager));
            sceneManager.AddScene(new MainMenuScene(this, "main", sceneManager));

            sceneManager.SetCurrentScene("splash1");

            SpellManager spellManager = SpellManager.GetInstance();
            spellManager.Initialize(Content, sceneManager);

            EnemyManager enemyManager = EnemyManager.Instance;
            enemyManager.Initialize(Content, sceneManager);

            if(GamePad.GetState(PlayerIndex.One).IsConnected == true)
                IsMouseVisible = false;
        }

        /// <summary>LoadContent</summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>UnloadContent</summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>Game Logic</summary>
        protected override void Update(GameTime gameTime)
        {
            sceneManager.Input(Keyboard.GetState(),
                Mouse.GetState(),
                GamePad.GetState(PlayerIndex.One));

            sceneManager.Update(gameTime);

            renderer.Update(gameTime);

            base.Update(gameTime);

            //Task.Factory.StartNew(() => { System.GC.Collect(2, System.GCCollectionMode.Forced); });
            //System.GC.Collect(2, System.GCCollectionMode.Forced);
        }

        /// <summary>This is called when the game should draw itself.</summary>
        protected override void Draw(GameTime gameTime)
        {
            renderer.Begin3D();
            sceneManager.Render3D(gameTime);

            renderer.Begin2D(spriteBatch);
            sceneManager.Render2D(gameTime, spriteBatch);

            renderer.End2D(spriteBatch);

            base.Draw(gameTime);
        }

        /// <summary>get the current mouse ray according to the elaborte game-gam</summary>
        /// <param name="viewport">the current viewport</param>
        /// <param name="camera">the game-cam</param>
        /// <returns>current mouse ray</returns>
        public static Ray GetMouseRay(Viewport viewport, Camera camera)
        {
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            Vector3 nearPoint = new Vector3(mousePosition, 0);
            Vector3 farPoint = new Vector3(mousePosition, 1);

            nearPoint = viewport.Unproject(nearPoint, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
            farPoint = viewport.Unproject(farPoint, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            return new Ray(nearPoint, direction);
        }
    }
}
