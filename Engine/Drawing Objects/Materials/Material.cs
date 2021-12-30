using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Managers;
#pragma warning disable 1591

namespace Engine.Drawing_Objects.Materials
{
    /// <summary>All specific data needed to save and load a material is inherited from this</summary>
    [Serializable]
    [XmlInclude(typeof(DiffuseMaterialXML))]
    [XmlInclude(typeof(LineMaterialXML))]
    [XmlInclude(typeof(WaterMaterialXML))]
    [XmlInclude(typeof(NormalMappedMaterialXML))]
    [XmlInclude(typeof(NullMaterialXML))]
    [XmlInclude(typeof(ParallaxOcclusionMaterialXML))]
    [XmlInclude(typeof(TexturedQuadMaterialXML))]
    public abstract class MaterialXML { }

    /// <summary>The material is the api between model and shader,
    /// this abstract class are what all rendering materials inherit from</summary>
    /// <author>Daniel Cuccia</author>
    public abstract class Material
    {
        /// <summary>indentification</summary>
        protected int                   id;
        /// <summary>renderer reference</summary>
        protected Renderer              renderer;
        /// <summary>xna effect object</summary>
        protected Effect                effect;
        /// <summary>reference to the drawable</summary>
        protected Object3D              drawable;
        /// <summary>lighting properties</summary>
        protected float[]               lightingProperties = 
            {   //ambient, diffuse, specular, shine
                  0.2f,     1f,      .1f,     64f
            };

        public const float MinAmbient = 0f;
        public const float MaxAmbient = 1f;
        public const float MinDiffuse = 0f;
        public const float MaxDiffuse = 2f;
        public const float MinSpecular = 0f;
        public const float MaxSpecular = 50f;
        public const float MinShine = 16f;
        public const float MaxShine = 512f;

        /// <summary>Default CTOR, assign major pointers, initialize must be called from within the inherited classes</summary>
        /// <param name="drawable">base renderable class</param>
        public Material(Object3D drawable)
        {
            this.drawable = drawable;
            this.renderer = Renderer.getInstance();
        }
        /// <summary>The loaded effect is stored and assigned to each model mesh part</summary>
        /// <param name="effect">the loaded effect</param>
        protected void Initialize(Effect effect)
        {
            if (effect == null)
                throw new ArgumentNullException("Material::Initialize: null parameters - unable to initialize");
            this.effect = effect;
        }

        /// <summary>fully assign parameters which all models drawing this material will share</summary>
        public abstract void InitializeShader();
        /// <summary>Override this function, update only shader parameters needed</summary>
        public abstract void PreRenderUpdate();
        /// <summary>Override this function, apply the shader to gpu</summary>
        public abstract void ApplyTechnique();
        /// <summary>Undo any specific rastering changes that may screw up other shaders</summary>
        public virtual void EndShader() { }
        /// <summary>Override this to return a copy of this material</summary>
        /// <param name="drawable">The newly copied material will be attached to this argument</param>
        /// <returns>A copy of this material, with correct references, and duplicate parameters</returns>
        public abstract Material CopyAndAttach(Object3D drawable);
        /// <summary>Get the LightArray containing the closest 4 lights to this material, and input it to the shader</summary>
        protected void loadLightArray()
        {
            Vector4[] lightArray = renderer.GetLightArray(drawable.WorldMatrix.Position);
            if (lightArray == null)
                return;
            
            if ( this.GetType() == typeof(DiffuseMaterial) ||
                this.GetType() == typeof(NullMaterial) ||
                this.GetType() == typeof(NormalMappedMaterial) ||
                this.GetType() == typeof(NullAnimatedMaterial) ||
                this.GetType() == typeof(DiffuseAnimatedMaterial) ||
                this.GetType() == typeof(NormalMappedAnimatedMaterial))
            {
                effect.Parameters["LightArray"].SetValue(lightArray);
            }
        }
        /// <summary>Load the Lighting Material properties to the shader</summary>
        protected virtual void loadMaterialArray()
        {
            if (this.GetType() == typeof(DiffuseMaterial) ||
                this.GetType() == typeof(NullMaterial) ||
                this.GetType() == typeof(NormalMappedMaterial) ||
                this.GetType() == typeof(NullAnimatedMaterial) ||
                this.GetType() == typeof(DiffuseAnimatedMaterial) ||
                this.GetType() == typeof(NormalMappedAnimatedMaterial))
            {
                effect.Parameters["Material"].SetValue(lightingProperties);
            }
        }
        /// <summary>main xna effect object</summary>
        public Effect Effect
        {
            get { return this.effect; }
        }
        /// <summary>identification</summary>
        public int ID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        /// <summary>Each material overrides this to save any unique parameters</summary>
        /// <returns>serializable class containing all information to replicate a material</returns>
        public abstract MaterialXML GetXml();

        /// <summary>Ambient is how dark the model can get, 
        /// if 0, and the model is nowhere near a light, the model will display black,
        /// if 1, the model will display no lighting at all,
        /// default is 0.2</summary>
        public virtual float Ambient
        {
            set { lightingProperties[0] = value; }
            get { return lightingProperties[0]; }
        }
        /// <summary> This is how much diffuse color gets applied to the model,
        /// if 0, the model will display nothing but lighting,
        /// if 1, the model will display normally
        /// if > 1, diffuse colors become exagerated and very bright</summary>
        public virtual float Diffuse
        {
            set { lightingProperties[1] = value; }
            get { return lightingProperties[1]; }
        }
        /// <summary>This is the "shiny" value applied,
        /// at 0, the model displays normally,
        /// at 1, there will be a huge glare from any nearby lights,
        /// if > 1, the glar gets stronger,
        /// default is .1</summary>
        public virtual float Specular
        {
            set { lightingProperties[2] = value; }
            get { return lightingProperties[2]; }
        }
        /// <summary>This is the exponential value of the glare, (how wide the reflecting glare is)
        /// ref: http://content.gpwiki.org/index.php/D3DBook:(Lighting)_Blinn-Phong
        /// if > 64, the glare becomes thinner, making the object look much more glossy
        /// if lessThan 64, the surface area of the glare widens on the model</summary>
        public virtual float Shine
        {
            set { lightingProperties[3] = value; }
            get { return lightingProperties[3]; }
        }
        /// <summary>Set and Get the lighting properties</summary>
        public virtual float[] LightingProperties
        {
            set { this.lightingProperties = value; }
            get { return this.lightingProperties; }
        }

    }
}
