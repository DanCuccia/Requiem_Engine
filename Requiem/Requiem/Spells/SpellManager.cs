#region using

using System.Collections.Generic;
using Engine.Managers;
using Engine.Managers.Camera;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Requiem.Entities;
using Requiem.Spells.Base;

#endregion

namespace Requiem.Spells
{
    /// <summary>
    /// 
    /// </summary>
    /// <author> Gabrial Dubois </author>
    public class SpellManager
    {
        #region variables

        public List<Spell> activeSpells = new List<Spell>();
        public List<Projectile> spellProjectiles = new List<Projectile>();
        private static SpellManager myInstance = null;

        private ContentManager contentManager;
        public SceneManager sceneManager;

        #endregion variables

        /// <summary>
        /// Default constructor 
        /// </summary>
        private SpellManager() { }

        public void Initialize(ContentManager contentManager, SceneManager sceneManager)
        {
            this.contentManager = contentManager;
            this.sceneManager = sceneManager;
        }

        #region interface

        /// <summary>
        /// Get an intance of the manager
        /// </summary>
        /// <returns>An intance of the manager</returns>
        public static SpellManager GetInstance()
        {
            if (myInstance == null)
            {
                myInstance = new SpellManager();
            }

            return myInstance;
        }

        /// <summary>
        /// Update the spells
        /// </summary>
        public void Update(Camera camera, GameTime time)
        {
            foreach (Spell s in activeSpells)
            {
                s.Update(camera, time);
            }
 
            DeleteExpired();
        }

        /// <summary>
        /// Draw the spells
        /// </summary>
        public void Draw()
        {
            foreach (Spell s in activeSpells)
            {
                s.Draw();
            }
        }

        /// <summary>Render bounding information</summary>
        public void DrawDebug()
        {
            foreach (Spell s in activeSpells)
            {
                s.DrawDebug();
            }
        }

        /// <summary>
        /// Add a spell to the manager
        /// </summary>
        /// <param name="newSpell"></param>
        public void AddSpell(Spell newSpell)
        {
            activeSpells.Add(newSpell);
        }

        /// <summary>
        /// Checks the manager for a spell
        /// </summary>
        /// <param name="spell">The spell to check for</param>
        /// <returns>True if the spell is found false if not</returns>
        public bool ContainsSpell(Spell spell)
        {
            foreach (Spell s in activeSpells)
            {
                if (s == spell)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks for collisions
        /// </summary>
        /// <param name="target">The entity to check</param>
        /// <returns>True if a collision is detected false if not</returns>
        public bool CheckCollisions(Livable target)
        {
            foreach (Spell s in activeSpells)
            {
                if (s.Owner != target && s.CheckCollisions(target))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks for collisions without activating spell effects
        /// </summary>
        /// <param name="target">The bounding box to check</param>
        /// <returns>True if a collision is detected false if not</returns>
        public bool CheckCollisionsNoEffect(OrientedBoundingBox target)
        {
            foreach (Spell s in activeSpells)
            {
                if (s.CheckCollisionsNoEffect(target))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Delete expired spells
        /// </summary>
        private void DeleteExpired()
        {
            for (int ctr = 0; ctr < activeSpells.Count; ctr++)
            {
                if (!activeSpells[ctr].Active)
                {
                    Spell s = activeSpells[ctr];
                    if(s.SpellRenderer != null)
                        s.SpellRenderer.OnDestroy();
                    activeSpells.RemoveAt(ctr);
                    ctr--;
                }
            }
        }

        #endregion interface

        #region accessors

        public ContentManager ContentManager
        {
            get { return contentManager; }
        }

        public SceneManager SceneManager
        {
            get { return sceneManager; }
        }

        #endregion accessors
    }
}
