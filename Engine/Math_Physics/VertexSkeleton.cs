using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Math_Physics
{
    /// <summary>Custom Vertex for animating models, without tangent space for normal mapped lighting</summary>
    /// <author>Daniel Cuccia</author>
    public struct VertexSkeleton : IVertexType
    {
        /// <summary>position component</summary>
        public Vector3 Position;
        /// <summary>normal component</summary>
        public Vector3 Normal;
        /// <summary>texCoord component</summary>
        public Vector2 TexCoord;
        /// <summary>blending component</summary>
        public byte[] BlendIndices;
        /// <summary>blending weight component</summary>
        public Vector4 BlendWeight;
        /// <summary>how many bytes this vertex is</summary>
        public static int SizeInBytes = (3 + 3 + 2 + 4) * 4 + 4;
        /// <summary>Vertex Declaration</summary>
        public readonly static VertexDeclaration VertexDeclaration
            = new VertexDeclaration(
                 new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                 new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                 new VertexElement(sizeof(float) * 3 * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                 new VertexElement(sizeof(float) * 3 * 3 * 2, VertexElementFormat.Byte4, VertexElementUsage.BlendIndices, 0),
                 new VertexElement((sizeof(float) * 3 * 3 * 2) + (sizeof(byte) * 4), VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0)
             );
        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
    }
}
