using Microsoft.Xna.Framework.Graphics;
using Engine.Managers;

namespace Engine.Drawing_Objects.Materials
{
    /// <summary>Draws an animated skeleton model using normal mapping and point lighting </summary>
    /// <author>Daniel Cuccia</author>
    public sealed class NormalMappedAnimatedMaterial : Material
    {
        Texture2D diffuseTexture;
        Texture2D normalsTexture;

        /// <summary>Default CTOR</summary>
        /// <param name="diffuse">diffuse texture</param>
        /// <param name="normals">normal map</param>
        /// <param name="drawable">pointer to the drawable this is applying to</param>
        public NormalMappedAnimatedMaterial(Object3D drawable, Texture2D diffuse, Texture2D normals)
            : base(drawable)
        {
            base.id = GameIDList.Shader_AnimatedNormals;
            base.Initialize(EffectManager.getInstance().GetEffect("shaders//NormalMapping"));
            
            if (diffuse != null)
                this.diffuseTexture = diffuse;
            else diffuseTexture = TextureManager.getInstance().GetTexture("error");

            if (normals != null)
                this.normalsTexture = normals;
            else normalsTexture = TextureManager.getInstance().GetTexture("error");
        }

        /// <summary>Initialize any properties all drawing objects under the same batch will use</summary>
        public override void InitializeShader()
        {
            base.effect.Parameters["View"].SetValue(renderer.Camera.ViewMatrix);
            base.effect.Parameters["ViewInverse"].SetValue(renderer.Camera.ViewInverseMatrix);
            base.effect.Parameters["Projection"].SetValue(renderer.Camera.ProjectionMatrix);
            base.effect.Parameters["CameraPosition"].SetValue(renderer.Camera.PositionDot4);
        }

        /// <summary>Update any shader parameters only this object will draw with</summary>
        public override void PreRenderUpdate()
        {
            base.loadLightArray();
            base.loadMaterialArray();
            base.effect.Parameters["Diffuse"].SetValue(this.diffuseTexture);
            base.effect.Parameters["Normals"].SetValue(this.normalsTexture);
        }

        /// <summary>Apply the correct technique and pass for this material</summary>
        public override void ApplyTechnique()
        {
            base.effect.CurrentTechnique = base.effect.Techniques["AnimatedNormalMap"];
            base.effect.CurrentTechnique.Passes[0].Apply();
        }

        /// <summary>Get the XML serializable class used to save this object</summary>
        /// <returns>serializable class to save</returns>
        public override MaterialXML GetXml()
        {
            NormalMappedMaterialXML output = new NormalMappedMaterialXML();
            output.id = this.id;
            output.diffuse = TextureManager.getInstance().GetFilepath(diffuseTexture);
            output.normal = TextureManager.getInstance().GetFilepath(normalsTexture);
            output.lightingParameters = this.lightingProperties;
            return output;
        }

        /// <summary>copy and attach a new normal mapped animated material</summary>
        public override Material CopyAndAttach(Object3D drawable)
        {
            NormalMappedAnimatedMaterial mat = new NormalMappedAnimatedMaterial(drawable, this.diffuseTexture, this.normalsTexture);
            mat.Ambient = this.Ambient;
            mat.Diffuse = this.Diffuse;
            mat.Specular = this.Specular;
            mat.Shine = this.Shine;
            return mat;
        }
    }
}
