using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Requiem.Entities;
using Requiem.Entities.EntitySupport;
using Requiem.Spells.Base;
using Requiem.Spells.SpellEffects;
using Engine;
using Engine.Managers.Camera;
using Engine.Drawing_Objects;
using Requiem.Spells.RenderEffects;

namespace Requiem.Spells
{
    /// <summary>owner local aoe spell</summary>
    /// <author>Gabrial Dubois</author>
    public sealed class WebTrapSpell : BurstSpell
    {
        /// <summary>Default constructor</summary>
        /// <param name="owner">The entity owning the spell</param>
        public WebTrapSpell(Livable owner)
        : base(owner) 
        {
            base.SpellRenderer = SpellRendererFactory.GetBurstRenderer(owner.WorldMatrix, SpellRendererType.PROJ_WEB);
        }

        /// <summary>Base initialize, must be called by the child class</summary>
        public override void Initialize()
        {
            base.Initialize();

            xRange = 50;
            yRange = 50;
            zRange = 50;

            //spellField = new OrientedBoundingBox(
            //    new Vector3(-xRange, 0f, -zRange),
            //    new Vector3(xRange, yRange * 2f, zRange));
            
            coolDown = new Timer(manager.SceneManager.Game, 10000);
            coolDown.StartTimer();
            manager.SceneManager.Game.Components.Add(coolDown);

            castingTimer = new Timer(manager.SceneManager.Game, 5000);
            manager.SceneManager.Game.Components.Add(castingTimer);

            icon = new Sprite();
            icon.Initialize("Sprites/WebBurst", new Point(1, 1));

            effects.Add(new SlowEffect(this, 0.5f));
        }

        /// <summary>Cast the spell</summary>
        public override void Cast()
        {
            if (coolDown.TimesUp())
            {
                active = true;
                numOfExpired = -1;

                castingTimer.ResetTimer();
                castingTimer.StartTimer();

                if (SpellRenderer != null)
                {
                    SpellRenderer.Initialize();
                }

                if (!manager.ContainsSpell(this))
                {
                    manager.AddSpell(this);
                }
            }
        }

        public override void Update(Camera camera, GameTime time)
        {
            base.Update(camera, time);

            if (castingTimer.TimesUpAlt() && numberOfTargets == 0)
            {
                active = false;
            }
        }

        /// <summary>Executes the spell's effects if a collision is detected</summary>
        /// <param name="target">The target to check</param>
        /// <returns>True if a collision is detected false if not</returns>
        public override bool CheckCollisions(Livable target)
        {
            if (SpellRenderer.GetDrawable().OBB.Intersects(target.OBB))
            {
                numberOfTargets++;
                foreach (SpellEffect s in effects)
                {
                    s.Execute(target);
                }
                return true;
            }

            return false;
        }
    }
}

