using System;
using Engine.Managers;

namespace Engine.Drawing_Objects.Materials
{
    /// <summary>Objects that need to draw into depth texture have this extra material</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class DepthAnimatedMaterial : Material
    {
        /// <summary>Default CTOR</summary>
        /// <param name="drawable">drawable in which this material will apply to</param>
        public DepthAnimatedMaterial(Object3D drawable)
            : base(drawable)
        {
            base.ID = GameIDList.Shader_Depth;
            base.effect = EffectManager.getInstance().GetEffect("shaders//Depth");
        }

        /// <summary>Load all values that every same-type-material will use</summary>
        public override void InitializeShader()
        {
            base.effect.Parameters["View"].SetValue(renderer.Camera.ViewMatrix);
            base.effect.Parameters["Projection"].SetValue(renderer.Camera.ProjectionMatrix);
        }

        /// <summary>Update only values needed for this material</summary>
        public override void PreRenderUpdate()
        {        }

        /// <summary>Apply the correct technique/pass to gpu</summary>
        public override void ApplyTechnique()
        {
            base.effect.CurrentTechnique = effect.Techniques["AnimatedDepth"];
            effect.CurrentTechnique.Passes[0].Apply();
        }

        /// <summary>Create a copy of this material and attach it</summary>
        /// <param name="drawable">object to attach copied material to</param>
        /// <returns>the newly created material</returns>
        public override Material CopyAndAttach(Object3D drawable)
        {
            DepthAnimatedMaterial mat = new DepthAnimatedMaterial(drawable);
            return mat;
        }

        /// <summary>Get the XML for this object -- unused</summary>
        public override MaterialXML GetXml()
        { throw new NotImplementedException(); }
    }
}
