using Engine.Drawing_Objects;
using Engine.Managers;
using Engine.Managers.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Requiem.Spells.Base;

namespace Requiem.Spells.RenderEffects
{
    /// <summary>visual interface for the "strike" spell projectile</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class StrikeRenderer : ISpellRenderer
    {
        StaticObject3D drawable;
        Projectile projectile;

        /// <summary>basic ctor, initialize for runtime</summary>
        /// <param name="player">pointer to the livable owner</param>
        /// <param name="p">pointer to the projectile this will display</param>
        public StrikeRenderer(Projectile p)
        {
            ContentManager c = TextureManager.getInstance().Content;
            drawable = new StaticObject3D();
            drawable.Initialize(c, "models//sword");
            drawable.WorldMatrix.sX = 2f;
            drawable.WorldMatrix.sZ = 2f;
            projectile = p;
        }

        /// <summary>after allocation is complete, complete last minute initialization</summary>
        public void Initialize()
        {
            drawable.WorldMatrix.X = projectile.WorldMatrix.X;
            drawable.WorldMatrix.Y = projectile.WorldMatrix.Y + 50f;
            drawable.WorldMatrix.Z = projectile.WorldMatrix.Z;
            drawable.WorldMatrix.rY = projectile.WorldMatrix.rY;
            drawable.GenerateBoundingBox();
        }

        /// <summary>render the drawable</summary>
        public void Render()
        {
            if (drawable != null)
                drawable.Render();
        }

        /// <summary>render debugging information</summary>
        public void RenderDebug()
        {
            if (drawable != null)
                drawable.RenderDebug();
        }

        /// <summary>update drawing objects</summary>
        /// <param name="camera">camera used for culling</param>
        /// <param name="time">current game timing</param>
        public void Update(ref Camera camera, GameTime time)
        {
            if (drawable != null)
            {
                drawable.WorldMatrix.X = projectile.WorldMatrix.X;
                drawable.WorldMatrix.Y = projectile.WorldMatrix.Y + 50f;
                drawable.WorldMatrix.Z = projectile.WorldMatrix.Z;
                drawable.WorldMatrix.rY = projectile.WorldMatrix.rY;

                drawable.Update(ref camera, time);
                drawable.UpdateBoundingBox();
            }
        }

        /// <summary>blah</summary>
        /// <returns>the object3D representing this projectile</returns>
        public Object3D GetDrawable() { return drawable; }

        public void OnDestroy() { }
    }
}
