using Microsoft.Xna.Framework;
using Requiem.Scenes;

namespace Requiem.Entities.Enemy.AI
{
    /// <summary>base Behavior class all enemys will behave with</summary>
    /// <author>David Ramirez, Daniel Cuccia</author>
    public abstract class Behavior
    {
        protected FiniteStateMachine fsm;

        public string Next { get; set; }

        /// <summary>constructor</summary>
        /// <param name="player">reference to the player</param>
        /// <param name="level">reference to the level</param>
        /// <param name="fsm">reference to the scene</param>
        /// <param name="next">name of the next behavior or null</param>
        protected Behavior(FiniteStateMachine fsm, string next = null)
        {
            this.fsm = fsm;
            this.Next = next;
        }

        /// <summary>Called once when the behavior begins</summary>
        public abstract void Start();

        /// <summary>per cycle update</summary>
        /// <param name="time">current game times</param>
        public abstract void Update(GameTime time);

        /// <summary>Called once when the behavior finishes</summary>
        public virtual void Finish()
        {
            if (! string.IsNullOrEmpty(Next))
                fsm.SetCurrent(Next);
            else fsm.SetCurrent(fsm.PickNext());
        }

        /// <summary>return a 0-1 value depending any and all conditions</summary>
        /// <returns>0 - not gonna happen, 1 - gonna happen</returns>
        public virtual double Evaluate() { return 0.0; }

        /// <summary>optinal virtual to render extra debugging information</summary>
        public virtual void RenderDebug() { }
    }
}
