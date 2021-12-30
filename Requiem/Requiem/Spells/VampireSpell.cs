using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Requiem.Entities;
using Requiem.Entities.EntitySupport;
using Requiem.Spells.Base;
using Requiem.Spells.SpellEffects;
using Engine.Managers.Camera;
using Engine.Drawing_Objects;
using Requiem.Spells.RenderEffects;

namespace Requiem.Spells
{
    /// <summary></summary>
    /// <author> Gabrial Dubois </author>
    public sealed class VampireSpell : BurstSpell
    {
        /// <summary>temprary hack fix</summary>
        Timer test;

        int damage = 5;
        SpellEffect damageEffect;

        /// <summary>Default constructor</summary>
        /// <param name="owner">The entity owning the spell</param>
        public VampireSpell(Livable owner)
            : base(owner)
        {
            SpellRenderer = SpellRendererFactory.GetBurstRenderer(Owner.WorldMatrix, SpellRendererType.PROJ_VAMPIRE);
        }

        /// <summary>Base initialize, must be called by the child class</summary>
        public override void Initialize()
        {
            base.Initialize();

            xRange = .8f;
            yRange = .5f;
            zRange = .5f;

            numberOfTargets = -1;

            spellField = new OrientedBoundingBox(
                new Vector3(-xRange, 0f, -zRange),
                new Vector3(xRange, yRange * 2f, zRange));

            coolDown = new Timer(manager.SceneManager.Game, 500);
            coolDown.StartTimer();
            manager.SceneManager.Game.Components.Add(coolDown);

            castingTimer = new Timer(manager.SceneManager.Game, 10000);
            manager.SceneManager.Game.Components.Add(castingTimer);

            test = new Timer(manager.SceneManager.Game, 50);
            manager.SceneManager.Game.Components.Add(test);

            icon = new Sprite();
            icon.Initialize("Sprites/Vampiric Embrace", new Point(1, 1));

            damageEffect = new AbsorbHealth(this, damage);
            effects.Add(damageEffect);
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

                test.ResetTimer();
                test.StartTimer();

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

        /// <summary>Updates spell effects and determines if the spell is expired</summary>
        public override void Update(Camera camera, GameTime time)
        {
           // if (test.TimesUp() && damageEffect.Targets.Count == 0)
           // {
           //     active = false;
           // }

            base.Update(camera, time);
        }

        public override void Draw()
        {
            spellField.Render();
            base.Draw();
        }

        /// <summary>Executes the spell's effects if a collision is detected</summary>
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

