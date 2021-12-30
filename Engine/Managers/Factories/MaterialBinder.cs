using Microsoft.Xna.Framework.Content;
using Engine.Drawing_Objects;
using Engine.Drawing_Objects.Materials;

namespace Engine.Managers.Factories
{
    /// <summary>The material binder wraps functionality for deserializing materials to objects,
    /// attaching the correct material to the correct type of object
    /// eg. animating material to animating object, as apposed to a static object</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class MaterialBinder
    {
        static MaterialBinder myInstance;
        /// <summary>Get the singleton instance of this object</summary>
        /// <returns>singleton instance</returns>
        public static MaterialBinder getInstance()
        {
            if (myInstance == null)
                myInstance = new MaterialBinder();
            return myInstance;
        }

        ContentManager content;//ref
        TextureManager texManager;//ref

        /// <summary>Default CTOR - private for singleton pattern</summary>
        private MaterialBinder() { }

        /// <summary>Initialize Main pointers</summary>
        /// <param name="content">asset manager</param>
        public void Initialize(ContentManager content)
        {
            this.content = content;
            texManager = TextureManager.getInstance();
        }

        /// <summary>Main API call to attach a material to an Object3D</summary>
        /// <param name="drawable">drawable to attach material to</param>
        /// <param name="material">material file from xml</param>
        /// <returns>whether or not the requested Bind was possible</returns>
        public bool BindMaterial(Object3D drawable, MaterialXML material)
        {
            if (drawable.GetType() == typeof(AnimatedObject3D))
            {
                return attachToAnimated(drawable, material);
            }
            else if (drawable.GetType() == typeof(StaticObject3D))
            {
                return attachToStatic(drawable, material);
            }
            else if (drawable.GetType() == typeof(Line3DMaterial))
            {
                return attachToLine(drawable, material);
            }
            return false;
        }

        /// <summary>privately called to attach the proper animated material to an object</summary>
        private bool attachToAnimated(Object3D drawable, MaterialXML material)
        {
            Material mat = null;
            if (material.GetType() == typeof(DiffuseMaterialXML))
            {
                mat = new DiffuseAnimatedMaterial(drawable, 
                    texManager.GetTexture(((DiffuseMaterialXML)material).diffuseFilepath));
                ((DiffuseAnimatedMaterial)mat).LightingProperties = ((DiffuseMaterialXML)material).lightingProperties;
            }
            else if (material.GetType() == typeof(NormalMappedMaterialXML))
            {
                mat = new NormalMappedAnimatedMaterial(drawable, 
                    texManager.GetTexture(((NormalMappedMaterialXML)material).diffuse),
                    texManager.GetTexture(((NormalMappedMaterialXML)material).normal));
                ((NormalMappedAnimatedMaterial)mat).LightingProperties = ((NormalMappedMaterialXML)material).lightingParameters;
            }
            else if (material.GetType() == typeof(NullMaterialXML))
            {
                mat = new NullAnimatedMaterial(drawable);
                ((NullAnimatedMaterial)mat).color = ((NullMaterialXML)material).color;
                ((NullAnimatedMaterial)mat).LightingProperties = ((NullMaterialXML)material).lightingProperties;
            }

            if (mat != null)
            {
                drawable.Material = mat;
                return true;
            }
            else return false;
        }

        /// <summary>privately called to attach the proper static materials to an object</summary>
        private bool attachToStatic(Object3D drawable, MaterialXML material)
        {
            Material mat = null;

            if (material.GetType() == typeof(DiffuseMaterialXML))
            {
                mat = new DiffuseMaterial(drawable, 
                    texManager.GetTexture(((DiffuseMaterialXML)material).diffuseFilepath));
                ((DiffuseMaterial)mat).LightingProperties = ((DiffuseMaterialXML)material).lightingProperties;
            }
            else if (material.GetType() == typeof(NormalMappedMaterialXML))
            {
                mat = new NormalMappedMaterial(drawable, 
                    texManager.GetTexture(((NormalMappedMaterialXML)material).diffuse),
                    texManager.GetTexture(((NormalMappedMaterialXML)material).normal) );
                ((NormalMappedMaterial)mat).LightingProperties = ((NormalMappedMaterialXML)material).lightingParameters;
            }
            else if (material.GetType() == typeof(WaterMaterialXML))
            {
                mat = new WaterMaterial(drawable, 
                    texManager.GetTexture(((WaterMaterialXML)material).refractionTexture),
                    texManager.GetTextureCube(((WaterMaterialXML)material).skyboxTexture));
                ((WaterMaterial)mat).Settings = ((WaterMaterialXML)material).settings;
            }
            else if (material.GetType() == typeof(NullMaterialXML))
            {
                mat = new NullMaterial(drawable);
                ((NullMaterial)mat).color = ((NullMaterialXML)material).color;
                ((NullMaterial)mat).LightingProperties = ((NullMaterialXML)material).lightingProperties;
            }
            else if (material.GetType() == typeof(ParallaxOcclusionMaterialXML))
            {
                mat = new ParallaxOcclusionMaterial(drawable,
                    texManager.GetTexture(((ParallaxOcclusionMaterialXML)material).diffuseTexture),
                    texManager.GetTexture(((ParallaxOcclusionMaterialXML)material).normalMap));
                ((ParallaxOcclusionMaterial)mat).DisplacementProperties = ((ParallaxOcclusionMaterialXML)material).displacementProperties;
            }

            if (mat != null)
            {
                drawable.Material = mat;
                return true;
            } else return false;
        }

        /// <summary>privately called to attach the proper line material to an object</summary>
        private bool attachToLine(Object3D drawable, MaterialXML material)
        {
            if (material.GetType() == typeof(LineMaterialXML))
            {
                drawable.Material = new Line3DMaterial(drawable);
                return true;
            }
            return false;
        }
    }
}
