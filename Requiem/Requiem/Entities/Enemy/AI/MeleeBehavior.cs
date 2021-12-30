using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Requiem.Entities.Enemy.AI
{
    /// <summary>This behavior casts a melee attack, and waits for their animation to finish before proceeding</summary>
    /// <remarks>if "next" is not set, it will default to "flee"</remarks>
    /// <author>Daniel Cuccia</author>
    public sealed class MeleeBehavior : Behavior
    {
        public static float MeleeDistanceAttackRange = 60f;

        public MeleeBehavior(FiniteStateMachine fsm, string next = "")
            :base(fsm, next)
        { }

        public override void Start()
        {
            fsm.Owner.Drawable.BeginAnimation("chop", false, attackAnimFinished);
            fsm.Player.Health.Hurt(50);
            fsm.Level.camera.Shake(1.5f, 500f);
        }

        private void attackAnimFinished()
        {
            if(String.IsNullOrEmpty(Next))
                Next = "flee";
            Finish();
        }

        public override double Evaluate()
        {
            if (Math.Abs(Vector3.Distance(fsm.Owner.WorldMatrix.Position, fsm.Player.WorldMatrix.Position)) 
                <= MeleeBehavior.MeleeDistanceAttackRange)
            {
                return 1.0;
            }
            return 0.0;
        }

        public override void Update(GameTime time) { }
    }
}
