#region using

using System.Collections.Generic;
using Requiem.Entities;
using Requiem.Spells.Base;

#endregion

namespace Requiem.Spells.SpellEffects
{
    /// <summary>
    /// 
    /// </summary>
    /// <author> Gabrial Dubois </author>
    public abstract class SpellEffect
    {
        #region variables

        protected List<Livable> targets;
        protected Spell owner;
        protected bool expired;
        protected bool active;

        #endregion

        public SpellEffect() { }

        #region interface

        public abstract void Execute(Livable target);
        public abstract void Update();
        public abstract void Draw();
        public abstract void OnCollision(Livable target);
        public abstract void OnExpired();

        #endregion

        #region accessors

        /// <summary>
        /// Get a reference to the target list
        /// </summary>
        public List<Livable> Targets
        {
            get { return targets; }
        }

        /// <summary>
        /// Get a reference to the owner
        /// </summary>
        public Spell Owner
        {
            get { return owner; }
        }

        /// <summary>
        /// Get or set executed
        /// </summary>
        public bool Expired
        {
            get { return expired; }
            set { expired = value; }
        }

        /// <summary>
        /// get or set active
        /// </summary>
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        #endregion
    }
}
