using Engine.Drawing_Objects;
using Engine.Managers.Camera;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Requiem.Entities;
using Requiem.Entities.EntitySupport;
using Requiem.Spells.Base;
using Requiem.Spells.RenderEffects;
using Requiem.Spells.SpellEffects;
using Requiem.Entities.Enemy;

namespace Requiem.Spells
{
    /// <summary> Fireball projectile spell </summary>
    /// <author> Gabrial Dubois, Daniel Cuccia </author>
    class FireballSpell : ProjectileSpell
    {
        protected Timer castingTimer = null;
        protected bool collided = false;

        /// <summary>Default constructor</summary>
        /// <param name="owner">The entity owning the spell</param>
        public FireballSpell(Livable owner)
            : base(owner) { }

        /// <summary> Base initialize, must be called by the child class </summary>
        public override void Initialize()
        {
            base.Initialize();
 
            coolDown = new Timer(manager.SceneManager.Game, 2500);
            coolDown.StartTimer();
            manager.SceneManager.Game.Components.Add(coolDown);

            castingTimer = new Timer(manager.SceneManager.Game, 50);
            manager.SceneManager.Game.Components.Add(castingTimer);

            icon = new Sprite();
            icon.Initialize("Sprites/FireBall", new Point(1, 1));

            effects.Add(new DamageHealthEffect(this, 25));
        }

        /// <summary> Cast the spell </summary>
        public override void Cast()
        {
            if (coolDown.TimesUp())
            {
                active = true;

                float dist = 0f;
                Vector3 position = Vector3.Zero;

                if (Owner.GetType() == typeof(Player))
                {
                    Ray toEnemyRayCheck = new Ray();
                    toEnemyRayCheck.Position = Owner.WorldMatrix.Position + new Vector3(0f, Owner.OBB.Dimensions.Y * 0.5f, 0f);
                    toEnemyRayCheck.Direction = Owner.LookAt;

                    float n, f;
                    Enemy closestEnemy = null;
                    float closestNear = float.MaxValue;

                    foreach (Enemy enemy in EnemyManager.Instance.ActiveEnemyList)
                    {
                        if (enemy.OBB.Intersects(toEnemyRayCheck, out n, out f) != -1)
                        {
                            if (n < closestNear)
                            {
                                closestNear = n;
                                closestEnemy = enemy;
                            }
                        }
                    }
                    if (closestEnemy != null)
                    {
                        dist = System.Math.Abs(Vector3.Distance(closestEnemy.WorldMatrix.Position, Owner.WorldMatrix.Position));
                        position = closestEnemy.WorldMatrix.Position;
                    }
                    else
                    {
                        dist = 500f;
                        position = Owner.WorldMatrix.Position + new Vector3(0f, Owner.OBB.Dimensions.Y * 0.5f, 0f) + (Owner.LookAt * 500f);
                    }
                }
                else
                {
                    Player player = (SpellManager.GetInstance().sceneManager.GetCurrentScene() as Requiem.Scenes.LevelScene).player;
                    dist = System.Math.Abs(Vector3.Distance(Owner.LookAt, Owner.WorldMatrix.Position));
                    position = player.WorldMatrix.Position;
                }

                Projectile p = new Projectile(Owner.WorldMatrix.Position, Owner.LookAt, 7f, dist, SpellRendererType.PROJ_FIREBALL, BlowUp);

                WorldMatrix w = p.WorldMatrix;
                p.movement = new Requiem.Movement.ProjectileArcMovement(ref w, position, 2000f, 50f);

                p.Initialize(manager.ContentManager);
                projectiles.Add(p);

                if (!manager.ContainsSpell(this))
                {
                    manager.AddSpell(this);
                }
            }
        }

        /// <summary>when the fireball projectile dies, it explodes</summary>
        private void BlowUp()
        {
        }

        /// <summary>
        /// Base update, updates projectiles and spell effects and 
        /// determines if the spell is expired
        /// </summary>
        public override void Update(Camera camera, GameTime time)
        {
            base.Update(camera, time);

            //if (collided)
            //{
            //    if (castingTimer.TimesUpAlt() && numOfExpired == -1)
            //    {
            //        active = false;
            //    }
            //}
        }

        /// <summary>Base collision check, executes the spell's effects if a collision is detected</summary>
        /// <param name="target">The target to check</param>
        /// <returns>True if a collision is detected false if not</returns>
        public override bool CheckCollisions(Livable target)
        {
            if (base.CheckCollisions(target))
            {
                return true;
            }

            return false;
        }

        /// <summary>Alternative collision check, does not execute the spell's effects</summary>
        /// <param name="obb">The bounding box to check</param>
        /// <returns>True if a collision is detected false if not</returns>
        public override bool CheckCollisionsNoEffect(OrientedBoundingBox obb)
        {
            if (base.CheckCollisionsNoEffect(obb))
            {
                return true;
            }

            return false;
        }
    }
}
