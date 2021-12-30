using Engine.Drawing_Objects;
using Engine.Managers.Camera;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Requiem.Entities;
using Requiem.Movement;
using Requiem.Spells.RenderEffects;
using Requiem.Entities.EntitySupport;

namespace Requiem.Spells.Base
{
    /// <summary>The main projectile object</summary>
    /// <author>Gabriel Dubois, Daniel Cuccia</author>
    public class Projectile : GameEntity
    {
        public ISpellRenderer drawing;
        public IMovement movement;
        public Timer lifeTime;

        public Vector3 startPosition;
        public float Range;

        public delegate void OnExpiredCallback();
        OnExpiredCallback callback;

        public bool Alive { set; get; }

        #region init
        /// <summary>fully constructs the projectile</summary>
        /// <param name="position">beginning position</param>
        /// <param name="direction">movement vector</param>
        /// <param name="velocity">movement units per cycle</param>
        /// <param name="type">drawing type</param>
        public Projectile(Vector3 position, Vector3 direction, float velocity, float range, SpellRendererType type, OnExpiredCallback callback = null, Timer lifeTime = null)
            :base()
        {
            this.Range = range;
            Alive = true;

            this.drawing = SpellRendererFactory.GetProjectileRenderer(this, type);

            this.WorldMatrix.Position = position;
            this.WorldMatrix.ForceUpdateMatrix();

            WorldMatrix w = this.WorldMatrix;
            direction.Normalize();
            this.movement = new ProjectileMovement(ref w, direction, velocity);

            this.startPosition = WorldMatrix.Position;

            this.callback = callback;
            
            this.lifeTime = lifeTime;
        }

        /// <summary>do final initializing to this projectile</summary>
        public override void Initialize(ContentManager content) 
        {
            this.drawing.Initialize();
        }
        #endregion

        #region API
        /// <summary>main update call</summary>
        public override void Update(ref Camera camera, GameTime time)
        {
            float dist = System.Math.Abs(Vector3.Distance(startPosition, WorldMatrix.Position));
            if (dist >= Range - 1f)
            {
                Alive = false;
            }

            if (lifeTime != null)
            {
                if (lifeTime.TimesUpAlt())
                    Alive = false;
            }

            movement.Update(time);
            drawing.Update(ref camera, time);
        }

        /// <summary>main draw call (uses IProjectileRenderer system)</summary>
        public override void Render()
        {
            drawing.Render();
        }

        /// <summary>debugging draw call (uses IProjectileRenderer system)</summary>
        public override void RenderDebug() 
        {
            drawing.RenderDebug();
        }

        /// <summary>generates the drawables bounding information</summary>
        public override void GenerateBoundingBox()
        {
            Object3D d = drawing.GetDrawable();
            if (d != null)
                d.GenerateBoundingBox();
        }

        /// <summary>update the drawables bounding information</summary>
        public override void UpdateBoundingBox() 
        {
            Object3D d = drawing.GetDrawable();
            if (d != null)
                d.UpdateBoundingBox();
        }
        #endregion

        #region Overrides
        /// <summary>override to projectile model</summary>
        public Object3D Model
        {
            get { return drawing.GetDrawable(); }
        }
        /// <summary>override to projectile model</summary>
        public override WorldMatrix WorldMatrix
        {
            get { return drawing.GetDrawable().WorldMatrix; }
            set { drawing.GetDrawable().WorldMatrix = value; }
        }
        /// <summary>override to projectile model</summary>
        public override OrientedBoundingBox OBB
        {
            get { return drawing.GetDrawable().OBB; }
            set { drawing.GetDrawable().OBB = value; }
        }
        #endregion
    }
}

