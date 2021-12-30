//**********************************
//Written by Gabrial Dubois
//**********************************

#region using

using Requiem.Entities;
using Requiem.Spells.Base;

#endregion

namespace Requiem.Spells.Factory
{
    /// <summary>
    /// This is the template for all spell factories.
    /// All concrete factories MUST inherit this class and 
    /// implement the CreateSpell method.
    /// The CreateSpell method should contain all the nessisary 
    /// steps to instantiate the object that the factory creates.
    /// </summary>
    public abstract class SpellFactory
    {
        #region properties

        /// <summary>
        /// An identifier for the factory
        /// Uses the SpellTypes enum
        /// </summary>
        protected SpellTypes spellType;

        #endregion

        public SpellFactory() { }

        public abstract Spell CreateSpell(GameEntity owner);

        #region accessors

        /// <summary>
        /// Set and get the spell type
        /// </summary>
        public SpellTypes Type
        {
            get { return spellType; }
            set { spellType = value; }
        }

        #endregion
    }
}
