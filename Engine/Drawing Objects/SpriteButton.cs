using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Managers;

namespace Engine.Drawing_Objects
{
    /// <summary>Using the 2D Sprite as a base class, this wraps functionality
    /// to make it a button</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class SpriteButton : Sprite
    {
        #region Member Variables

        /// <summary>delegate when button is pressed</summary>
        public delegate void onPressCallback();
        onPressCallback pressCallback = null;

        /// <summary>delegate when button is release</summary>
        public delegate void onReleaseCallback();
        onReleaseCallback releaseCallback = null;

        bool isPressed = false;
        bool selectable = true;
        bool toggleFrames = true;

        string label = null;

        #endregion Member Variables

        #region Initialization

        /// <summary>Default CTOR</summary>
        public SpriteButton(int id = -1)
            : base(id)
        {        }

        /// <summary>Load assets and init base Sprite class parameters</summary>
        /// <param name="filepath">filepath to the 2-cell sprite sheet you want to use as a button</param>
        public void Initialize(string filepath)
        {
            base.Initialize(filepath, new Point(2, 1));
            base.isAnimating = false;
        }

        /// <summary>another initializer</summary>
        /// <param name="filepath">image filepath</param>
        /// <param name="sheetSize">custom sheetsize</param>
        public void InitializeSpriteButton(string filepath, Point sheetSize)
        {
            base.Initialize(filepath, sheetSize);
            base.isAnimating = false;
        }

        /// <summary>Load assets, this will print the "label" on top of your sprite button</summary>
        /// <param name="filepath">image filepath</param>
        /// <param name="label">button label</param>
        public void Initialize(string filepath, string label)
        {
            base.Initialize(filepath, new Point(2, 1));
            base.isAnimating = false;
            this.label = label;
        }

        #endregion Initialization

        #region API

        /// <summary>Sets the delegate callbacks, either parameter may be null</summary>
        /// <param name="pressLogic">on press callback, may be null</param>
        /// <param name="releaseLogic">on release callback, may be null</param>
        public void setExecution(onPressCallback pressLogic, onReleaseCallback releaseLogic)
        {
            pressCallback = pressLogic;
            releaseCallback = releaseLogic;
        }

        /// <summary>OnPressCallback is activated if not null, and sprite frames are toggled</summary>
        public void OnPress()
        {
            if (selectable == false)
                return;
            
            if(toggleFrames == true)
                base.currentFrame.X = 1;

            if (pressCallback != null && isPressed == false)
                this.pressCallback();

            isPressed = true;
        }

        /// <summary>onReleaseCallback is activated if not null, and sprite frames are toggled</summary>
        public void OnRelease()
        {
            if (selectable == false)
                return;

            if(toggleFrames == true)
                base.currentFrame.X = 0;

            if(releaseCallback != null && isPressed == true)
                this.releaseCallback();

            isPressed = false;
        }

        /// <summary>Undoes any framing to this button's default state</summary>
        public void ToDefault()
        {
            if(toggleFrames == true)
                base.currentFrame.X = 0;
            isPressed = false;
        }

        /// <summary>The Main 2d draw call - overriden to modify opacity if not selectable</summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isVisible == false)
                return;

            if (selectable == false)
                opacity = .7f;
            else opacity = 1f;

            spriteBatch.Draw(texture,
                position,
                new Rectangle(
                    currentFrame.X * texture.Width / sheetSize.X,
                    currentFrame.Y * texture.Height / sheetSize.Y,
                    texture.Width / sheetSize.X,
                    texture.Height / sheetSize.Y),
                Color.FromNonPremultiplied(255, 255, 255, (int)(opacity * 255)),
                rotation, new Vector2(0), scale, SpriteEffects.None, 0);

            if (label != null)
            {
                spriteBatch.DrawString(Renderer.getInstance().GameFont, this.label, position + new Vector2(10, 10), Color.Black);
            }
        }

        #endregion API

        /// <summary>toggle clickable</summary>
        public bool Selectable
        {
            set { selectable = value; }
            get { return selectable; }
        }
        /// <summary>will not autochange frames</summary>
        public bool ToggleFrames
        {
            set { toggleFrames = value; }
            get { return toggleFrames; }
        }
        /// <summary>The optional overlayed name of this button</summary>
        public string Label
        {
            set { this.label = value; }
            get { return this.label; }
        }
    }
}
