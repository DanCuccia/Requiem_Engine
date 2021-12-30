using System;
using Engine.Managers;

namespace Engine.Drawing_Objects.Materials
{
    /// <summary>Line material serializable class</summary>
    [Serializable]
    public class LineMaterialXML : MaterialXML
    {
        /// <summary>material id</summary>
        public int id;
    }

    /// <summary>Default Line material used to draw linear lines in 3D space</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class Line3DMaterial : Material
    {
        /// <summary>Default CTOR</summary>
        /// <param name="drawable">pointer to the model this material is applying to</param>
        public Line3DMaterial(Object3D drawable)
            : base(drawable)
        {
            base.id = GameIDList.Shader_Line;
            base.Initialize(EffectManager.getInstance().GetEffect("shaders//Line"));
        }

        /// <summary>Called by the first model in the batch group to initialize the shader</summary>
        public override void InitializeShader()
        {
            base.effect.Parameters["View"].SetValue(base.renderer.Camera.ViewMatrix);
            base.effect.Parameters["Projection"].SetValue(base.renderer.Camera.ProjectionMatrix);
        }

        /// <summary>Called by each model in the batch group to update it's own values in the shader</summary>
        public override void PreRenderUpdate() { }

        /// <summary>Called by each model in the batch group to apply it's own technique</summary>
        public override void ApplyTechnique()
        {
            base.effect.CurrentTechnique = base.effect.Techniques["Line"];
            base.effect.CurrentTechnique.Passes[0].Apply();
        }

        /// <summary>Get the serializable class used to save any needed information of this object</summary>
        /// <returns>serializable object</returns>
        public override MaterialXML GetXml()
        {
            LineMaterialXML output = new LineMaterialXML();
            output.id = this.id;
            return output;
        }

        /// <summary>copy and attach a new line3D material</summary>
        public override Material CopyAndAttach(Object3D drawable)
        {
            return new Line3DMaterial(drawable);
        }
    }
}
