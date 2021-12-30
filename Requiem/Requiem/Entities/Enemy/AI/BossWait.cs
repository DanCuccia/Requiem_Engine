using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Math_Physics;

namespace Requiem.Entities.Enemy.AI
{
    public sealed class BossWait : Behavior
    {
        float waitUntil = 2000f;
        float currentWait = 0f;
        public float WaitMillies
        {
            get { return waitUntil; }
            set { this.waitUntil = value; }
        }

        public BossWait(FiniteStateMachine fsm, float duration, string next = null)
            : base(fsm, next)
        {
            this.waitUntil = duration;
        }

        public override void Start()
        {
            if(fsm.Owner.Drawable.CurrentAnimation != "idle")
                fsm.Owner.Drawable.BeginAnimation("idle");
            fsm.Owner.Drawable.AnimationSpeed = 1f;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime time)
        {
            currentWait += time.ElapsedGameTime.Milliseconds;
            if (currentWait >= waitUntil)
            {
                Finish();
            }

            fsm.Owner.WorldMatrix.rY = MyMath.RotateYToCamera(fsm.Player.WorldMatrix.X, fsm.Player.WorldMatrix.Z,
                fsm.Owner.WorldMatrix.X, fsm.Owner.WorldMatrix.Z);

            fsm.Owner.WorldMatrix.Z = fsm.Player.zLock;
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
