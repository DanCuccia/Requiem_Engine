using System;
using Engine.Drawing_Objects;
using Engine.Drawing_Objects.ParticleSystems;
using Engine.Managers;
using Engine.Managers.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Requiem.Spells.Base;

namespace Requiem.Spells.RenderEffects
{
    /// <summary>This concrete projectile renderer displays a trail of lightning quads</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class LightningProjectileRenderer : ISpellRenderer
    {
        Projectile projectile;
        LightningTrailEmitter drawable;
        GlitterEmitter glitter;

        /// <summary>default ctor</summary>
        /// <param name="p">projectile this renderer is representing</param>
        public LightningProjectileRenderer(Projectile p)
        {
            if (p == null)
                throw new ArgumentNullException("LightningProjectileRenderer::LightningProjectileRenderer - Projectile arg is null");

            this.projectile = p;
            drawable = new LightningTrailEmitter(TextureManager.getInstance().Content);
            glitter = new GlitterEmitter(TextureManager.getInstance().Content);
        }

        /// <summary>generate OBB and reset positions</summary>
        public void Initialize()
        {
            drawable.WorldMatrix.X = projectile.WorldMatrix.X;
            drawable.WorldMatrix.Y = projectile.WorldMatrix.Y;
            drawable.WorldMatrix.Z = projectile.WorldMatrix.Z;
            drawable.GenerateBoundingBox();
            drawable.UpdateBoundingBox();

            glitter.WorldMatrix.X = projectile.WorldMatrix.X;
            glitter.WorldMatrix.Y = projectile.WorldMatrix.Y;
            glitter.WorldMatrix.Z = projectile.WorldMatrix.Z;
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

            glitter.WorldMatrix.X = projectile.WorldMatrix.X;
            glitter.WorldMatrix.Y = projectile.WorldMatrix.Y;
            glitter.WorldMatrix.Z = projectile.WorldMatrix.Z;
            glitter.Update(ref camera, time);
            drawable.Update(ref camera, time);
            drawable.UpdateBoundingBox();
        }

        /// <summary>draw this projectile to screen</summary>
        public void Render()
        {
            if (drawable != null)
                drawable.Render();
            if (glitter != null)
                glitter.Render();
        }

        /// <summary>draw debugging information to screen</summary>
        public void RenderDebug()
        {
            if (drawable != null)
                drawable.RenderDebug();
        }

        /// <summary>This getter is so collision is possible</summary>
        /// <remarks>may return null</remarks>
        /// <returns>the main drawing object representing this projectile</returns>
        public Object3D GetDrawable() { return this.drawable; }

        public void OnDestroy() { }
    }
}
