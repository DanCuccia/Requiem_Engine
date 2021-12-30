using System;
using Microsoft.Xna.Framework.Graphics;
using Engine.Managers;
#pragma warning disable 1591

namespace Engine.Drawing_Objects.Materials
{
    /// <summary>serializable class used to save normal mapped material to xml</summary>
    [Serializable]
    public class NormalMappedMaterialXML : MaterialXML
    {
        public int id;
        public string diffuse;
        public string normal;
        public float[] lightingParameters;
    }

    /// <summary>This material is basic Normal Mapping, using a texture of vectors to depict per pixel normals,
    /// notes: models must have tangents and binormals, do this with the flag within the content processor</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class NormalMappedMaterial : Material
    {
        Texture2D diffuseTexture;
        Texture2D normalsTexture;

        /// <summary>Default CTOR</summary>
        /// <param name="diffuse">loaded diffuse texture</param>
        /// <param name="normals">loaded normals texture</param>
        /// <param name="drawable">reference to the drawable is applies to</param>
        public NormalMappedMaterial(Object3D drawable, Texture2D diffuse, Texture2D normals)
            :base(drawable)
        {
            base.id = GameIDList.Shader_Normals;
            base.Initialize(EffectManager.getInstance().GetEffect("shaders//NormalMapping"));

            if (diffuse != null)
                this.diffuseTexture = diffuse;
            else diffuseTexture = TextureManager.getInstance().GetTexture("error");

            if (normals != null)
                this.normalsTexture = normals;
            else normalsTexture = TextureManager.getInstance().GetTexture("error");
        }

        /// <summary>Set all needed parameters</summary>
        public override void InitializeShader()
        {
            base.effect.Parameters["View"].SetValue(renderer.Camera.ViewMatrix);
            base.effect.Parameters["ViewInverse"].SetValue(renderer.Camera.ViewInverseMatrix);
            base.effect.Parameters["Projection"].SetValue(renderer.Camera.ProjectionMatrix);
            base.effect.Parameters["CameraPosition"].SetValue(renderer.Camera.PositionDot4);
        }

        /// <summary>Update all necessary parameters</summary>
        public override void PreRenderUpdate()
        {
            base.effect.Parameters["Diffuse"].SetValue(this.diffuseTexture);
            base.effect.Parameters["Normals"].SetValue(this.normalsTexture);
            base.loadLightArray();
            base.loadMaterialArray();
        }

        /// <summary>Apply the correct technique and pass to gpu</summary>
        public override void ApplyTechnique()
        {
            base.effect.CurrentTechnique = base.effect.Techniques["NormalMap"];
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

        /// <summary>copy and attach a new normal mapped material</summary>
        public override Material CopyAndAttach(Object3D drawable)
        {
            NormalMappedMaterial mat = new NormalMappedMaterial(drawable, this.diffuseTexture, this.normalsTexture);
            mat.Ambient = this.Ambient;
            mat.Diffuse = this.Diffuse;
            mat.Specular = this.Specular;
            mat.Shine = this.Shine;
            return mat;
        }
    }
}
