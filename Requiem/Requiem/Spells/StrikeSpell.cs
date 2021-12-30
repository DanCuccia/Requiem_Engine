using Engine.Drawing_Objects;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Requiem.Entities;
using Requiem.Entities.EntitySupport;
using Requiem.Spells.Base;
using Requiem.Spells.RenderEffects;
using Requiem.Spells.SpellEffects;

namespace Requiem.Spells
{
    /// <summary>close range 'melee' type spell</summary>
    /// <author> Gabrial Dubois</author>
    public sealed class StrikeSpell : ProjectileSpell
    {
       /// <summary> Default constructor</summary>
       /// <param name="owner">The entity owning the spell</param>
        public StrikeSpell(Livable owner)
            : base(owner) { }

        /// <summary>Base initialize, must be called by the child class </summary>
        public override void Initialize()
        {
            base.Initialize();

            coolDown = new Timer(manager.SceneManager.Game, 500);
            coolDown.StartTimer();
            manager.SceneManager.Game.Components.Add(coolDown);

            icon = new Sprite();
            icon.Initialize("Sprites/Strike", new Point(1, 1));

            effects.Add(new DamageHealthEffect(this, 30));
        }

        /// <summary>Cast the spell</summary>
        public override void Cast()
        {
            if (coolDown.TimesUp())
            {
                active = true;

                Timer t = new Timer(manager.SceneManager.Game, 2000);
                t.StartTimer();
                manager.SceneManager.Game.Components.Add(t);

                Projectile p = new Projectile(owner.WorldMatrix.Position, Owner.LookAt, 7f, 200f, SpellRendererType.PROJ_STRIKE, null, t);

                WorldMatrix w = p.WorldMatrix;
                Vector3 position = Owner.WorldMatrix.Position + new Vector3(0f, Owner.OBB.Dimensions.Y * 0.5f, 0f) + (Owner.LookAt * 500f);
                p.movement = new Requiem.Movement.StrikeProjectileMovement(ref w, Owner);

                p.Initialize(manager.ContentManager);
                projectiles.Add(p);

                if (!manager.ContainsSpell(this))
                {
                    manager.AddSpell(this);
                }
            }
        }

        /// <summary>Executes the spell's effects if a collision is detected</summary>
        /// <param name="target">The target to check</param>
        /// <returns>True if a collision is detected false if not</returns>
        public override bool CheckCollisions(Livable target)
        {
            if (base.CheckCollisions(target))
            {
                numOfExpired = 0;
                return true;
            }

            return false;
        }
    }
}
