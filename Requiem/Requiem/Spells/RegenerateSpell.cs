using Requiem.Entities;
using Requiem.Entities.EntitySupport;
using Requiem.Spells.Base;
using Requiem.Spells.SpellEffects;
using Requiem.Spells.RenderEffects;
using Engine.Drawing_Objects;
using Microsoft.Xna.Framework;
using Engine.Drawing_Objects.ParticleSystems;
using Engine.Math_Physics;

namespace Requiem.Spells
{
    /// <summary> </summary>
    /// <author> Gabrial Dubois </author>
    public sealed class RegenerateSpell : SelfSpell
    {
        
        /// <summary>Default constructor</summary>
        /// <param name="owner">The entity owning the spell</param>
        public RegenerateSpell(Livable owner)
            : base(owner) { }

        /// <summary>Base initialize, must be called by the child class</summary>
        public override void Initialize()
        {
            base.Initialize();
            
            coolDown = new Timer(manager.SceneManager.Game, 500);
            coolDown.StartTimer();
            manager.SceneManager.Game.Components.Add(coolDown);

            effects.Add(new RestoreHealthEffect(this, 75));

            icon = new Sprite();
            icon.Initialize("Sprites/Regeneration", new Point(1, 1));

            WorldMatrix w = Owner.WorldMatrix;
            base.SpellRenderer = SpellRendererFactory.GetSelfRenderer(Owner, SpellRendererType.SELF_HEALING);
        }

        /// <summary>Cast the spell</summary>
        public override void Cast()
        {
            if (coolDown.TimesUp())
            {
                active = true;
                SpellRenderer.Initialize();
                numOfExpired = 0;

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