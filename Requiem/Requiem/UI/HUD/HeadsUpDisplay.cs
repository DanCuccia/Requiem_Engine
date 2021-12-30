using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Requiem.Entities;
using Engine.Drawing_Objects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Managers;

namespace Requiem.UI.HUD
{
    public class HeadsUpDisplay
    {
        HealthBar healthBar;
        SkillWheel skillWheel;

        Player player;
 
        public HeadsUpDisplay(Player player)
        {
            this.player = player;
        }

        public void Initialize(Rectangle window)
        {
            healthBar = new HealthBar();
            healthBar.Initialize(player.Health, 100, 20, 30);

            skillWheel = new SkillWheel(player);
            skillWheel.Initialize();
        }

        public void Update()
        {
            skillWheel.Update();
            healthBar.Update();
        }

        public void Draw(SpriteBatch batch)
        {
            healthBar.Draw(batch);
            skillWheel.Draw(batch);
        }

        public Player Player
        {
            get { return player; }
        }
    }
}
