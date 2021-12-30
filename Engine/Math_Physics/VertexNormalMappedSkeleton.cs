using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Math_Physics
{
    /// <summary>Custom vertex used to get animated skeleton vertex information, 
    /// used when generating the OBB of a animated model</summary>
    /// <author>Daniel Cuccia</author>
    public struct VertexNormalMappedSkeleton : IVertexType
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
        /// <summary>tangent component</summary>
        public Vector3 Tangent;
        /// <summary>BiNormal component</summary>
        public Vector3 Binormal;

        /// <summary>how many bytes this vertex is</summary>
        public static int SizeInBytes = 76;
        /// <summary>Vertex Declaration</summary>
        public readonly static VertexDeclaration VertexDeclaration
            = new VertexDeclaration(
                 new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                 new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                 new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                 new VertexElement(32, VertexElementFormat.Byte4, VertexElementUsage.BlendIndices, 0),
                 new VertexElement(36, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
                 new VertexElement(52, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
                 new VertexElement(64, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0)
             );
        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
    }
}
