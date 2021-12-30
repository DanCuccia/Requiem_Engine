using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Engine.Drawing_Objects;

namespace Requiem.Entities.Enemy.AI
{
    /// <summary>This behavior will run for a second opposite of the player,
    /// until it hits an edge or a wall.</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class FleeBehavior : Behavior
    {
        public static float RunForMillies = 1000f;
        public static float DownCheckXOffset = 100f;
        public static float maxRunTime = 1500f;

        float currentCount = 0f;

        Ray downRay;
        Line3D downLine;

        public FleeBehavior(FiniteStateMachine fsm, string next = "wait")
            :base(fsm, next)
        {
            downRay = new Ray();
            downRay.Direction = new Vector3(0f, -1f, 0f);
            downLine = new Line3D();
            downLine.Initialize();
            downLine.SetWorldStartEnd(Vector3.Zero, Vector3.One, Color.Pink, Color.Pink);
        }

        public override void Start()
        {
            fsm.Owner.Drawable.BeginAnimation("run");
            fsm.Owner.WorldMatrix.rY = MyMath.RotateYToCamera(fsm.Player.WorldMatrix.X, fsm.Player.WorldMatrix.Z,
                fsm.Owner.WorldMatrix.X, fsm.Owner.WorldMatrix.Z) + 180f;
            currentCount = 0f;
        }

        public override void Update(GameTime time)
        {
            currentCount += time.ElapsedGameTime.Milliseconds;
            if (currentCount >= FleeBehavior.maxRunTime)
            {
                Finish();
            }

            bool facingRight = (fsm.Owner.WorldMatrix.X > fsm.Player.WorldMatrix.X);

            downRay.Position.X = fsm.Owner.WorldMatrix.X + (facingRight == true ? PursueBehavior.DownCheckXOffset : -PursueBehavior.DownCheckXOffset);
            downRay.Position.Y = fsm.Owner.WorldMatrix.Y + (fsm.Owner.OBB.Dimensions.Y * 0.5f);
            downRay.Position.Z = fsm.Owner.WorldMatrix.Z;
            downLine.UpdateWorldStartEnd(downRay.Position, downRay.Position + (downRay.Direction * 50f));

            bool downFound = false;
            float n, f;
            foreach (Object3D obj in fsm.Level.level.collidableList)
            {
                if (obj.OBB.Intersects(downRay, out n, out f) != -1)
                {
                    downFound = true;
                    break;
                }
            }

            if (downFound == true)
            { //we're still on ground, move
                if (facingRight == true)
                    fsm.Owner.Movement.Input(Keys.D);
                else fsm.Owner.Movement.Input(Keys.A);
            }
            else
            { // reached the end, stop and wait a second
                Finish();
            }
        }

        public override void Finish()
        {
            Next = "wait";
            Behavior w = fsm.StateList.ContainsKey("wait") ? fsm.StateList["wait"] : null;
            if (w != null)
                (w as WaitBehavior).WaitMillies = 750f;
            base.Finish();
        }

        public override void RenderDebug()
        {
            if(fsm.CurrentState == "flee")
                downLine.Render();
        }
    }
}
