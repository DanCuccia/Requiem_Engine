using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Managers;
#pragma warning disable 1591

namespace Engine.Drawing_Objects
{
    /// <summary>The base 2D animating or non-animating sprite object</summary>
    /// <author>Daniel Cuccia</author>
    public class Sprite
    {
        protected int           id                  = -1;

        protected Texture2D     texture;
        protected Vector2       position            = Vector2.Zero;
        protected float         rotation            = 0f;
        protected float         scale               = 1f;
        protected float         opacity             = 1f;

        protected int           lastFrameTime       = 0;
        protected int           milliesPerFrame     = 30;

        protected Point         currentFrame        = Point.Zero;
        protected Point         sheetSize;

        protected bool          isAnimating         = true;
        protected bool          isVisible           = true;


        #region Initialization

        /// <summary>Default Constructor</summary>
        public Sprite(int id = -1) 
        {
            this.id = id;
        }

        /// <summary>Call this for init a sprite sheet</summary>
        /// <param name="filepath">filepath to the texture</param>
        /// <param name="sheetSize">the amount of columns and rows of the sheet</param>
        public void Initialize(string filepath, Point sheetSize)
        {
            this.sheetSize = sheetSize;
            this.texture = TextureManager.getInstance().GetTexture(filepath);
            if (texture == null)
                throw new ArgumentNullException("Sprite::Initialize - texture was unable to load");
        }

        #endregion Initialization

        #region API

        /// <summary>The Main 2d draw call </summary>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (isVisible == false)
                return;

            spriteBatch.Draw(texture, position,
                new Rectangle(
                    currentFrame.X * (texture.Width / sheetSize.X),
                    currentFrame.Y * (texture.Height / sheetSize.Y),
                    texture.Width / sheetSize.X,
                    texture.Height / sheetSize.Y),
                Color.FromNonPremultiplied(255, 255, 255, (int)(opacity * 255)),
                rotation,
                new Vector2(
                    (texture.Width / sheetSize.X / 2) * scale,
                    (texture.Height / sheetSize.Y / 2) * scale),
                scale, SpriteEffects.None, 0);
        }

        /// <summary>if this sprite is animating, this will increment the frame</summary>
        public virtual void Update(GameTime gameTime)
        {
            if (isAnimating == false)
                return;
            
            lastFrameTime += gameTime.ElapsedGameTime.Milliseconds;
            if (lastFrameTime > milliesPerFrame)
            {
                lastFrameTime -= milliesPerFrame;
                ++currentFrame.X;
                if (currentFrame.X >= sheetSize.X)
                {
                    currentFrame.X = 0;
                    ++currentFrame.Y;
                    if (currentFrame.Y >= sheetSize.Y)
                        currentFrame.Y = 0;
                }
            }
        }

        /// <summary>gets the bounding box of this sprite</summary>
        /// <returns> the new rectangle bounding box</returns>
        public Rectangle GetBoundingBox()
        {
            return new Rectangle((int)position.X, (int)position.Y, texture.Width / sheetSize.X, texture.Height / sheetSize.Y);
        }

        #endregion API

        #region Mutators
        /// <summary>texture pointer of this object</summary>
        public virtual Texture2D Texture
        {
            get { return texture; }
        }
        /// <summary>id of this sprite</summary>
        public virtual int ID
        {
            get { return this.id; }
        }
        /// <summary>size of an individual frame</summary>
        public virtual Point FrameSize
        {
            get
            {
                return new Point(texture.Width / sheetSize.X, 
                    texture.Height / sheetSize.Y);
            }
        }
        /// <summary>current frame index X</summary>
        public virtual int CurrentFrameX
        {
            get { return currentFrame.X; }
            set { currentFrame.X = value; }
        }
        /// <summary>current frame index Y</summary>
        public virtual int CurrentFrameY
        {
            get { return currentFrame.Y; }
            set { currentFrame.Y = value; }
        }
        /// <summary>toggle visibility</summary>
        public virtual bool Visible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }
        /// <summary>toggle animation</summary>
        public virtual bool Animating
        {
            get { return isAnimating; }
            set { isAnimating = value; }
        }
        /// <summary>x-y location</summary>
        public virtual Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        /// <summary>x location</summary>
        public virtual float PositionX
        {
            get { return position.X; }
            set { this.position.X = value; }
        }
        /// <summary>y location</summary>
        public virtual float PositionY
        {
            get { return position.Y; }
            set { this.position.Y = value; }
        }
        /// <summary>rotation component</summary>
        public virtual float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        /// <summary>uniform scale</summary>
        public virtual float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        /// <summary>This is a 0-1 value</summary>
        public virtual float Opacity
        {
            get { return this.opacity; }
            set { this.opacity = value; }
        }

        #endregion Mutators
    }
}
