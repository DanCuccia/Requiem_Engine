using Engine.Math_Physics;
using Engine.Managers.Camera;
using Microsoft.Xna.Framework;
using Engine.Drawing_Objects;

namespace Requiem.Spells.RenderEffects
{
    /// <summary>This is a the null renderer for spells, notice it takes a reference to a worldMatrix for updating</summary>
    public sealed class NullSpellRenderer : ISpellRenderer
    {
        WorldMatrix world;

        public NullSpellRenderer(ref WorldMatrix worldMatrix)
        {
            this.world = worldMatrix;
        }

        public void Initialize() { }
        public void Update(ref Camera camera, GameTime time) { }
        public void Render() { }
        public void RenderDebug() { }
        public Object3D GetDrawable() { return null; }
        public void OnDestroy() { }
    }
}
