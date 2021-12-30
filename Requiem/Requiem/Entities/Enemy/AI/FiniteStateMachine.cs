using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Requiem.Scenes;

namespace Requiem.Entities.Enemy.AI
{
    /// <summary>FSM which controls the AI for enemies</summary>
    /// <author>David Ramirez, Daniel Cuccia</author>
    public class FiniteStateMachine
    {
        #region MemberVars
        Dictionary<string, Behavior> states = new Dictionary<string, Behavior>();
        public Dictionary<string, Behavior> StateList { get { return states; } }
        
        string currentState = null;
        public string CurrentState { get { return currentState; } }
        string fallback = "wait";

        public Livable Owner { get; protected set; }
        public Player Player { get; protected set; }
        public LevelScene Level { get; protected set; }
        #endregion

        #region Init
        /// <summary>CTOR</summary>
        public FiniteStateMachine(Livable owner, LevelScene level)
        {
            this.Owner = owner;
            this.Level = level;
            this.Player = level.player;
        }

        /// <summary>add a new behavior to this FSM</summary>
        /// <param name="name">key name</param>
        /// <param name="behavior">initiated behavior</param>
        public void AddState(string name, Behavior behavior, bool fallback = false)
        {
            if (string.IsNullOrEmpty(name) || behavior == null)
                throw new ArgumentNullException("FSM::AddState - found null argument");
            states.Add(name, behavior);
            if (fallback == true)
                this.fallback = name;
        }
        #endregion

        #region API
        public void BeginAutomation()
        {
            currentState = fallback;
            states[currentState].Start();
        }

        /// <summary>update the current behavior</summary>
        /// <param name="time">current time</param>
        public void Update(GameTime time)
        {
            if (String.IsNullOrEmpty(currentState))
                return;

            if (states.ContainsKey(currentState))
                states[currentState].Update(time);
            else
                throw new InvalidOperationException("FSM::Update - Behavior Key was not found");
        }

        /// <summary>evaluate all of the behaviors, and choose what is next</summary>
        /// <returns>string behavior key who evaulated the highest</returns>
        public string PickNext()
        {
            string next = fallback;
            double max = 0.0;
            foreach (KeyValuePair<string, Behavior> b in states)
            {
                double t = b.Value.Evaluate();
                if (t > max)
                {
                    max = t;
                    next = b.Key;
                }
            }
            return next;
        }

        /// <summary>set the current behavior</summary>
        /// <param name="behaviorName">name of the behavior</param>
        public void SetCurrent(string behaviorName)
        {
            currentState = String.IsNullOrEmpty(behaviorName) ? fallback : behaviorName;
            states[currentState].Start();
        }
        #endregion
    }
}
