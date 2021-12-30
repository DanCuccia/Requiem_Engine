using System.Collections.Generic;
using Engine.Drawing_Objects;
using Engine.Managers;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Engine.Scenes.HUD
{
    /// <summary>Base Hud class to be used with level editor</summary>
    /// <author>Daniel Cuccia</author>
    public abstract class HudBase
    {
        /// <summary>the null texture used to draw backgrounds</summary>
        protected Texture2D blank = TextureManager.getInstance().GetTexture("sprites//blank");
        /// <summary>global game font</summary>
        protected SpriteFont font = Renderer.getInstance().GameFont;

        /// <summary>main list of buttons</summary>
        protected List<SpriteButton> buttonList = new List<SpriteButton>();
        /// <summary>main list of sliders</summary>
        protected List<Slider> sliderList = new List<Slider>();
        /// <summary>main list of animated sprites</summary>
        protected List<Sprite> spriteList = new List<Sprite>();

        /// <summary>the background rect</summary>
        protected Rectangle rect;
        /// <summary>Background Accessor</summary>
        public Rectangle Rect
        { get { return this.rect; } }

        /// <summary>pointer to the level editor scene</summary>
        protected EditorScene editor;

        /// <summary>default ctor</summary>
        /// <param name="editor">Level Editor Scene</param>
        public HudBase(EditorScene editor)
        {
            this.editor = editor;
            rect = this.LoadRectangle();
        }

        /// <summary>initialize buttons and sliders now that derived classes are constructed</summary>
        /// <param name="content">content manager</param>
        public void Initialize(ContentManager content)
        {
            this.LoadButtons(content);
            this.LoadSliders(content);
            this.LoadSprites(content);
        }

        /// <summary>load the background rectangle</summary>
        /// <remarks>override this for custom sized backgrounds</remarks>
        /// <returns>custom sized rectangle</returns>
        protected virtual Rectangle LoadRectangle()
        {
            return new Rectangle(Renderer.getInstance().Device.Viewport.Width - 256,
            Renderer.getInstance().Device.Viewport.Height - 380, 256, 380);
        }

        /// <summary>load the buttons from derived huds</summary>
        /// <param name="content">content manager</param>
        protected abstract void LoadButtons(ContentManager content);

        /// <summary>load the sliders from derived huds</summary>
        /// <param name="content">content manager</param>
        protected abstract void LoadSliders(ContentManager content);

        /// <summary>load the list of sprites from derived huds</summary>
        /// <param name="content">content manager</param>
        protected abstract void LoadSprites(ContentManager content);

        /// <summary>process stuff</summary>
        /// <param name="ms">mouse state</param>
        public void Input(MouseState ms)
        {
            MyUtility.ProcessButtonList(ms, buttonList);
            MyUtility.processSliders(ms, sliderList);
        }

        /// <summary>render everything</summary>
        /// <param name="batch">xna spritebatch</param>
        public void Render2D(SpriteBatch batch)
        {
            batch.Draw(blank, rect, MyColors.AlphaBlack);
            foreach (Sprite spr in spriteList)
                spr.Draw(batch);
            foreach (SpriteButton btn in buttonList)
                btn.Draw(batch);
            foreach (Slider slider in sliderList)
                slider.Render2D(batch);
            this.Render2DExtra(batch);
        }

        /// <summary>provides an optional way to render extra stuff</summary>
        /// <param name="batch">sprite batch</param>
        protected virtual void Render2DExtra(SpriteBatch batch) { }

        /// <summary>optional update call to override</summary>
        /// <param name="time">current time</param>
        public virtual void Update(GameTime time) { }
        /// <summary>optional render3D call</summary>
        public virtual void Render3D() { }

        /// <summary>register a new slider to override</summary>
        /// <param name="slider">fully contrusted slider</param>
        protected void AddSlider(Slider slider)
        {
            if (slider != null)
                sliderList.Add(slider);
        }

        /// <summary>register a new button</summary>
        /// <param name="btn">fully contrusted button</param>
        protected void AddButton(SpriteButton btn)
        {
            if (btn != null)
                buttonList.Add(btn);
        }
    }
}
