using Engine.Managers.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Requiem.Environment
{
    /// <summary>This object is for environmental objects that can be destroyed</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class Destructable : Doodad
    {
        /// <summary>Init object</summary>
        /// <param name="content">content manager</param>
        /// <param name="engine">physics engine</param>
        public override void Initialize(ContentManager content)
        {
        }

        /// <summary>generate bounding information</summary>
        public override void GenerateBoundingBox()
        {
        }

        /// <summary>update culling and logic</summary>
        /// <param name="camera">camera to cull with</param>
        /// <param name="time">game time</param>
        public override void Update(ref Camera camera, GameTime time)
        {
        }

        /// <summary>update bounding information</summary>
        public override void UpdateBoundingBox()
        {
        }

        /// <summary>main render call</summary>
        public override void Render()
        {
        }

        /// <summary>render dubugging info</summary>
        public override void RenderDebug()
        {
        }
    }
}
