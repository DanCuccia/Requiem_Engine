using Engine.Drawing_Objects;
using Engine.Managers.Camera;
using Microsoft.Xna.Framework;
using Requiem.Spells.Base;

namespace Requiem.Spells.RenderEffects
{
    /// <summary>Null and default Projectile Renderer</summary>
    public sealed class NullProjectileRenderer : ISpellRenderer
    {
        /// <summary>default ctor</summary>
        /// <param name="p">projectile this renderer is representing</param>
        public NullProjectileRenderer(Projectile p) { }

        /// <summary>last-minute initialization logic (some objects may not be fully constructed (in this constructor)</summary>
        public void Initialize() { }

        /// <summary>update the drawable's position and itself</summary>
        /// <param name="camera">reference camera for culling</param>
        /// <param name="time">current game time values</param>
        public void Update(ref Camera camera, GameTime time) { }

        /// <summary>draw this projectile to screen</summary>
        public void Render() { }

        /// <summary>draw debugging information to screen</summary>
        public void RenderDebug() { }

        /// <summary>This getter is so collision is possible</summary>
        /// <remarks>may return null!</remarks>
        /// <returns>the main drawing object representing this projectile</returns>
        public Object3D GetDrawable() { return null; }

        public void OnDestroy() { }
    }
}
