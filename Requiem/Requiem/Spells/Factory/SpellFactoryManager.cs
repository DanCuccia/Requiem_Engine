//**********************************
//Written by Gabrial Dubois
//**********************************

#region using

using System.Collections.Generic;
using Requiem.Entities;
using Requiem.Spells.Base;

#endregion

namespace Requiem.Spells.Factory
{
    /// <summary>
    /// This class is a sigleton that manages all concrete factories
    /// It contains methods to add a factory, 
    /// check that a factory exists and create an object 
    /// </summary>
    class SpellFactoryManager
    {
        /// <summary>
        /// The list of availiable factories 
        /// </summary>
        private List<SpellFactory> factories = new List<SpellFactory>();

        private static SpellFactoryManager myInstance = null;

        #region initialize

        /// <summary>
        /// Default constructor 
        /// </summary>
        private SpellFactoryManager() 
        {
            Initialize();
        }

        /// <summary>
        /// Performs any nessisary initialization
        /// Must be called before the maneger can be used
        /// </summary>
        public void Initialize()
        {
        }

        #endregion initialize

        #region API

        /// <summary>
        /// Get an intance of the manager
        /// </summary>
        /// <returns>An intance of the manager</returns>
        public static SpellFactoryManager GetInstance()
        {
            if (myInstance == null)
            {
                myInstance = new SpellFactoryManager();
            }

            return myInstance;
        }

        /// <summary>
        /// Add a factory to the manager
        /// If the new factory's type and name match an already 
        /// exisiting factory the new factory will not be added 
        /// </summary>
        /// <param name="newFactory">Spell factory to add</param>
        public void AddFactory(SpellFactory newFactory)
        {
            if (FindFactory(newFactory.Type)) 
            { 
                return; 
            }

            factories.Add(newFactory);
        }

        /// <summary>
        /// Creates a spell based on the requested spell type
        /// </summary>
        /// <param name="type">The type of factory to use</param>
        /// <returns>A spell if a requested factory exists, otherwise throws an argument exception</returns>
        public Spell CreateSpell(SpellTypes type, GameEntity owner)
        {
            foreach (SpellFactory factory in factories)
            {
                if (factory.Type == type)
                {
                    return factory.CreateSpell(owner);
                }
            }

            throw new System.ArgumentException("No valid factory exists", "type");
        }

        /// <summary>
        /// Searches the manager for a factory based on a type
        /// </summary>
        /// <param name="type">The type identifier of the factory</param>
        /// <returns>True if the factory exists false if not</returns>
        public bool FindFactory(SpellTypes type)
        {
            foreach (SpellFactory factory in factories)
            {
                if (type == factory.Type)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion API
    }
}
