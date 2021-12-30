using Microsoft.Xna.Framework;
using Engine.Managers;

namespace Engine.Drawing_Objects.Materials
{
    /// <summary> Null White Material used for drawing animated skeleton objects</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class NullAnimatedMaterial : Material
    {
        /// <summary>color of this object</summary>
        public Vector4 color = new Vector4(.5f, .5f, .5f, 1);

        /// <summary>Default CTOR</summary>
        /// <param name="drawable">the model this material is linked to</param>
        public NullAnimatedMaterial(Object3D drawable)
            : base(drawable)
        {
            base.id = GameIDList.Shader_AnimatedNull;
            base.Initialize(EffectManager.getInstance().GetEffect("shaders//MLColor"));
        }

        /// <summary>Initialize effect parameters which all objects in this batch will draw with</summary>
        public override void InitializeShader()
        {
            base.effect.Parameters["View"].SetValue(renderer.Camera.ViewMatrix);
            base.effect.Parameters["Projection"].SetValue(renderer.Camera.ProjectionMatrix);
            base.effect.Parameters["CameraPosition"].SetValue(renderer.Camera.Position);
        }

        /// <summary>Set any parameters that only this model will draw with</summary>
        public override void PreRenderUpdate()
        {
            base.loadLightArray();
            base.loadMaterialArray();
            effect.Parameters["ModelColor"].SetValue(color);
        }

        /// <summary>Applies the correct technique</summary>
        public override void ApplyTechnique()
        {
            base.effect.CurrentTechnique = base.effect.Techniques["BlinnPhong_AnimatedColor"];
            base.effect.CurrentTechnique.Passes[0].Apply();
        }

        /// <summary>Get the serializable class used to save any needed information of this object</summary>
        /// <returns>serializable object</returns>
        public override MaterialXML GetXml()
        {
            NullMaterialXML output = new NullMaterialXML();
            output.id = this.id;
            output.color = this.color;
            output.lightingProperties = this.lightingProperties;
            return output;
        }

        /// <summary>copy null animated material</summary>
        public override Material CopyAndAttach(Object3D drawable)
        {
            NullAnimatedMaterial mat = new NullAnimatedMaterial(drawable);
            mat.Ambient = this.Ambient;
            mat.Diffuse = this.Diffuse;
            mat.Specular = this.Specular;
            mat.Shine = this.Shine;
            return mat;
        }
    }
}
