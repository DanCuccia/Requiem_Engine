using Microsoft.Xna.Framework.Graphics;
using Engine.Managers;

namespace Engine.Drawing_Objects.Materials
{
    /// <summary>Material used for animated objects, draws with only a diffuse texture</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class DiffuseAnimatedMaterial : Material
    {
        Texture2D diffuseTexture;

        /// <summary>Default CTOR</summary>
        public DiffuseAnimatedMaterial(Object3D drawable, Texture2D texture)
            : base(drawable)
        {
            base.id = GameIDList.Shader_AnimatedDiffuse;
            base.Initialize(EffectManager.getInstance().GetEffect("shaders//MLColor"));
            if (texture != null)
                this.diffuseTexture = texture;
            else
                this.diffuseTexture = TextureManager.getInstance().GetTexture("error");
        }

        /// <summary>Initialize shader parameters all objects under this material uses</summary>
        public override void InitializeShader()
        {
            base.effect.Parameters["View"].SetValue(renderer.Camera.ViewMatrix);
            base.effect.Parameters["Projection"].SetValue(renderer.Camera.ProjectionMatrix);
            base.effect.Parameters["CameraPosition"].SetValue(renderer.Camera.Position);
        }

        /// <summary>Initialize shader parameters which only this object uses</summary>
        public override void PreRenderUpdate()
        {
            base.loadLightArray();
            base.loadMaterialArray();
            base.effect.Parameters["DiffuseTexture"].SetValue(diffuseTexture);
        }

        /// <summary>Apply the correct technique and pass to gpu</summary>
        public override void ApplyTechnique()
        {
            base.effect.CurrentTechnique = base.effect.Techniques["BlinnPhong_AnimatedTexture"];
            base.effect.CurrentTechnique.Passes[0].Apply();
        }

        /// <summary>Get the serializable class used to save any needed information of this object</summary>
        /// <returns>serializable object</returns>
        public override MaterialXML GetXml()
        {
            DiffuseMaterialXML output = new DiffuseMaterialXML();
            output.id = this.id;
            output.diffuseFilepath = TextureManager.getInstance().GetFilepath(diffuseTexture);
            output.lightingProperties = this.lightingProperties;
            return output;
        }

        /// <summary>Copy and attach a new diffuse animated material</summary>
        public override Material CopyAndAttach(Object3D drawable)
        {
            DiffuseAnimatedMaterial mat = new DiffuseAnimatedMaterial(drawable, this.diffuseTexture);
            mat.Ambient = this.Ambient;
            mat.Diffuse = this.Diffuse;
            mat.Specular = this.Specular;
            mat.Shine = this.Shine;
            return mat;
        }
    }
}
