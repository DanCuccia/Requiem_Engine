using Requiem.Spells.Base;
using Requiem.Entities;
using System.Collections.Generic;
using Requiem.Entities.EntitySupport;

namespace Requiem.Spells.SpellEffects
{
    /// <summary>
    /// An effect that reduces the target's movement speed
    /// </summary>
    /// <author> Gabrial Dubois </author>
    class SlowEffect : SpellEffect
    {
        Timer duration;
        float reduction;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="owner">The spell that owns the effect</param>
        /// <param name="amount">The amount of health to restore</param>
        public SlowEffect(Spell owner, float reduction)
        {
            this.owner = owner;
            this.targets = new List<Livable>();
            this.expired = false;
            this.active = false;
            this.reduction = reduction;

            this.duration = new Timer(owner.Manager.SceneManager.Game, 5000);
            this.owner.Manager.SceneManager.Game.Components.Add(duration);
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
            target.Movement.ModifySpeed(reduction);
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
                        e.Movement.ModifySpeed(1);
                        
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
