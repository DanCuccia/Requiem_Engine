using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Engine.Managers;
using Engine.Math_Physics;

namespace Engine.Drawing_Objects
{
    /// <summary>Functionality for UI Sliders</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class Slider
    {
        #region Member Vars

        SpriteFont font;
        string name;

        Vector2 position = Vector2.Zero;
        float width = 72f;
        float displayMultiplier = 1f;

        SpriteButton grabber = new SpriteButton();
        Texture2D blank = null;
        /// <summary>if 1 is grabbed, will not test others</summary>
        public bool isGrabbed = false;

        Rectangle bar;
        Rectangle barStart;
        Rectangle barEnd;
        float edgeOffsets = 16f;

        float actualValue = 0;

        List<SpriteButton> buttonList = new List<SpriteButton>();

        /// <summary>delegate onchange func call</summary>
        public delegate void OnChange();
        private OnChange onChange = null;

        #endregion Member Vars

        #region Initialization

        /// <summary>default CTOR - you do not have control over it's height(</summary>
        /// <param name="position">where this slider starts</param>
        /// <param name="width">how wide you want this slider on the screen</param>
        /// <param name="name">Optional - draws a name in the top center of the slider</param>
        /// <param name="onChange">optional delegate function to automatically call when there is a change</param>
        /// <param name="multiplier">optional multiplier to the output display value</param>
        /// <param name="content">xna content manager</param>
        public Slider(ContentManager content, Vector2 position, float width = 72f, string name = "", OnChange onChange = null, float multiplier = 1f)
        {
            try
            {
                this.position = position;
                this.width = width;
                this.name = name;
                this.onChange = onChange;
                this.displayMultiplier = multiplier;

                grabber.Initialize("sprites//editor//LE_sliderGrabber");
                grabber.Position = new Vector2(position.X + (width / 2), position.Y + 8);

                blank = content.Load<Texture2D>("sprites//blank");
                if (blank == null)
                    throw new ArgumentNullException("Slider::Slider - blank texture loaded null");

                font = Renderer.getInstance().GameFont;
                
                this.initDrawingRects();

                SpriteButton btn = new SpriteButton();
                btn.Initialize("sprites//Slider_setZero");
                btn.setExecution(null, SetToZero);
                btn.Position = new Vector2(position.X, position.Y + btn.Texture.Height / 2);
                buttonList.Add(btn);

                btn = new SpriteButton();
                btn.Initialize("sprites//Slider_setOne");
                btn.setExecution(null, SetToOne);
                btn.Position = new Vector2(position.X + width - btn.Texture.Width/2, position.Y + btn.Texture.Height / 2);
                buttonList.Add(btn);
            }
            catch (Exception e)
            {
                if (EngineFlags.drawDebug)
                    Math_Physics.MyUtility.DumpException(e);
            }
        }

        /// <summary>Initializes the Rectangles used to draw the slider</summary>
        private void initDrawingRects()
        {
            bar = new Rectangle((int)position.X + (int)edgeOffsets, (int)position.Y + 24, (int)width - 32, 1);
            barStart = new Rectangle((int)position.X + 14, (int)position.Y + 4, 2, 40);
            barEnd = new Rectangle((int)position.X + (int)width - (int)edgeOffsets, (int)position.Y + 4, 2, 40);
        }

        #endregion Initialization

        #region API

        /// <summary>Main input call, process the grabber movement</summary>
        public void Input(MouseState ms)
        {
            MyUtility.ProcessButtonList(ms, buttonList);

            switch (ms.LeftButton)
            {
                case ButtonState.Pressed:
                    if (isGrabbed == false)
                    {
                        if (MyMath.IsWithin(ms.X, ms.Y, grabber.GetBoundingBox()))
                            isGrabbed = true;
                    }
                    if (isGrabbed == true)
                    {
                        actualValue = (ms.X - position.X - edgeOffsets) / (width - (edgeOffsets*2));
                        if (actualValue < 0f)
                            actualValue = 0f;
                        if (actualValue > 1f)
                            actualValue = 1f;

                        updateGrabberPos();

                        if (onChange != null)
                            this.onChange();
                    }
                    break;

                case ButtonState.Released:
                    isGrabbed = false;
                    break;
            }
        }

        /// <summary>Change the location of the grabber</summary>
        private void updateGrabberPos()
        {
            grabber.PositionX = position.X + edgeOffsets - ((float)grabber.Texture.Width / 4f) + (bar.Width * actualValue);
        }

        /// <summary>Main Draw Call</summary>
        public void Render2D(SpriteBatch batch)
        {
            batch.Draw(blank, bar, MyColors.AlphaWhite);
            batch.Draw(blank, barEnd, MyColors.AlphaWhite);
            batch.Draw(blank, barStart, MyColors.AlphaWhite);

            foreach (SpriteButton btn in buttonList)
                btn.Draw(batch);

            grabber.Draw(batch);

            string output = (this.GetValue() * this.displayMultiplier).ToString();
            string final = "";
            for (int i = 0; i < output.Length; i++)
            {
                final += output[i].ToString();
                if (i > 3)
                    break;
            }
            batch.DrawString(font, final, new Vector2(position.X + (width / 2) - (font.MeasureString(output).X / 2), position.Y + 32), Color.White);

            batch.DrawString(font, this.name, new Vector2(position.X + (width / 2) - (font.MeasureString(name).X / 2), position.Y), Color.White);
        }

        /// <summary>Get the 0 to 1 value of where this slider is along the bar</summary>
        /// <returns>0 to 1 slider value</returns>
        public float GetValue()
        {
            return actualValue;
        }

        /// <summary>Manually set the value of the slider</summary>
        /// <param name="value">0 to 1 value</param>
        public void SetValue(float value)
        {
            if (value < 0f || value > 1f)
            {
                Console.WriteLine("Slider::SetValue - cannot be below 0f or greater than 1f");
                return;
            }

            this.actualValue = value;

            grabber.PositionX = position.X + 16 + ((width - 32) * value) - ((float)grabber.Texture.Width / 4);
        }

        /// <summary>hardzet the slider to 0</summary>
        public void SetToZero()
        {
            grabber.PositionX = position.X + 16 + ((width - 32) * 0f) - ((float)grabber.Texture.Width / 4);
            this.actualValue = 0f;
            this.onChange();
        }
        /// <summary>hardzet the slider to 1</summary>
        public void SetToOne()
        {
            grabber.PositionX = position.X + 16 + ((width - 32) * 1f) - (grabber.Texture.Width / 4);
            this.actualValue = 1f;
            this.onChange();
        }

        #endregion API
    }
}
