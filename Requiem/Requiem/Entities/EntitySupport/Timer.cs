//*****************************************
//Written by Gabrial Dubois
//*****************************************

#region using

using System;
using Microsoft.Xna.Framework;

#endregion

namespace Requiem.Entities.EntitySupport
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Timer : Microsoft.Xna.Framework.GameComponent
    {
        #region Member variables

        int timerLength;
        int currentTime;
        bool active;

        #endregion

        #region constructor

        public Timer(Microsoft.Xna.Framework.Game game, int timerLength)
            : base(game)
        {
            this.timerLength = timerLength;
            this.currentTime = timerLength;
            this.active = false;
        }

        #endregion

        #region initialize

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        #endregion

        #region update

        /// <summary>
        /// Automatically updates the timer
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (active)
            {
                currentTime -= gameTime.ElapsedGameTime.Milliseconds;
            }

            base.Update(gameTime);
        }

        #endregion

        #region public functions

        /// <summary>
        /// Checks if the timer has run out
        /// If it has the timer will reset but not stop
        /// </summary>
        /// <returns>True if the timer has run out, false if not</returns>
        public bool TimesUp()
        {
            if (currentTime <= 0)
            {
                ResetTimer();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the timer has run out
        /// If it has the timer will stop but not reset
        /// </summary>
        /// <returns>True if the timer has run out, false if not</returns>
        public bool TimesUpAlt()
        {
            if (currentTime <= 0)
            {
                StopTimer();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the timer to active
        /// </summary>
        public void StartTimer()
        {
            active = true;
        }

        /// <summary>
        /// Sets the timer to false
        /// </summary>
        public void StopTimer()
        {
            active = false;
        }

        /// <summary>
        /// Resets the timer
        /// </summary>
        public void ResetTimer()
        {
            currentTime = timerLength;
        }

        #endregion

        #region mutators

        /// <summary>
        /// get or set the timerLegnth 
        /// </summary>
        public int TimerLength
        {
            get { return timerLength; }
            set { timerLength = value; }
        }

        /// <summary>
        /// get or set currentTime
        /// </summary>
        public int CurrentTime
        {
            get { return currentTime; }
            set { currentTime = value; }
        }

        #endregion
    }
}
