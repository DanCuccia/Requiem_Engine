using Engine.Drawing_Objects;
using Engine.Drawing_Objects.ParticleSystems;
using Engine.Managers;
using Engine.Managers.Camera;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;

namespace Requiem.Spells.RenderEffects
{
    /// <summary>renderer used to display the 'vampire' spell</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class VampireRenderer : ISpellRenderer
    {
        VampireEmitter emitter;
        WorldMatrix owner;

        /// <summary>default ctor</summary>
        /// <param name="heightFromWorld">height offset from the world position (world is bottom center oriented)</param>
        /// <param name="world">reference to the world matrix this emitter will follow</param>
        public VampireRenderer(ref WorldMatrix world)
        {
            emitter = new VampireEmitter(TextureManager.getInstance().Content);
            owner = world;
        }

        /// <summary>generate OBB and reset positions</summary>
        public void Initialize()
        {
            if (emitter != null)
            {
                emitter.WorldMatrix.X = owner.X;
                emitter.WorldMatrix.Y = owner.Y + 75f;
                emitter.WorldMatrix.Z = owner.Z;
            }
        }

        /// <summary>update the drawable's position and itself</summary>
        /// <param name="camera">reference camera for culling</param>
        /// <param name="time">current game time values</param>
        public void Update(ref Camera camera, GameTime time)
        {
            if (emitter != null)
            {
                emitter.WorldMatrix.X = owner.X;
                emitter.WorldMatrix.Y = owner.Y + 75f;
                emitter.WorldMatrix.Z = owner.Z;
                emitter.Update(ref camera, time);
            }
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
