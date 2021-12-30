using System.Collections.Generic;
using Requiem.Entities;
using Requiem.Spells.Base;

namespace Requiem.Spells.SpellEffects
{
    /// <summary>
    /// An effect that reduces the target's health
    /// </summary>
    /// <author> Gabrial Dubois </author>
    class DamageHealthEffect : SpellEffect
    {
        private int damage;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="owner">The spell that owns the effect</param>
        /// <param name="amount">The amount of health to restore</param>
        public DamageHealthEffect(Spell owner, int damage)
        {
            this.owner = owner;
            this.damage = damage;
            this.targets = new List<Livable>();
            this.expired = false;
            this.active = false;
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
            active = true;
        }

        /// <summary>
        /// Update the effect 
        /// </summary>
        public override void Update()
        {
            if (active)
            {
                foreach (Livable e in targets)
                {
                    e.Health.Hurt(damage);
                }

                OnExpired();
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

