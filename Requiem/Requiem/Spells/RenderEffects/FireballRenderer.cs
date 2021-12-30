using System;
using Engine.Drawing_Objects;
using Engine.Drawing_Objects.ParticleSystems;
using Engine.Managers;
using Engine.Managers.Camera;
using Microsoft.Xna.Framework;
using Requiem.Spells.Base;
using Engine.Math_Physics;
using Microsoft.Xna.Framework.Content;

namespace Requiem.Spells.RenderEffects
{
    /// <summary>Renders a mega-fire particle system</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class FireballRenderer : ISpellRenderer
    {
        FireBallEmitter drawable;
        Projectile projectile;

        PointLight yellowLight;

        /// <summary>default ctor</summary>
        /// <param name="p">projectile this renderer is representing</param>
        public FireballRenderer(Projectile p) 
        {
            if (p == null)
                throw new ArgumentNullException("LightningProjectileRenderer::LightningProjectileRenderer - Projectile arg is null");

            ContentManager c = TextureManager.getInstance().Content;

            this.projectile = p;
            drawable = new FireBallEmitter(c);

            yellowLight = new PointLight();
            yellowLight.Initialize(c);
            yellowLight.color = MyColors.GoalText;

            Renderer r = Renderer.getInstance();

            if (r.RegisteredLightList != null)
            {
                r.RegisteredLightList.Add(yellowLight);
            }
        }

        /// <summary>last-minute initialization logic (some objects may not be fully constructed (in this constructor)</summary>
        public void Initialize() 
        {
            drawable.WorldMatrix.X = projectile.WorldMatrix.X;
            drawable.WorldMatrix.Y = projectile.WorldMatrix.Y;
            drawable.WorldMatrix.Z = projectile.WorldMatrix.Z;
            drawable.GenerateBoundingBox();
            drawable.UpdateBoundingBox();

            yellowLight.WorldMatrix.Position = projectile.WorldMatrix.Position + (-projectile.movement.Direction() * 20f);
        }

        /// <summary>update the drawable's position and itself</summary>
        /// <param name="camera">reference camera for culling</param>
        /// <param name="time">current game time values</param>
        public void Update(ref Camera camera, GameTime time) 
        {
            if (drawable == null)
                return;

            drawable.WorldMatrix.X = projectile.WorldMatrix.X;
            drawable.WorldMatrix.Y = projectile.WorldMatrix.Y;
            drawable.WorldMatrix.Z = projectile.WorldMatrix.Z;

            drawable.Update(ref camera, time);
            drawable.UpdateBoundingBox();

            yellowLight.WorldMatrix.Position = projectile.WorldMatrix.Position + (-projectile.movement.Direction() * 20f);
            yellowLight.Update(ref camera, time);
        }

        /// <summary>draw this projectile to screen</summary>
        public void Render() 
        {
            if (drawable != null)
                drawable.Render();
        }

        /// <summary>draw debugging information to screen</summary>
        public void RenderDebug() 
        {
            if (drawable != null)
                drawable.RenderDebug();
            if(yellowLight != null)
                yellowLight.Render();
        }

        /// <summary>This getter is so collision is possible</summary>
        /// <remarks>may return null!</remarks>
        /// <returns>the main drawing object representing this projectile</returns>
        public Object3D GetDrawable() { return drawable; }

        public void OnDestroy() 
        {
            if (yellowLight != null)
            {
                Renderer r = Renderer.getInstance();
                r.RegisteredLightList.Remove(yellowLight);
            }
        }
    }
}
