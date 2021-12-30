using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Managers;


namespace Engine.Drawing_Objects.Materials
{
    /// <summary>Serialized data for this material</summary>
    [Serializable]
    public sealed class NullMaterialXML : MaterialXML
    {
        /// <summary>material id</summary>
        public int id;
        /// <summary>color of the material</summary>
        public Vector4 color;
        /// <summary>lighting properties to save</summary>
        public float[] lightingProperties;
    }

    /// <summary>Null Material, all non-animating objects default to this</summary>
    /// <author>Daniel Cuccia</author>
    public class NullMaterial : Material
    {
        /// <summary>color of the model</summary>
        public Vector4 color = new Vector4(.5f, .5f, .5f, 1);
        
        /// <summary>default CTOR</summary>
        /// <param name="drawable">pointer to the object this material is applying to</param>
        public NullMaterial(Object3D drawable)
            : base(drawable)
        {
            base.id = GameIDList.Shader_Null;
            base.Initialize(EffectManager.getInstance().GetEffect("shaders//MLColor"));
        }

        /// <summary>Load all values that every null material will use</summary>
        public override void InitializeShader()
        {
            base.effect.Parameters["View"].SetValue(renderer.Camera.ViewMatrix);
            base.effect.Parameters["Projection"].SetValue(renderer.Camera.ProjectionMatrix);
            base.effect.Parameters["CameraPosition"].SetValue(renderer.Camera.Position);
            renderer.Device.BlendState = BlendState.Opaque;
        }

        /// <summary>Load all values that this material only will use</summary>
        public override void PreRenderUpdate()
        {
            base.loadLightArray();
            base.loadMaterialArray();
            base.effect.Parameters["ModelColor"].SetValue(color);
        }

        /// <summary>Apply the correct technique to the gpu for this materal</summary>
        public override void ApplyTechnique()
        {
            base.effect.CurrentTechnique = base.effect.Techniques["BlinnPhong_Color"];
            base.effect.CurrentTechnique.Passes[0].Apply();
        }

        /// <summary>gets the serializable class related to this material</summary>
        /// <returns>serializable object with any values this material must save</returns>
        public override MaterialXML GetXml()
        {
            NullMaterialXML output = new NullMaterialXML();
            output.color = this.color;
            output.id = this.id;
            output.lightingProperties = this.lightingProperties;
            return output;
        }

        /// <summary>Return a copy of a null material</summary>
        public override Material CopyAndAttach(Object3D drawable)
        {
            NullMaterial mat = new NullMaterial(drawable);
            mat.Ambient = this.Ambient;
            mat.Diffuse = this.Diffuse;
            mat.Specular = this.Specular;
            mat.Shine = this.Shine;
            return mat;
        }
    }
}
