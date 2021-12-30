using Engine.Drawing_Objects;
using Engine.Drawing_Objects.ParticleSystems;
using Engine.Managers;
using Engine.Managers.Camera;
using Microsoft.Xna.Framework;
using Engine.Math_Physics;

namespace Requiem.Spells.RenderEffects
{
    /// <summary>the rendering intercae for the regenerate spell</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class RegenerateRenderer : ISpellRenderer
    {
        HealingEmitter emitter;
        WorldMatrix world;
        float heightOffset;
        float startOffset;

        /// <summary>default ctor</summary>
        /// <param name="heightFromWorld">height offset from the world position (world is bottom center oriented)</param>
        /// <param name="world">reference to the world matrix this emitter will follow</param>
        public RegenerateRenderer(ref WorldMatrix world, float heightFromWorld)
        {
            if (world == null)
                throw new System.ArgumentNullException("RegenerateRenderer::RegenerateRenderer - world argument is null");

            this.world = world;
            this.heightOffset = heightFromWorld;
            this.startOffset = heightOffset;

            emitter = new HealingEmitter(TextureManager.getInstance().Content);
        }

        /// <summary>generate OBB and reset positions</summary>
        public void Initialize()
        {
            heightOffset = startOffset;
            emitter.WorldMatrix.X = world.X;
            emitter.WorldMatrix.Y = world.Y + heightOffset;
            emitter.WorldMatrix.Z = world.Z;
        }

        /// <summary>update the drawable's position and itself</summary>
        /// <param name="camera">reference camera for culling</param>
        /// <param name="time">current game time values</param>
        public void Update(ref Camera camera, GameTime time)
        {

            if (emitter == null)
                return;

            emitter.WorldMatrix.X = world.X;
            emitter.WorldMatrix.Y = world.Y + heightOffset;
            emitter.WorldMatrix.Z = world.Z;

            emitter.Update(ref camera, time);
            emitter.UpdateBoundingBox();
        }

        /// <summary>draw this projectile to screen</summary>
        public void Render()
        {
            if (emitter != null)
                emitter.Render();
        }

        /// <summary>draw debugging information to screen</summary>
        public void RenderDebug()
        {
            if (emitter != null)
                emitter.RenderDebug();
        }

        /// <summary>This getter is so collision is possible</summary>
        /// <remarks>may return null</remarks>
        /// <returns>the main drawing object representing this projectile</returns>
        public Object3D GetDrawable() { return emitter; }

        public void OnDestroy() { }
    }
}
