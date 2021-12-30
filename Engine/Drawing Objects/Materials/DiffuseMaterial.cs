using System;
using Microsoft.Xna.Framework.Graphics;
using Engine.Managers;

namespace Engine.Drawing_Objects.Materials
{
    /// <summary>Contains all values needed to save a diffuse material</summary>
    [Serializable]
    public class DiffuseMaterialXML : MaterialXML
    {
        /// <summary>material id</summary>
        public int id;
        /// <summary>diffuse image filepath</summary>
        public string diffuseFilepath;
        /// <summary>lighting properties</summary>
        public float[] lightingProperties;
    }

    /// <summary>Standard non-animating diffuse textured material</summary>
    /// <author>Daniel Cuccia</author>
    public class DiffuseMaterial : Material
    {
        Texture2D diffuseTexture;

        /// <summary>Default CTOR</summary>
        public DiffuseMaterial(Object3D drawable, Texture2D texture)
            : base(drawable)
        {
            base.id = GameIDList.Shader_Diffuse;
            base.Initialize(EffectManager.getInstance().GetEffect("shaders//MLColor"));
            if (texture != null)
                this.diffuseTexture = texture;
            else
                TextureManager.getInstance().GetTexture("DiffuseMaterial::DiffuseMaterial - input texture found null");
        }

        /// <summary>Fully assign effect parameters</summary>
        public override void InitializeShader()
        {
            base.effect.Parameters["View"].SetValue(renderer.Camera.ViewMatrix);
            base.effect.Parameters["Projection"].SetValue(renderer.Camera.ProjectionMatrix);
            base.effect.Parameters["CameraPosition"].SetValue(renderer.Camera.Position);
            Renderer.getInstance().Device.BlendState = BlendState.Opaque;
        }

        /// <summary>Only update parameters needed</summary>
        public override void PreRenderUpdate()
        {
            base.loadLightArray();
            base.loadMaterialArray();
            base.effect.Parameters["DiffuseTexture"].SetValue(diffuseTexture);
        }

        /// <summary>Apply correct technique and pass to gpu</summary>
        public override void ApplyTechnique()
        {
            base.effect.CurrentTechnique = base.effect.Techniques["BlinnPhong_Texture"];
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

        /// <summary>copy and attach a new diffuse material</summary>
        public override Material CopyAndAttach(Object3D drawable)
        {
            DiffuseMaterial mat = new DiffuseMaterial(drawable, this.diffuseTexture);
            mat.Ambient = this.Ambient;
            mat.Diffuse = this.Diffuse;
            mat.Specular = this.Specular;
            mat.Shine = this.Shine;
            return mat;
        }
    }
}
