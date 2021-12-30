using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Requiem.Spells.Base;
using Requiem.Entities;
using Requiem.Entities.EntitySupport;

namespace Requiem.Spells.SpellEffects
{
    class AbsorbHealth : SpellEffect
    {
        Timer duration;
        private int damage;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="owner">The spell that owns the effect</param>
        /// <param name="amount">The amount of health to restore</param>
        public AbsorbHealth(Spell owner, int damage)
        {
            this.owner = owner;
            this.damage = damage;
            this.targets = new List<Livable>();
            this.expired = false;
            this.active = false;

            this.duration = new Timer(owner.Manager.SceneManager.Game, 500);
            this.owner.Manager.SceneManager.Game.Components.Add(duration);
        }

        /// <summary>
        /// Execute the effect
        /// </summary>
        /// <param name="target">The target for the spell</param>
        public override void Execute(Livable target)
        {
            foreach (Livable l in targets)
            {
                if (l == target) return; 
            }

            targets.Add(target);
            duration.ResetTimer();
            duration.StartTimer();

            active = true;
        }

        /// <summary>
        /// Update the effect 
        /// </summary>
        public override void Update()
        {
            if (active)
            {
                if (duration.TimesUpAlt())
                {
                    foreach (Livable e in targets)
                    {
                        e.Health.Hurt(damage);
                        owner.Owner.Health.Heal(damage);
                    }

                    OnExpired();
                }

            }
        }

        /// <summary>
        /// Draw any visual component of the effect
        /// </summary>
        public override void Draw()
        {
        }

        /// <summary>
        /// Run any collision logic for the effect
        /// </summary>
        /// <param name="target">Target of the effect</param>
        public override void OnCollision(Livable target)
        {
            Execute(target);
        }

        public override void OnExpired()
        {
            expired = true;
            active = false;

            targets.Clear();
        }
    }
}

