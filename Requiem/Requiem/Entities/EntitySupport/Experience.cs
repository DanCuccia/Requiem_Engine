//*****************************************
//Written by Gabrial Dubois
//*****************************************

namespace Requiem.Entities.EntitySupport
{
    class Experience
    {
        #region member variables

        int neededExperience;
        int currentExperience;
        int offset;

        #endregion

        #region constructor

        /// <summary>
        /// Constructs the experience object
        /// </summary>
        /// <param name="neededExperience">Experience need to level up</param>
        /// <param name="offset">The amount that the needed experience increases by with each level</param>
        public Experience(int neededExperience, int offset)
        {
            this.neededExperience = neededExperience;
            this.offset = offset;
            this.currentExperience = 0;
        }

        #endregion

        #region public functions

        /// <summary>
        /// Checks if the current experience has reached the needed experience 
        /// The effects of the level are determined by the entity
        /// </summary>
        /// <returns>True if a level has been reached false if not</returns>
        public bool LevelUp()
        {
            if (currentExperience >= neededExperience)
            {
                ResetExperience();
                neededExperience += offset;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Increase current experience by a specified amount
        /// </summary>
        /// <param name="amount">Amount by which the entities experience is increased</param>
        public void AddExperience(int amount)
        {
            currentExperience += amount;
        }

        /// <summary>
        /// Sets current experience to the needed experience
        /// </summary>
        public void FullLevel()
        {
            currentExperience = neededExperience;
        }

        #endregion

        #region private functions

        /// <summary>
        /// Resets the current experience to zero;
        /// </summary>
        private void ResetExperience()
        {
            currentExperience = 0;
        }

        #endregion

        #region mutaters

        /// <summary>
        /// Get and set needed experience
        /// </summary>
        public int NeededExperience 
        {
            get { return neededExperience; }
            set { neededExperience = value; } 
        }
        
        /// <summary>
        /// Get and set current experience
        /// </summary>
        public int CurrentExperience 
        {
            get { return currentExperience; }
            set { currentExperience = value; }
        }
        
        /// <summary>
        /// Get and set the offset
        /// </summary>
        public int Offset 
        {
            get { return offset; }
            set { offset = value; }
        }

        #endregion
    }
}
