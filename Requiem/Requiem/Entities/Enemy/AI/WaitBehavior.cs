using Microsoft.Xna.Framework;
using Requiem.Scenes;

namespace Requiem.Entities.Enemy.AI
{
    /// <summary>This behavior idles for a given amount of time</summary>
    /// <remarks>evaluate always returns .01, other behaviors will always win</remarks>
    /// <author>Daniel Cuccia</author>
    public sealed class WaitBehavior : Behavior
    {
        float waitUntil = 2000f;
        float currentWait = 0f;
        public float WaitMillies 
        { 
            get { return waitUntil; } 
            set { this.waitUntil = value; } 
        }

        public WaitBehavior(FiniteStateMachine fsm, float duration, string next = null)
            : base(fsm, next)
        {
            this.waitUntil = duration;
        }

        public override void Start()
        {
            fsm.Owner.Drawable.BeginAnimation("idle");
        }

        public override void Update(GameTime time)
        {
            currentWait += time.ElapsedGameTime.Milliseconds;
            if (currentWait >= waitUntil)
            {
                Finish();
            }
        }

        public override void Finish()
        {
            base.Finish();
            currentWait = 0;
        }

        public override double Evaluate()
        {
            return 0.01;
        }
    }
}
