using System.Collections.Generic;
using Requiem.Entities;
using Requiem.Spells.Base;
using Requiem.Entities.EntitySupport;

namespace Requiem.Spells.SpellEffects
{
    /// <summary>
    /// An effect that increases the  target's health
    /// </summary>
    /// <author> Gabrial Dubois </author>
    class RestoreHealthEffect : SpellEffect
    {
        private int amount;
        Timer duration;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="owner">The spell that owns the effect</param>
        /// <param name="amount">The amount of health to restore</param>
        public RestoreHealthEffect(Spell owner, int amount)
        {
            this.owner = owner;
            this.amount = amount;
            this.targets = new List<Livable>();
            this.expired = false;
            this.active = false;

            this.duration = new Timer(owner.Manager.SceneManager.Game, 2000);
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

            target.Health.Heal(amount);
            duration.ResetTimer();
            duration.StartTimer();
            targets.Add(target);
            active = true;
            expired = false;
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
