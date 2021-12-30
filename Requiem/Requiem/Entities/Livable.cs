using System.Collections.Generic;
using Engine.Drawing_Objects;
using Engine.Managers.Camera;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Requiem.Movement;
using Requiem.Entities.EntitySupport;
using Requiem.Spells.Base;
using StillDesign.PhysX;

namespace Requiem.Entities
{
    /// <summary> Base class for all living entities </summary>
    /// <author>Gabrial Dubois</author>
    public abstract class Livable : GameEntity
    {
        #region Member Variables

        protected AnimatedObject3D model = new AnimatedObject3D();
        protected IMovement movement;
        protected List<Spell> spells = new List<Spell>();
        protected Health health;
        protected Vector3 lookAt = new Vector3(1, 0, 0);
        protected bool alive = true;
        #endregion Member Variables

        #region Initialization

        public Livable()
            : base()
        { }

        #endregion Initialization

        #region API

        public override void Update(ref Camera camera, GameTime time)
        {
            model.Update(ref camera, time);
        }

        public override void Render()
        {
            model.Render();
        }

        public override void RenderDebug()
        {
            model.RenderDebug();
        }

        public override void GenerateBoundingBox()
        {
            if(model != null)
                model.GenerateBoundingBox();
        }

        public override void UpdateBoundingBox()
        {
            if(model != null)
                if(model.OBB != null)
                    model.UpdateBoundingBox();
        }

        #endregion API

        #region Mutators

        public IMovement Movement 
        {
            get { return movement; }
            set { movement = value; }
        }

        public Health Health
        {
            get { return health; }
        }

        public Vector3 LookAt
        {
            get { return lookAt; }
            set { lookAt = value; }
        }

        /// <summary>world matrix overrides to point at the drawable</summary>
        public override WorldMatrix WorldMatrix
        {
            get { return model.WorldMatrix; }
            set { model.WorldMatrix = value; }
        }

        /// <summary>obb is overriden to the drawable's obb</summary>
        public override OrientedBoundingBox OBB
        {
            get { return model.OBB; }
            set { model.OBB = value; }
        }

        /// <summary>Actor is overriden to the drawable's actor</summary>
        public sealed override Actor Actor
        {
            get { return model.Actor; }
            set { model.Actor = value; }
        }
        /// <summary>The drawing object this livable primarily uses</summary>
        public AnimatedObject3D Drawable
        {
            get { return this.model; }
        }
        /// <summary>the list of spells this livable owns</summary>
        public List<Spell> SpellList
        {
            get { return this.spells; }
            set { this.spells = value; }
        }

        public bool Alive
        {
            get { return alive; }
            set { alive = value; }
        }

        #endregion Mutators
    }
}
