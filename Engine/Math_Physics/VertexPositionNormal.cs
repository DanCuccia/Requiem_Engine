using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Math_Physics
{
    /// <author>Daniel Cuccia</author>
    public struct VertexPositionNormal : IVertexType
    {
        /// <summary>position component</summary>
        public Vector3 Position;
        /// <summary>normal component</summary>
        public Vector3 Normal;

        /// <summary>how many bytes this vertex is</summary>
        public static int SizeInBytes = (3 + 3) * 4;
        /// <summary>Vertex Declaration</summary>
        public readonly static VertexDeclaration VertexDeclaration
            = new VertexDeclaration(
                 new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                 new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
             );
        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
    }
}
