using System;
using Microsoft.Xna.Framework.Graphics;
using Engine.Managers;
#pragma warning disable 1591

namespace Engine.Drawing_Objects.Materials
{
    /// <summary>XML Serializable class containing all information needed to save this material</summary>
    [Serializable]
    public class ParallaxOcclusionMaterialXML : MaterialXML
    {
        /// <summary>material id</summary>
        public int id;
        /// <summary>diffuse filepath</summary>
        public string diffuseTexture;
        /// <summary>normals filepath</summary>
        public string normalMap;
        /// <summary>parallax properties</summary>
        public float[] displacementProperties;
    }

    /// <summary>Simulate Displacement across a plane, will be referred to as "POM"</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class ParallaxOcclusionMaterial : Material
    {
        Texture2D diffuseTexture;
        Texture2D normalMap;

        float[] displacementProperties = 
        {
              2f,   //texture repeat X
              2f,   //texture repeat Y
              0.15f,//extrusion Displacement
              .3f,  //specular level
              100f, //specular exponent
              1.09f //depth Multiplier
        };

        public static float Min_TextureRepeat = 1f;
        public static float Max_TextureRepeat = 15f;
        public static float Min_Extrusion = 0.0001f;
        public static float Max_Extrusion = 2.0f;
        public static float Min_Depth = 0.04f;
        public static float Max_Depth = 3.0f;
        public static float Min_SpecularLevel = 0f;
        public static float Max_SpecularLevel = 1f;
        public static float Min_SpecularExponent = 2f;
        public static float Max_SpecularExponent = 128f;

        /// <summary> Default CTOR</summary>
        /// <param name="drawable">what object this material is applying to</param>
        /// <param name="diffuse">diffuse texture</param>
        /// <param name="normalMap">normal map, to be used to simulate displacement</param>
        public ParallaxOcclusionMaterial(Object3D drawable, Texture2D diffuse, Texture2D normalMap)
            : base(drawable)
        {
            this.ID = GameIDList.Shader_ParallaxOcclusion;
            base.Initialize(EffectManager.getInstance().GetEffect("shaders//ParallaxOcclusionMapping"));
            if (diffuse != null)
                this.diffuseTexture = diffuse;
            else this.diffuseTexture = TextureManager.getInstance().GetTexture("Error");
            if (normalMap != null)
                this.normalMap = normalMap;
            else this.normalMap = TextureManager.getInstance().GetTexture("Error");
        }

        /// <summary>Initializes any parameters which all of objects of this material will use</summary>
        public override void InitializeShader()
        {
            base.effect.Parameters["View"].SetValue(renderer.Camera.ViewMatrix);
            base.effect.Parameters["ViewInverse"].SetValue(renderer.Camera.ViewInverseMatrix);
            base.effect.Parameters["Projection"].SetValue(renderer.Camera.ProjectionMatrix);
            base.effect.Parameters["CameraPosition"].SetValue(renderer.Camera.Position);
            renderer.Device.BlendState = BlendState.Opaque;
        }

        /// <summary>Initialize any specific parameters for only this drawing object</summary>
        public override void PreRenderUpdate()
        {
            base.effect.Parameters["DiffuseTexture"].SetValue(diffuseTexture);
            base.effect.Parameters["NormalMap"].SetValue(normalMap);

            //this was overriden
            loadMaterialArray();
        }

        /// <summary>Apply the correct teckniqueidk</summary>
        public override void ApplyTechnique()
        {
            base.effect.CurrentTechnique = base.effect.Techniques["Parallax_Occlusion_Mapping"];
            base.effect.CurrentTechnique.Passes[0].Apply();
        }

        /// <summary>Get the serializable object used to save this material</summary>
        /// <returns>serializable object</returns>
        public override MaterialXML GetXml()
        {
            ParallaxOcclusionMaterialXML output = new ParallaxOcclusionMaterialXML();
            output.id = this.ID;
            output.diffuseTexture = TextureManager.getInstance().GetFilepath(diffuseTexture);
            output.normalMap = TextureManager.getInstance().GetFilepath(normalMap);
            output.displacementProperties = this.displacementProperties;
            return output;
        }

        /// <summary>Override material input to our local array</summary>
        protected override void loadMaterialArray()
        {
 	        base.effect.Parameters["fBaseTextureRepeat_x"].SetValue(this.displacementProperties[0]);
            base.effect.Parameters["fBaseTextureRepeat_y"].SetValue(this.displacementProperties[1]);
            base.effect.Parameters["fHeightMapRange"].SetValue(0f);//this.displacementProperties[2]
            base.effect.Parameters["fSpecular"].SetValue(this.displacementProperties[3]);
            base.effect.Parameters["fSpecularExponent"].SetValue(this.displacementProperties[4]);
            base.effect.Parameters["fHeightMapRange_ps"].SetValue(this.displacementProperties[5]);
        }

        /// <summary>copy and attach a new POM material</summary>
        public override Material CopyAndAttach(Object3D drawable)
        {
            ParallaxOcclusionMaterial mat = new ParallaxOcclusionMaterial(drawable, this.diffuseTexture, this.normalMap);
            mat.TextureRepeatX = this.TextureRepeatX;
            mat.TextureRepeatY = this.TextureRepeatY;
            mat.ExtrusionDepth = this.ExtrusionDepth;
            mat.ExtrusionDisplacement = this.ExtrusionDisplacement;
            mat.Specular = this.Specular;
            mat.Shine = this.Shine;
            mat.Ambient = this.Ambient;
            mat.Diffuse = this.Diffuse;
            return mat;
        }

        /// <summary>Get/Set the entire array of properties</summary>
        public float[] DisplacementProperties
        {
            set { this.displacementProperties = value; }
            get { return this.displacementProperties; }
        }
        /// <summary>How many times the texture will wrap X-component, default: 1f</summary>
        public float TextureRepeatX
        {
            set { this.displacementProperties[0] = value; }
            get { return this.displacementProperties[0]; }
        }
        /// <summary>How many times the texture will wrap y-component, default: 1f</summary>
        public float TextureRepeatY
        {
            set { this.displacementProperties[1] = value; }
            get { return this.displacementProperties[1]; }
        }
        /// <summary>How high the extrusion will be, default: 0.05f</summary>
        public float ExtrusionDisplacement
        {
            set { this.displacementProperties[2] = value; }
            get { return this.displacementProperties[2]; }
        }
        /// <summary>Scalar value of the specular shine, 0: no shine, 1: full shine</summary>
        public override float Specular
        {
            set { this.displacementProperties[3] = value; }
            get { return this.displacementProperties[3]; }
        }
        /// <summary>How sharp the specular shine will be</summary>
        public override float Shine
        {
            set { this.displacementProperties[4] = value; }
            get { return this.displacementProperties[4]; }
        }
        /// <summary>How deep the extrusion goes, when set high, deep parts of the extrusion will be black</summary>
        public float ExtrusionDepth
        {
            set { this.displacementProperties[5] = value; }
            get { return this.displacementProperties[5]; }
        }
    }
}
