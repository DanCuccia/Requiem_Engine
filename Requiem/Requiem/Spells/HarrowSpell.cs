using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Requiem.Entities;
using Requiem.Entities.EntitySupport;
using Requiem.Spells.Base;
using Requiem.Spells.SpellEffects;
using Engine.Drawing_Objects;

namespace Requiem.Spells
{
    /// <summary>
    /// 
    /// </summary>
    /// <author> Gabrial Dubois </author>
    class HarrowSpell : TouchSpell
   {
        
       /// <summary>
       /// Default constructor 
       /// </summary>
       /// <param name="owner">The entity owning the spell</param>
        public HarrowSpell(Livable owner)
            : base(owner) { }

        /// <summary>
        /// Base initialize, must be called by the child class 
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            xRange = 30;
            yRange = 50;
            zRange = 50;

            coolDown = new Timer(manager.SceneManager.Game, 500);
            coolDown.StartTimer();
            manager.SceneManager.Game.Components.Add(coolDown);

            castingTimer = new Timer(manager.SceneManager.Game, 50);
            manager.SceneManager.Game.Components.Add(castingTimer);

            icon = new Sprite();
            icon.Initialize("Sprites/Harrow", new Point(1, 1));

            effects.Add(new DamageHealthEffect(this, 5));
        }

        /// <summary>
        /// Cast the spell
        /// </summary>
        public override void Cast()
        {
            if (coolDown.TimesUp())
            {
                active = true;
                numOfExpired = -1;

                castingTimer.ResetTimer();
                castingTimer.StartTimer();

                if (owner.LookAt.X < 0)
                {
                    spellField = new OrientedBoundingBox(owner.WorldMatrix.Position,
                                        new Vector3(owner.WorldMatrix.Position.X - xRange, owner.WorldMatrix.Position.Y + yRange, owner.WorldMatrix.Position.Z + zRange));
                    effects.Add(new KnockBackEffect(this, new Vector3(-1, 0, 0), 100));
                }
                else
                {
                    spellField = new OrientedBoundingBox(owner.WorldMatrix.Position,
                                        new Vector3(owner.WorldMatrix.Position.X + xRange, owner.WorldMatrix.Position.Y + yRange, owner.WorldMatrix.Position.Z + zRange));
                    effects.Add(new KnockBackEffect(this, new Vector3(-1, 0, 0), 100));
                }

                if (!manager.ContainsSpell(this))
                {
                    manager.AddSpell(this);
                }
            }
        }

        /// <summary>
        /// Draw the visual component of the spell
        /// </summary>
        public override void Draw()
        {
            spellField.Render();
        }

        /// <summary>
        /// Executes the spell's effects if a collision is detected
        /// </summary>
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

