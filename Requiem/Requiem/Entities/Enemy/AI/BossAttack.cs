using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Requiem.Entities.Enemy.AI
{
    public sealed class BossAttack : Behavior
    {
        public BossAttack(FiniteStateMachine fsm, string next = "wait")
            : base(fsm, next)
        { }

        public override void Start()
        {
            fsm.Owner.Drawable.BeginAnimation("attack", false, idleBoss);
        }

        private void idleBoss()
        {
            base.Finish();
        }

        public override void Update(GameTime time)
        {
            
        }

        public override void Finish()
        {
            base.Finish();
        }

        public override double Evaluate()
        {
            float d = Math.Abs(Vector3.Distance(fsm.Owner.WorldMatrix.Position, fsm.Player.WorldMatrix.Position));

            if (d < 200f)
                return 0.8;
            else return 0.0;
        }
    }
}
