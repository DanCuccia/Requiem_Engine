using Microsoft.Xna.Framework;
using Requiem.Scenes;
using Engine.Drawing_Objects;
using Microsoft.Xna.Framework.Input;
using StillDesign.PhysX;
using XRay = Microsoft.Xna.Framework.Ray;
using PV3 = StillDesign.PhysX.MathPrimitives.Vector3;
using Engine.Math_Physics;
using System;
using System.Collections.Generic;

namespace Requiem.Entities.Enemy.AI
{
    /// <summary>This behavior will walk back and fourth, using walls and the current floor as bounds</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class PatrolBehavior : Behavior
    {
        bool facingRight = false;
        
        Line3D downLine;
        Line3D straightLine;

        XRay straightRay = new XRay();
        XRay downRay = new XRay();

        public PatrolBehavior(FiniteStateMachine fsm, string next = null)
            : base(fsm, next)
        {
            downLine = new Line3D();
            downLine.Initialize();
            downLine.SetWorldStartEnd(Vector3.Zero, Vector3.One, Color.Pink, Color.Pink);
            straightLine = new Line3D();
            straightLine.Initialize();
            straightLine.SetWorldStartEnd(Vector3.Zero, Vector3.One, Color.Pink, Color.Pink); 
        }

        public override void Start()
        {
            fsm.Owner.Drawable.BeginAnimation("walk");
        }

        public override void Update(GameTime time)
        {
            if (fsm.StateList.ContainsKey("pursue"))
            { 
                Behavior test = fsm.StateList["pursue"];
                double result = test.Evaluate();
                if (result > 0.0)
                {
                    Next = "pursue";
                    Finish();
                }
            }

            adjustRotation();
            
            straightRay.Position = fsm.Owner.WorldMatrix.Position;
            straightRay.Position.Y += fsm.Owner.OBB.Dimensions.Y * 0.5f;
            straightRay.Direction.X = facingRight == true ? 1f : -1f;

            downRay.Position = fsm.Owner.WorldMatrix.Position;
            downRay.Position.X += facingRight == true ? 50f : -50f;
            downRay.Position.Y += fsm.Owner.OBB.Dimensions.Y * 0.5f;
            downRay.Direction.Y = -1f;
            
            downLine.UpdateWorldStartEnd(downRay.Position, downRay.Position + (downRay.Direction * 75f));
            straightLine.UpdateWorldStartEnd(straightRay.Position, straightRay.Position + (straightRay.Direction * 75f));

            float n, f;
            bool downFound = false;
            foreach (Object3D obj in fsm.Level.level.collidableList)
            {
                if(obj.OBB.Intersects(straightRay, out n, out f) != -1)
                {
                    if(n < 75f)
                    {
                        turnAround();
                        break;
                    }
                }
                if (obj.OBB.Intersects(downRay, out n, out f) != -1)
                {
                    downFound = true;
                }
            }

            if (downFound == false)
                turnAround();

            if (facingRight == true)
                fsm.Owner.Movement.Input(Keys.D);
            else fsm.Owner.Movement.Input(Keys.A);

            PV3 p = fsm.Owner.Actor.GlobalPosition;
            p.Z = fsm.Player.zLock;
            fsm.Owner.WorldMatrix.Z = fsm.Player.zLock;
            fsm.Owner.Actor.GlobalPosition = p;
        }

        private void turnAround()
        {
            facingRight = !facingRight;
            adjustRotation();
        }

        private void adjustRotation()
        {
            if (facingRight == true)
                fsm.Owner.WorldMatrix.rY = 90f;
            else fsm.Owner.WorldMatrix.rY = 270f;
        }

        public override void Finish()
        {
            base.Finish();
        }

        public override double Evaluate()
        {
            float dist = Vector3.Distance(fsm.Owner.WorldMatrix.Position, fsm.Player.WorldMatrix.Position);
            if (dist >= 500)
                return 0.5;
            else
                return 0.02;//0.01 above "wait", other behaviors will win
        }

        public override void RenderDebug()
        {
            if (fsm.CurrentState == "patrol")
            {
                downLine.Render();
                straightLine.Render();
            }
        }
    }
}
