using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Game_Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine.Drawing_Objects;
using Engine.Managers;
#pragma warning disable 1591

namespace Engine.Scenes.HUD
{
    /// <summary>allows the user to edit triggers</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class TriggerHud : HudBase
    {
        Trigger trigger;
        Vector2 idPosition;

        public TriggerHud(EditorScene editor, Trigger t)
            : base(editor)
        {
            trigger = t;
        }

        protected override void LoadButtons(ContentManager content)
        {
            idPosition = new Vector2(rect.Left + (rect.Width * 0.5f) - 
                (font.MeasureString("ID = " + trigger.ID).X * 0.5f), rect.Top + 16f);

            SpriteButton btn = new SpriteButton(GameIDList.Button_TriggerHud_Repeatable);
            btn.Initialize("sprites//editor//LE_levelNameBlank", "REPEATABLE = " + trigger.Repeatable);
            btn.setExecution(null, toggleRepeat);
            btn.Position = new Vector2(rect.Left + 4f, rect.Top + 30f);
            AddButton(btn);
        }

        private void toggleRepeat()
        {
            trigger.Repeatable = !trigger.Repeatable;
            buttonList[0].Label = "REPEATABLE = " + trigger.Repeatable;
        }

        protected override void Render2DExtra(SpriteBatch batch)
        {
            batch.DrawString(font, "ID = " + trigger.ID, idPosition, Color.White);
        }

        protected override void LoadSliders(ContentManager content) { }
        protected override void LoadSprites(ContentManager content) { }
    }
}
