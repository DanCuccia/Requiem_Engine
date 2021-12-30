using Engine.Drawing_Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Scenes.HUD
{
    /// <summary>Verification Hud</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class VerifyHud : HudBase
    {
        /// <summary>callback the yes and no buttons execute</summary>
        public delegate void clickCallback();
        clickCallback onYes = null;
        clickCallback onNo = null;
        string notification = "";

        /// <summary>Default CTOR</summary>
        public VerifyHud(EditorScene editor, string notification, clickCallback yes, clickCallback no)
            :base(editor) 
        {
            this.onYes = yes;
            this.onNo = no;
            this.notification = notification;
        }

        /// <summary>hud defaults to the center of the screen</summary>
        /// <returns>256x128 center in screen</returns>
        protected override Rectangle LoadRectangle()
        {
            return new Rectangle((int)((editor.device.Viewport.Width * .5f) - 128), 
                (int)((editor.device.Viewport.Height * .5f) - 64), 256, 128);
        }

        /// <summary>load buttons</summary>
        /// <param name="content">content manager</param>
        protected override void LoadButtons(ContentManager content)
        {
            SpriteButton btn = new SpriteButton();
            btn.Initialize("sprites//editor//LE_checkBox");
            btn.Position = new Vector2(Rect.X + 64f, Rect.Y + ((Rect.Height * 0.333f) * 2f));
            btn.ToggleFrames = false;
            btn.CurrentFrameX = 1;
            btn.setExecution(null, clickYes);
            AddButton(btn);

            btn = new SpriteButton();
            btn.Initialize("sprites//editor//LE_checkBox");
            btn.Position = new Vector2(Rect.X + Rect.Width - 96f, Rect.Y + ((Rect.Height * 0.333f) * 2f));
            btn.ToggleFrames = false;
            btn.CurrentFrameX = 0;
            btn.setExecution(null, clickNo);
            AddButton(btn);
        }

        private void clickYes()
        {
            if (onYes != null)
                onYes();
            editor.verifyHud = null;
        }

        private void clickNo()
        {
            if (onNo != null)
                onNo();
            editor.verifyHud = null;
        }

        /// <summary>overriden to draw the extra string</summary>
        /// <param name="batch"></param>
        protected override void Render2DExtra(SpriteBatch batch)
        {
            Vector2 measure = font.MeasureString(notification);
            batch.DrawString(font, notification, new Vector2(Rect.Center.X - (measure.X * 0.5f), Rect.Y + (measure.Y + 12)), Color.Red);
        }

        /// <summary>load sliders</summary>
        /// <param name="content">content manager</param>
        protected override void LoadSliders(ContentManager content) { }
        /// <summary>load sprites</summary>
        /// <param name="content">content manager</param>
        protected override void LoadSprites(ContentManager content) { }
    }
}
