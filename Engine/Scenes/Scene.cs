using Engine.Game_Objects;
using Engine.Managers;
using Engine.Scenes.IScene_Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Engine.Scenes
{
    /// <summary>Base Scene object</summary>
    /// <author>Daniel Cuccia</author>
    public abstract class Scene
    {
        /// <summary>name and identifier</summary>
        public string               name;
        /// <summary>will not run unless this is toggled</summary>
        public bool                 isInitialized;
        /// <summary>reference to xna content manager</summary>
        public ContentManager       content;
        /// <summary>reference to the scene manager</summary>
        public SceneManager         sceneManager;
        /// <summary>reference to the hardware device</summary>
        public GraphicsDevice       device;
        /// <summary>renderer reference</summary>
        public Renderer             renderer;
        /// <summary>animating in logic</summary>
        public ISceneAnimateIn      animateIn;
        /// <summary>animating out logic</summary>
        public ISceneAnimateOut     animateOut;

        /// <summary>Default CTOR</summary>
        /// <param name="g">main game pointer</param>
        /// <param name="name">string identifier</param>
        /// <param name="sm">reference to the sceneManager</param>
        public Scene(Microsoft.Xna.Framework.Game g, string name, SceneManager sm)
        {
            content = g.Content;
            this.device = g.GraphicsDevice;
            this.name = name;
            this.sceneManager = sm;
            this.renderer = Renderer.getInstance();
            isInitialized = false;
            animateIn = null;
            animateOut = null;
        }
        /// <summary>abstract sceneManager call</summary>
        public abstract void Initialize();
        /// <summary>abstract sceneManager call</summary>
        public abstract void Input(KeyboardState kb, MouseState ms, GamePadState gp1);
        /// <summary>abstract sceneManager call</summary>
        public abstract void Update(GameTime time);
        /// <summary>abstract sceneManager call</summary>
        public abstract void Render2D(GameTime time, SpriteBatch batch);
        /// <summary>abstract sceneManager call</summary>
        public abstract void Render3D(GameTime time);
        /// <summary>abstract sceneManager call</summary>
        public abstract void RenderDebug2D(GameTime time, SpriteBatch batch);
        /// <summary>abstract sceneManager call</summary>
        public abstract void RenderDebug3D(GameTime time);
        /// <summary>abstract sceneManager call</summary>
        public abstract void Close();
        /// <summary>Override this in game-scenes to assign incoming triggers</summary>
        /// <param name="trigger">already constructed trigger, only assign it's callback</param>
        public virtual void AssignTrigger(ref Trigger trigger) { }
    }
}
