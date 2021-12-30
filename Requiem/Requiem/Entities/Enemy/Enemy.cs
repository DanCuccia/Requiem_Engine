using System.Collections.Generic;
using Engine.Managers.Camera;
using Engine.Managers.Factories;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Requiem.Entities.Enemy.AI;
using Requiem.Movement;
using StillDesign.PhysX;

namespace Requiem.Entities.Enemy
{
    /// <summary>Enemy Base class</summary>
    /// <author>Gabrial Dubois</author>
    public class Enemy : Livable
    {
        #region Member Variables

        private string fileName;
        public FiniteStateMachine Ai { set; get; }

        #endregion Member Variables

        #region Initialization

        /// <summary>Default CTOR </summary>
        /// <param name="fileName">model filepath</param>
        public Enemy(string fileName)
            :base()
        {
            this.fileName = fileName;
        }

        public override void Initialize(ContentManager content)
        {
            model.Initialize(content, fileName);
            model.GenerateBoundingBox();
            model.UpdateBoundingBox();

            this.Actor = PhysXPrimitive.GetPlayerActor(new Vector3(15,25,15), WorldMatrix.Position);
            Actor a = this.Actor;
            WorldMatrix w = this.WorldMatrix;
            this.Movement = new MovementXAxis(ref w, ref a);
        }

        #endregion Initialization

        #region API

        public override void Update(ref Camera camera, GameTime time)
        {
            Ai.Update(time);
            base.Update(ref camera, time);
            if (Movement != null)
                Movement.Update(time);
            model.Update(ref camera, time);

            if (health.CurrentHealth <= 0)
            {
                alive = false;
                
            }
        }

        public override void RenderDebug()
        {
            base.RenderDebug();
            foreach(KeyValuePair<string, Behavior> state in Ai.StateList)
            {
                state.Value.RenderDebug();
            }
        }

        #endregion API
    }
}
