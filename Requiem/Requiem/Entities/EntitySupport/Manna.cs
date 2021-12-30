//*****************************************
//Written by Gabrial Dubois
//*****************************************

namespace Requiem.Entities.EntitySupport
{
    class Manna
    {
        #region member variables

        int maxManna;
        int currentManna;

        #endregion

        #region constructor 

        public Manna(int maxManna)
        {
            this.maxManna = maxManna;
            this.currentManna = maxManna;
        }

        #endregion

        #region decrease 

        /// <summary>
        /// Reduce current Manna by a specified amount
        /// </summary>
        /// <param name="amount">Amount by which the entities Manna is reduced</param>
        public void Reduce(int amount)
        {
            currentManna -= amount;
        }

        /// <summary>
        /// Sets current Manna to zero
        /// </summary>
        public void Empty()
        {
            currentManna = 0;
        }

        /// <summary>
        /// Decrease maximum Manna by a specified amount
        /// </summary>
        /// <param name="amount">Amount by which the entities maximum Manna is decrease</param>
        public void DecreaseMaxManna(int amount)
        {
            maxManna += amount;
        }

        #endregion

        #region increase 

        /// <summary>
        /// Increase current Manna by a specified amount
        /// </summary>
        /// <param name="amount">Amount by which the entities Manna is increased</param>
        public void Restore(int amount)
        {
            currentManna += amount;
        }

        /// <summary>
        /// Sets current Manna to its maximum Manna
        /// </summary>
        public void Fill()
        {
            currentManna = maxManna;
        }

        /// <summary>
        /// Increase maximum Manna by a specified amount
        /// </summary>
        /// <param name="amount">Amount by which the entities maximum Manna is increased</param>
        public void IncreaseManna(int amount)
        {
            maxManna += amount;
        }

        #endregion

        #region mutaters

        /// <summary>
        /// Get the entity's max Manna
        /// </summary>
        public int MaxManna
        {
            get { return maxManna; }
        }

        /// <summary>
        /// Get the entity's current Manna
        /// </summary>
        public int CurrentManna
        {
            get { return currentManna; }
        }

        #endregion
    }
}
