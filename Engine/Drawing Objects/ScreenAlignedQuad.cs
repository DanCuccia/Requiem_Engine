using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Drawing_Objects
{
    /// <summary>Scene Imposter, covers the frustrum with a quad billboard,
    /// used in conjuction with Renderer to achieve screen effects</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class ScreenAlignedQuad
    {
        GraphicsDevice device;
        VertexPositionTexture[] verts;
        short[] indices;

        /// <summary>Default CTOR</summary>
        /// <param name="device">device used to create vertex buffer</param>
        public ScreenAlignedQuad(GraphicsDevice device)
        {
            this.device = device;
            verts = new VertexPositionTexture[]
                {
                    new VertexPositionTexture(
                        new Vector3(1,-1,0),
                        new Vector2(1,1)),
                    new VertexPositionTexture(
                        new Vector3(-1,-1,0),
                        new Vector2(0,1)),
                    new VertexPositionTexture(
                        new Vector3(-1,1,0),
                        new Vector2(0,0)),
                    new VertexPositionTexture(
                        new Vector3(1,1,0),
                        new Vector2(1,0))
                };
                
            indices = new short[] { 0, 1, 2, 2, 3, 0 };
        }

        /// <summary>draw the quad to screen</summary>
        public void Draw()
        {
            device.DrawUserIndexedPrimitives<VertexPositionTexture>(
                PrimitiveType.TriangleList, verts, 0, 4, indices, 0, 2);
        }        
    }
}
