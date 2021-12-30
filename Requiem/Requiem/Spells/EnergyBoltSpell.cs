using Engine.Drawing_Objects;
using Microsoft.Xna.Framework;
using Requiem.Entities;
using Requiem.Entities.EntitySupport;
using Requiem.Spells.Base;
using Requiem.Spells.RenderEffects;
using Requiem.Spells.SpellEffects;

namespace Requiem.Spells
{
    /// <summary>
    /// A base class for projectile spells
    /// </summary>
    /// <author> Gabrial Dubois </author>
    public sealed class EnergyBoltSpell : ProjectileSpell
    {
        /// <summary>
        /// Default constructor 
        /// </summary>
        /// <param name="owner">The entity owning the spell</param>
        public EnergyBoltSpell(Livable owner)
            : base(owner) { }

        /// <summary>
        /// Base initialize, must be called by the child class 
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            coolDown = new Timer(manager.SceneManager.Game, 500);
            coolDown.StartTimer();
            manager.SceneManager.Game.Components.Add(coolDown);

            icon = new Sprite();
            icon.Initialize("Sprites/EnergyBolt", new Point(1, 1));

            effects.Add(new DamageHealthEffect(this, 10));           
        }

        /// <summary>
        /// Cast the spell
        /// </summary>
        public override void Cast()
        {
            if (coolDown.TimesUp())
            {
                active = true;
                Projectile p = new Projectile(owner.WorldMatrix.Position + new Vector3(0f, Owner.OBB.Dimensions.Y * 0.5f, 0f), 
                    owner.LookAt, 10f, 1500f, SpellRendererType.PROJ_LIGHTNINGTRAIL);
                p.Initialize(manager.ContentManager);
                projectiles.Add(p);

                if (!manager.ContainsSpell(this))
                {
                    manager.AddSpell(this);
                }
            }
        }

        /// <summary>Base collision check, executes the spell's effects if a collision is detected</summary>
        /// <param name="target">The target to check</param>
        /// <returns>True if a collision is detected false if not</returns>
        public override bool CheckCollisions(Livable target)
        {
            if (base.CheckCollisions(target))
            {
                return true;
            }

            return false;
        }
    }
}


   
