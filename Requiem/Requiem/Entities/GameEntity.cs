using Engine.Drawing_Objects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StillDesign.PhysX;

namespace Requiem.Entities
{
    /// <summary>The base abstract for all game-type objects</summary>
    /// <author>Gabrial Dubois, modified by Daniel Cuccia</author>
    public abstract class GameEntity : Object3D
    {
        public string Name { set; get; }

        /// <summary>Default CTOR </summary>
        /// <param name="device">reference to graphics device</param>
        /// <param name="world">reference to the world manger</param>
        public GameEntity()
            : base()
        {
            this.collidable = true;
        }

        public abstract void Initialize(ContentManager content);

        #region Unused
        public override sealed void CreateFromXML(ContentManager content, XMLMedium inputXml)  { }
        public override sealed Object3D GetCopy(ContentManager content) { return null; }
        public override sealed XMLMedium GetXML() { return null; }
        public override sealed void RenderImplicit(Effect effect) { }
        #endregion
    }
}
