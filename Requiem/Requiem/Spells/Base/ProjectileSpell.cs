#region using

using System.Collections.Generic;
using Engine.Managers.Camera;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Requiem.Entities;
using Requiem.Spells.SpellEffects;

#endregion using

namespace Requiem.Spells.Base
{
    /// <summary>
    /// A base class for projectile spells
    /// </summary>
    /// <author> Gabrial Dubois </author>
    public abstract class ProjectileSpell : Spell
    {
        protected List<Projectile> projectiles = null;

        #region initialize

        /// <summary>
        /// Default constructor 
        /// </summary>
        /// <param name="owner">The entity owning the spell</param>
        public ProjectileSpell(Livable owner)
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
            targetingType = TargetingTypes.PROJECTILE;
            projectiles = new List<Projectile>();
            active = false;
            numOfExpired = -1;
        }

        #endregion initialize

        #region interface

        /// <summary>Base update, updates projectiles and spell effects and determines if the spell is expired</summary>
        public override void Update(Camera camera, GameTime time)
        {
            for (int ctr = 0; ctr < projectiles.Count; ctr++)
            {
                projectiles[ctr].Update(ref camera, time);
                
                if (!projectiles[ctr].Alive)
                {
                    projectiles[ctr].drawing.OnDestroy();
                    projectiles.RemoveAt(ctr);
                    ctr--;
                }
            }

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

            if (numOfExpired == effects.Count && projectiles.Count == 0)
            {
                active = false;
            }
            else if (numOfExpired == -1 && projectiles.Count == 0)
            {
                active = false;
            }
        }

        /// <summary>Base draw, draws the projectiles</summary>
        public override void Draw()
        {
            foreach (Projectile p in projectiles)
            {
                p.Render();
            }
        }

        /// <summary>Render debugging information</summary>
        public override void DrawDebug()
        {
            foreach (Projectile p in projectiles)
            {
                p.RenderDebug();
            }
        }

        /// <summary>Base collision check, executes the spell's effects if a collision is detected</summary>
        /// <param name="target">The target to check</param>
        /// <returns>True if a collision is detected false if not</returns>
        public override bool CheckCollisions(Livable target)
        {
            for (int ctr = 0; ctr < projectiles.Count; ctr++)
            {
                if (projectiles[ctr].Model.OBB.Intersects(target.OBB))
                {
                    foreach (SpellEffect s in effects)
                    {
                        s.Execute(target);
                        numOfExpired = 0;
                    }

                    projectiles[ctr].drawing.OnDestroy();
                    projectiles.RemoveAt(ctr);
                    ctr--;

                    return true;
                }
            }

            return false;
        }

        /// <summary>Alternative collision check, does not execute the spell's effects</summary>
        /// <param name="obb">The bounding box to check</param>
        /// <returns>True if a collision is detected false if not</returns>
        public override bool CheckCollisionsNoEffect(OrientedBoundingBox obb)
        {
            for (int ctr = 0; ctr < projectiles.Count; ctr++)
            {
                if (projectiles[ctr].Model.OBB.Intersects(obb))
                {
                    projectiles[ctr].drawing.OnDestroy();
                    projectiles.RemoveAt(ctr);
                    ctr--;

                    return true;
                }
            }

            return false;
        }
    
        #endregion interface
    }
}