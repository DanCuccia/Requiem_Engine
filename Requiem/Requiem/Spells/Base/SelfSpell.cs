//*****************************************
//Written by Gabrial Dubois
//*****************************************

#region using

using System.Collections.Generic;
using Engine.Managers.Camera;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Requiem.Entities;
using Requiem.Spells.SpellEffects;
using Requiem.Spells.RenderEffects;

#endregion using

namespace Requiem.Spells.Base
{
    /// <summary>
    /// 
    /// </summary>
    /// <author> Gabrial Dubois </author>
    public abstract class SelfSpell : Spell
    {
        #region intitialze
        
        /// <summary>
        /// Default constructor 
        /// </summary>
        /// <param name="owner">The entity owning the spell</param>
        public SelfSpell(Livable owner)
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
            targetingType = TargetingTypes.SELF;
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

            if (numOfExpired == effects.Count)
            {
                active = false;
            }

            if (SpellRenderer != null)
                SpellRenderer.Update(ref camera, time);
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
        }

        /// <summary>
        /// Base collision check 
        /// Always returns false, provided for compatibility  
        /// </summary>
        /// <param name="target">The target to check</param>
        /// <returns>False</returns>
        public override bool CheckCollisions(Livable target = null)
        {
            return false;
        }

        /// <summary>
        /// Alternative collision check 
        /// Always returns false, provided for compatibility 
        /// </summary>
        /// <param name="target">The target to check</param>
        /// <returns>False</returns>
        public override bool CheckCollisionsNoEffect(OrientedBoundingBox obb = null)
        {
            return false;
        }

        #endregion interface
    }
}
