using Microsoft.Xna.Framework;
using Requiem.Scenes;
using Engine.Drawing_Objects;

namespace Requiem.Entities.Enemy.AI
{
    /// <summary>This behavior will tell the fsm's owner to shoot, and default to wait</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class ShootBehavior : Behavior
    {
        public static float Min_Shoot = 300f;
        public static float Max_Shoot = 2000f;

        Line3D shootLine;

        public ShootBehavior(FiniteStateMachine fsm, string next = "wait")
            : base(fsm, next)
        {
            shootLine = new Line3D();
            shootLine.Initialize();

            fsm.Owner.LookAt = fsm.Player.WorldMatrix.Position - fsm.Owner.WorldMatrix.Position;
            fsm.Owner.LookAt.Normalize();

            shootLine.SetWorldStartEnd(Vector3.Zero, Vector3.One, Color.Green, Color.Green);
        }

        public override void Start()
        {
            fsm.Owner.LookAt = fsm.Player.WorldMatrix.Position;

            if (fsm.Owner.SpellList[0] != null)
                fsm.Owner.SpellList[0].Cast();
        }

        public override void Update(GameTime time)
        {
            shootLine.UpdateWorldStartEnd(fsm.Owner.WorldMatrix.Position + new Vector3(fsm.Owner.OBB.Dimensions.Y * 0.5f),
                fsm.Owner.LookAt * ShootBehavior.Max_Shoot);
            Finish();
        }

        public override double Evaluate()
        {
            double output = 0.0;

            Vector3 v = fsm.Player.WorldMatrix.Position - fsm.Owner.WorldMatrix.Position;
            v.Normalize();
            Ray toPlayer = new Ray(fsm.Owner.WorldMatrix.Position, v);

            float n, f;
            if (fsm.Player.OBB.Intersects(toPlayer, out n, out f) != -1)
            {
                output = 1.0;

                if (n > ShootBehavior.Min_Shoot && n < ShootBehavior.Max_Shoot)
                    output = .1;

                if (n > ShootBehavior.Max_Shoot)
                    output = 0.0;
            }
            else output = 0.0;

            return output;
        }

        public override void RenderDebug()
        {
            shootLine.Render();
        }
    }
}
