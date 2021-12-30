using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Math_Physics;

namespace Engine.Drawing_Objects.ParticleSystems
{
    /// <summary>a base class for all particle types to inherit from for any extra needed logic</summary>
    /// <author>Daniel Cuccia</author>
    public class QuadParticle : Particle
    {
        private Quad3D quad;

        /// <summary>Default CTOR</summary>
        public QuadParticle(int instructionCount, Texture2D texture)
            :base(instructionCount)
        {
            quad = new Quad3D();
            quad.Initialize(texture);
            //if (texture != null)
            //    quad.Texture = texture;
        }

        /// <summary>Draws the quad</summary>
        public override void Render()
        {
            quad.Render();
        }

        /// <summary>Main Drawable Accessor</summary>
        public virtual Quad3D Quad
        {
            get { return this.quad; }
            set { this.quad = value; }
        }
        /// <summary>Overriden to the drawable's worldMatrix</summary>
        public override WorldMatrix WorldMatrix
        {
            get { return quad.WorldMatrix; }
            set { quad.WorldMatrix = value; }
        }
        /// <summary>Overriden to the drawable's color</summary>
        public override Color Color
        {
            get { return quad.Color; }
            set { quad.Color = value; }
        }
        /// <summary>Overriden to point to the drawables' alpha</summary>
        public override float Alpha
        {
            get { return quad.Alpha; }
            set { base.Alpha = value; }
        }
    }
}
