using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Managers;

namespace Engine.Math_Physics
{
    /// <summary>Used to output the current frames per second to screen</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class FPSDisplay
    {
        #region Member Vars

        SpriteFont  font;
        Vector2     position;
        Vector2     position2;
        bool        isSlow = false;
        GameTime    time;

        #endregion Member Vars

        #region API

        /// <summary>Default CTOR</summary>
        public FPSDisplay()
        {
            font = Renderer.getInstance().GameFont;
            position = new Vector2(Renderer.getInstance().Device.Viewport.Width - font.MeasureString("FPS: 00").X - 35, 5);
            position2 = new Vector2(Renderer.getInstance().Device.Viewport.Width - font.MeasureString("Target FrameRate Missed").X, 20);
        }

        /// <summary>update values</summary>
        public void Update(GameTime time)
        {
            this.time = time;
            isSlow = time.IsRunningSlowly;
        }

        /// <summary>render to screen</summary>
        public void Render2D(SpriteBatch batch)
        {
            batch.DrawString(font, "FPS: " + (1 / (float)time.ElapsedGameTime.TotalSeconds),
                position, isSlow ? Color.Red : Color.Green);
            if (isSlow) batch.DrawString(font, "Target FrameRate Missed", 
                position2, isSlow ? Color.Red : Color.Green);
        }

        #endregion API
    }
}
