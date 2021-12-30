using System.Collections.Generic;
using Requiem.Entities;
using Requiem.Spells.Base;
using Microsoft.Xna.Framework;

namespace Requiem.Spells.SpellEffects
{
    /// <summary>
    /// An effect that pushes the target away from the caster
    /// </summary>
    /// <author> Gabrial Dubois </author>
    class KnockBackEffect : SpellEffect
    {
        Vector3 direction = Vector3.Zero;
        float amount = 0;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="owner">The spell that owns the effect</param>
        public KnockBackEffect(Spell owner, Vector3 direction, float amount)
        {
            this.owner = owner;
            this.targets = new List<Livable>();
            this.expired = false;
            this.active = false;

            this.direction = direction;
            this.amount = amount;
        }

        /// <summary>
        /// Execute the effect
        /// </summary>
        /// <param name="target">The target for the spell</param>
        public override void Execute(Entities.Livable target)
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
                    e.Movement.AddImpulse(direction, amount);
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
        public override void OnCollision(Entities.Livable target)
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
