#region using

using System.Collections.Generic;
using Engine.Managers.Camera;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Requiem.Entities;
using Requiem.Entities.EntitySupport;
using Requiem.Spells.SpellEffects;

#endregion using

namespace Requiem.Spells.Base
{
    /// <summary></summary>
    /// <author> Gabrial Dubois </author>
    public abstract class BurstSpell : Spell
    {
        #region variables

        protected float xRange = 0;
        protected float yRange = 0;
        protected float zRange = 0;
        protected int numberOfTargets = 0;
        protected Timer castingTimer = null;
        protected OrientedBoundingBox spellField = null;

        #endregion variables

        #region initialize

        /// <summary>
        /// Default constructor 
        /// </summary>
        /// <param name="owner">The entity owning the spell</param>
        public BurstSpell(Livable owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Base initialize, must be called by the child class 
        /// </summary>
        public override void Initialize()
        {
            manager = SpellManager.GetInstance();
            effects = new List<SpellEffect>();
            targetingType = TargetingTypes.TOUCH;
            active = false;
            numOfExpired = 0;
        }

        #endregion initialize

        #region interface

        /// <summary>
        /// Base update, updates spell effects and determines if the spell is expired
        /// </summary>
        public override void Update(Camera camera, GameTime time)
        {
            foreach (SpellEffect e in effects)
            {
                if (e.Active)
                {
                    e.Update();

                    if (e.Expired)
                    {
                        numOfExpired++;
                    }
                }
            }


            if (castingTimer.TimesUpAlt())
            {
                if (numberOfTargets == 0)
                {
                    active = false;
                }

                foreach (SpellEffect s in effects)
                {
                    if (s.Targets.Count == 0)
                    {
                        active = false;
                    }
                }
            }

            if (base.SpellRenderer != null)
                SpellRenderer.Update(ref camera, time);

            if (spellField != null)
                spellField.Update(Owner.WorldMatrix.GetWorldMatrix());
            
        }

        /// <summary>Main draw call</summary>
        public override void Draw() 
        {
            if (SpellRenderer != null)
                SpellRenderer.Render();
        }

        /// <summary>Draw debug call</summary>
        public override void DrawDebug()
        {
            if (SpellRenderer != null)
                SpellRenderer.RenderDebug();
            if (spellField != null)
                spellField.Render();
        }

        /// <summary>
        /// Base collision check, executes the spell's effects if a collision is detected
        /// </summary>
        /// <param name="target">The target to check</param>
        /// <returns>True if a collision is detected false if not</returns>
        public override bool CheckCollisions(Livable target)
        {
            if (spellField.Intersects(target.OBB))
            {
                foreach (SpellEffect s in effects)
                {
                    s.Execute(target);
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Alternative collision check, does not execute the spell's effects
        /// </summary>
        /// <param name="obb">The bounding box to check</param>
        /// <returns>True if a collision is detected false if not</returns>
        public override bool CheckCollisionsNoEffect(OrientedBoundingBox obb)
        {
            if (spellField.Intersects(obb))
            {
                return true;
            }

            return false;
        }

        #endregion interface
    }
}
