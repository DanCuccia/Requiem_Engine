using Engine.Managers.Camera;
using Microsoft.Xna.Framework;
using Engine.Drawing_Objects;

namespace Requiem.Spells.RenderEffects
{
    /// <summary>enum listing all types of drawing renderers for spells</summary>
    public enum SpellRendererType
    {
        PROJ_LIGHTNINGTRAIL,
        PROJ_FIREBALL,
        SELF_HEALING,
        PROJ_STRIKE,
        PROJ_WEB,
        PROJ_VAMPIRE
    }

    /// <summary>this base interface is used to create concrete rendering behaviors for projectiles</summary>
    /// <author>Daniel Cuccia</author>
    public interface ISpellRenderer
    {
        /// <summary>after allocation is complete, complete last minute initialization</summary>
        void Initialize();
        /// <summary>render the drawable</summary>
        void Render();
        /// <summary>render debugging information</summary>
        void RenderDebug();
        /// <summary>update drawing objects</summary>
        /// <param name="camera">camera used for culling</param>
        /// <param name="time">current game timing</param>
        void Update(ref Camera camera, GameTime time);
        /// <summary>blah</summary>
        /// <returns>the object3D representing this projectile</returns>
        Object3D GetDrawable();
        /// <summary>called once when the renderer gets destroyed</summary>
        void OnDestroy();
    }
}
