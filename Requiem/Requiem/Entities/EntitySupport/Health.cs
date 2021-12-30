//*****************************************
//Written by Gabrial Dubois
//*****************************************

namespace Requiem.Entities.EntitySupport
{
    public class Health
    {
        #region member variables

        int maxHealth;
        int currentHealth;
        float damageReduction = 1;

        #endregion

        #region constructor 

        public Health(int maxHealth)
        {
            this.maxHealth = maxHealth;
            this.currentHealth = maxHealth;
        }

        #endregion

        #region decrease 

        /// <summary>
        /// Reduce current health by a specified amount
        /// </summary>
        /// <param name="amount">Amount by which the entities health is reduced</param>
        public void Hurt(int amount)
        {
            currentHealth -= (int)(amount * damageReduction);
        }

        /// <summary>
        /// Sets current health to zero
        /// </summary>
        public void Kill()
        {
            currentHealth = 0;
        }

        /// <summary>
        /// Decrease maximum health by a specified amount
        /// </summary>
        /// <param name="amount">Amount by which the entities maximum health is decrease</param>
        public void DecreaseHealth(int amount)
        {
            maxHealth += amount;
        }

        #endregion

        #region increase 

        /// <summary>
        /// Increase current health by a specified amount
        /// </summary>
        /// <param name="amount">Amount by which the entities health is increased</param>
        public void Heal(int amount)
        {
            currentHealth += amount;

            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }

        /// <summary>
        /// Sets current health to its maximum health
        /// </summary>
        public void FullHeal()
        {
            currentHealth = maxHealth;
        }

        /// <summary>
        /// Increase maximum health by a specified amount
        /// </summary>
        /// <param name="amount">Amount by which the entities maximum health is increased</param>
        public void IncreaseHealth(int amount)
        {
            maxHealth += amount;
        }

        #endregion

        #region mutators

        /// <summary>
        /// Get the entity's max health
        /// </summary>
        public int MaxHealth
        {
            get { return maxHealth; }
        }

        /// <summary>
        /// Get the entity's current health
        /// </summary>
        public int CurrentHealth
        {
            get { return currentHealth; }
            set { currentHealth = value; }
        }

        /// <summary>
        /// The amount by which the incomeing damage is reduced
        /// 1 = no reduction, 0 = no damage
        /// </summary>
        public float DamageReduction
        {
            get { return damageReduction; }
            set { damageReduction = value; }
        }

        #endregion
    }
}
