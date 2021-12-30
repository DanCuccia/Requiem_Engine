using Engine.Drawing_Objects;
using Engine.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Requiem.Entities.EntitySupport;

namespace Requiem.UI.HUD
{
    class HealthBar
    {
        Rectangle bar;
        Health health;
        Texture2D blank = TextureManager.getInstance().GetTexture("sprites//blank");
        int lastHealth;

        public HealthBar()
        { 
        }

        public void Initialize(Health health, int xPos, int yPos, int height)
        {
            bar = new Rectangle(xPos, yPos, health.CurrentHealth, height);

            this.health = health;
            lastHealth = health.CurrentHealth;
        }

        public void Update()
        {
            if (lastHealth != health.CurrentHealth)
            {
                bar.Width = health.CurrentHealth;
                lastHealth = health.CurrentHealth;
            }
        }

        public void Draw(SpriteBatch batch)
        {
            
            batch.Draw(blank, bar, Color.Green);
        }

        public Point Location
        {
            get { return bar.Location; }
            set { bar.Location = value; }
        }
    }
}
