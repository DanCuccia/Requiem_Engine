using Microsoft.Xna.Framework;
using Requiem.Scenes;
using Engine.Drawing_Objects;
using Microsoft.Xna.Framework.Input;
using System;
using Engine.Math_Physics;

namespace Requiem.Entities.Enemy.AI
{
    /// <summary>This behavior will run after the player when inside it's aggro range</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class PursueBehavior : Behavior
    {
        LineSphere3D aggroSphere;

        Ray downRay;
        Line3D downLine;
        
        bool facingRight = true;

        public static float AggroRadius = 200f;
        public static float FinishRange = 50f;
        public static float DownCheckXOffset = 100f;

        public PursueBehavior(FiniteStateMachine fsm, string next = null)
            : base(fsm, next)
        {
            aggroSphere = new LineSphere3D();
            aggroSphere.Initialize(fsm.Owner.WorldMatrix.Position, AggroRadius, Color.Azure);
            downRay = new Ray();
            downRay.Direction = new Vector3(0f, -1f, 0f);
            downLine = new Line3D();
            downLine.Initialize();
        }

        public override void Start()
        {
            fsm.Owner.Drawable.BeginAnimation("run");

            facingRight = (fsm.Owner.WorldMatrix.X < fsm.Player.WorldMatrix.X);

            fsm.Owner.WorldMatrix.rY = MyMath.RotateYToCamera(fsm.Player.WorldMatrix.X, fsm.Player.WorldMatrix.Z, 
                fsm.Owner.WorldMatrix.X, fsm.Owner.WorldMatrix.Z);
            fsm.Owner.WorldMatrix.Z = fsm.Player.zLock;

            aggroSphere.WorldMatrix.Position = fsm.Owner.WorldMatrix.Position;

            downRay.Position.X = fsm.Owner.WorldMatrix.X + (facingRight == true ? PursueBehavior.DownCheckXOffset : -PursueBehavior.DownCheckXOffset);
            downRay.Position.Z = fsm.Owner.WorldMatrix.Z;
            downRay.Position.Y = fsm.Owner.WorldMatrix.Y + (fsm.Owner.OBB.Dimensions.Y * 0.5f);
            downRay.Direction.Y = -1f;
            downLine.SetWorldStartEnd(downRay.Position, downRay.Position + (downRay.Direction * 50f), Color.Pink, Color.Pink);
        }

        public override void Update(GameTime time)
        {
            facingRight = (fsm.Owner.WorldMatrix.X < fsm.Player.WorldMatrix.X);

            // player is in range, attack
            if (Vector3.Distance(fsm.Owner.WorldMatrix.Position, fsm.Player.WorldMatrix.Position) <= PursueBehavior.FinishRange)
            {
                Next = "melee";
                Finish();
            }

            downRay.Position.X = fsm.Owner.WorldMatrix.X + (facingRight == true ? PursueBehavior.DownCheckXOffset : -PursueBehavior.DownCheckXOffset);
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
                Next = "wait";
                Behavior w = fsm.StateList.ContainsKey("wait") ? fsm.StateList["wait"] : null;
                if (w != null)
                    (w as WaitBehavior).WaitMillies = 1000f;
                Finish();
            }

            aggroSphere.Update(ref fsm.Level.camera, time);
        }

        public override void RenderDebug()
        {
            if (fsm.CurrentState != "pursue")
            {
                aggroSphere.WorldMatrix.Position = fsm.Owner.WorldMatrix.Position;
                aggroSphere.UpdateVertices();
                aggroSphere.Render();
            }
            if(fsm.CurrentState == "pursue")
                downLine.Render();
        }

        public override double Evaluate()
        {
            float dist = Math.Abs(Vector3.Distance(fsm.Player.WorldMatrix.Position, fsm.Owner.WorldMatrix.Position));
            if (dist <= PursueBehavior.AggroRadius)
            {
                return 0.5;
            }
            else return 0.0;
        }
    }
}
