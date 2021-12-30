using System;
using Engine.Drawing_Objects;
using Engine.Managers;
using Engine.Managers.Camera;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Requiem.Spells.RenderEffects
{
    /// <summary>the projectile renderer used for the "webtrap" spell</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class WebTrapRenderer : ISpellRenderer
    {
        WorldMatrix worldMatrix;
        StaticObject3D drawable;
        
        static float startScale = 20f;
        static float endScale = 200f;
        static float scaleDuration = 1500f;
        float currentTime = 0f;

        static float startHeight = 200f;
        float currentHeight = WebTrapRenderer.startHeight;
        float spawnHeight = 0f;

        /// <summary>basic ctor, initialize for runtime</summary>
        /// <param name="player">pointer to the livable owner</param>
        /// <param name="p">pointer to the projectile this will display</param>
        public WebTrapRenderer(ref WorldMatrix world)
        {
            if (world == null)
                throw new ArgumentNullException("WebTrapRenderer::WebTrapRenderer - null worldMatrix argument");

            ContentManager c = TextureManager.getInstance().Content;

            drawable = new StaticObject3D();
            drawable.Initialize(c, "models//web");
            drawable.Material.Ambient = 0.5f;
            drawable.Material.Shine = 64f;
            drawable.Material.Specular = 0.6f;
            drawable.WorldMatrix.Position = world.Position;

            this.worldMatrix = world;
        }

        /// <summary>after allocation is complete, complete last minute initialization</summary>
        public void Initialize()
        {
            drawable.GenerateBoundingBox();
            drawable.WorldMatrix.Position = worldMatrix.Position;
            spawnHeight = worldMatrix.Y;

            drawable.WorldMatrix.UniformScale = WebTrapRenderer.startScale;
            currentTime = 0f;
            currentHeight = WebTrapRenderer.startHeight;
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
            currentTime += time.ElapsedGameTime.Milliseconds;
            float scalar = MyMath.GetScalarBetween(0f, WebTrapRenderer.scaleDuration, currentTime);

            drawable.WorldMatrix.UniformScale = 
                MyMath.GetValueBetween(WebTrapRenderer.startScale, WebTrapRenderer.endScale, scalar);

            currentHeight = MyMath.GetValueBetween(0, WebTrapRenderer.startHeight, 1f - scalar);
            drawable.WorldMatrix.Y = spawnHeight + currentHeight;

            if (drawable != null)
            {
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
