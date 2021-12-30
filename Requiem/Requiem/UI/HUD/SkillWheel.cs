using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Drawing_Objects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Requiem.Entities;

namespace Requiem.UI.HUD
{
    class SkillWheel
    {
        Sprite spellIcon1;
        Sprite spellIcon2;
        Player player;

        Vector2 iconPosition;
        Vector2 secondIconPosition;
        float iconScale = 0.5f;

        public SkillWheel(Player player)
        {
            this.player = player;
        }

        public void Initialize()
        {
            spellIcon1 = player.LeftSpellIcon;
            spellIcon2 = player.RightSpellIcon;

            iconPosition = new Vector2(20, 20);
            secondIconPosition = new Vector2(iconPosition.X + 70, iconPosition.Y + 50);

            spellIcon1.Position = iconPosition;
            spellIcon1.Scale = iconScale;
            spellIcon2.Position = secondIconPosition;
            spellIcon2.Scale = iconScale;        
        }

        public void Update()
        {
            spellIcon1 = player.LeftSpellIcon;
            spellIcon2 = player.RightSpellIcon;

            spellIcon1.Position = iconPosition;
            spellIcon1.Scale = iconScale;
            spellIcon2.Position = secondIconPosition;
            spellIcon2.Scale = iconScale;      
        }

        public void Draw(SpriteBatch batch)
        {
            spellIcon1.Draw(batch);
            spellIcon2.Draw(batch);
        }

        public Vector2 Position
        {
            get { return iconPosition; }
            set 
            {
                iconPosition = value;
                spellIcon1.Position = iconPosition;
                spellIcon1.Scale = iconScale;
                spellIcon2.Position = secondIconPosition;
                spellIcon2.Scale = iconScale;  
            }
        }
    }
}
