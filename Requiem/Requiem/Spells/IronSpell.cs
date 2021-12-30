using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Requiem.Spells.Base;
using Requiem.Entities;
using Requiem.Entities.EntitySupport;
using Requiem.Spells.SpellEffects;
using Engine.Drawing_Objects;
using Microsoft.Xna.Framework;

namespace Requiem.Spells
{
    /// <summary>
    /// 
    /// </summary>
    /// <author> Gabrial Dubois </author>
    class IronSpell : SelfSpell
    {
        /// <summary>
        /// Default constructor 
        /// </summary>
        /// <param name="owner">The entity owning the spell</param>
        public IronSpell(Livable owner)
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
            icon.Initialize("Sprites/IronWill", new Point(1, 1));

            effects.Add(new DamageReductionEffect(this, 0.5f));
        }

        /// <summary>
        /// Cast the spell
        /// </summary>
        public override void Cast()
        {
            if (coolDown.TimesUp())
            {
                active = true;

                foreach (SpellEffect s in effects)
                {
                    s.Execute(owner);
                }

                if (!manager.ContainsSpell(this))
                {
                    manager.AddSpell(this);
                }
            }
        }
    }
}
