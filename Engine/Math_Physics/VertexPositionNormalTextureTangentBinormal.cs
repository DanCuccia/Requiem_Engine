using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#pragma warning disable 0649

namespace Engine.Math_Physics
{
    /// <summary>So we can accurately read data from the vertex buffer on items which are using tangent-space lighting,
    /// this is the class we use GetData() with</summary>
    /// <author>Daniel Cuccia</author>
    public struct VertexPositionNormalTextureTangentBinormal : IVertexType
    {
        /// <summary>position component</summary>
        public Vector3 Position;
        /// <summary>normal component</summary>
        public Vector3 Normal;
        /// <summary>texCoord component</summary>
        public Vector2 TexCoord;
        /// <summary>tangent component</summary>
        public Vector3 Tangent;
        /// <summary>BiNormal component</summary>
        public Vector3 Binormal;

        /// <summary>how many bytes this vertex is</summary>
        public static int SizeInBytes = (3 + 3 + 2 + 3 + 3) * 4;
        /// <summary>Vertex Declaration</summary>
        public readonly static VertexDeclaration VertexDeclaration
            = new VertexDeclaration(
                 new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                 new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                 new VertexElement(sizeof(float) * 3 * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                 new VertexElement(sizeof(float) * 3 * 3 * 2, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
                 new VertexElement(sizeof(float) * 3 * 3 * 2 * 3, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0)
             );
        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
    }
}
