using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Engine.Drawing_Objects;
using Engine.Drawing_Objects.Materials;
using Engine.Game_Objects.PreFabs;
using Microsoft.Xna.Framework.Content;
using Engine.Game_Objects;
using Engine.Game_Objects.PlatformBehaviors;
#pragma warning disable 1591
#pragma warning disable 1570

namespace Engine.Managers
{
    /**
     * In here you'll find the hierarchy of objects which Level Editor will load 
     * from a user-created xml.  This list of "EditorInput" objects are all of the 
     * possible assets level editor uses.
     * 
     * This way, when a developer wants to add new models into the level editor, 
     * all you have to do is add it into the EditorModelList.xml file like so:
     * 
     * this example is a staticobject3D, with diffuse material, and the correct
     * filepaths. all elements are marked inside the .xml file.
     * 
     * <EditorInput xsi:type="StaticObject3D_Diffuse">
     *      <model>models//mushroom</model>
     *      <diffuse>textures//ground_diffuse</diffuse>
     * </EditorInput>
     * */
    /// <remarks>Limited Comments, you can figure what class is what for yourself ;)</remarks>
    /// <author>Daniel Cuccia</author>
    [Serializable]
    [XmlInclude(typeof(StaticObject3D_Diffuse))]
    [XmlInclude(typeof(StaticObject3D_NormalMap))]
    [XmlInclude(typeof(StaticObject3D_POM))]
    [XmlInclude(typeof(AnimatedObject3D_Diffuse))]
    [XmlInclude(typeof(AnimatedObject3D_NormalMap))]
    [XmlInclude(typeof(StaticObject3D_Water))]
    [XmlInclude(typeof(StaticObject3D_Null))]
    [XmlInclude(typeof(AnimatedObject3D_Null))]
    [XmlInclude(typeof(PreFab))]
    [XmlInclude(typeof(Quad3D_Texture))]
    [XmlInclude(typeof(Platform_Null))]
    [XmlInclude(typeof(Platform_Diffuse))]
    [XmlInclude(typeof(Platform_NormalMap))]
    [XmlInclude(typeof(Platform_POM))]
    public abstract class EditorInput { }

    [Serializable]
    public class Platform_Null : EditorInput
    {
        public string model;
        public int behavior;
    }

    [Serializable]
    public class Platform_Diffuse : EditorInput
    {
        public string model;
        public string diffuse;
        public int behavior;
    }

    [Serializable]
    public class Platform_NormalMap : EditorInput
    {
        public string model;
        public string diffuse;
        public string normal;
        public int behavior;
    }

    [Serializable]
    public class Platform_POM : EditorInput
    {
        public string model;
        public string diffuse;
        public string normal;
        public int behavior;
    }

    [Serializable]
    public class StaticObject3D_Null : EditorInput
    {
        public string model;
    }

    [Serializable]
    public class StaticObject3D_Diffuse : EditorInput
    {
        public string model;
        public string diffuse;
    }

    [Serializable]
    public class StaticObject3D_NormalMap : EditorInput
    {
        public string model;
        public string diffuse;
        public string normal;
    }

    [Serializable]
    public class StaticObject3D_POM : EditorInput
    {
        public string model;
        public string diffuse;
        public string normal;
    }

    [Serializable]
    public class AnimatedObject3D_Null : EditorInput
    {
        public string model;
    }

    [Serializable]
    public class AnimatedObject3D_Diffuse : EditorInput
    {
        public string model;
        public string diffuse;
    }

    [Serializable]
    public class AnimatedObject3D_NormalMap : EditorInput
    {
        public string model;
        public string diffuse;
        public string normal;
    }

    [Serializable]
    public class StaticObject3D_Water : EditorInput
    {
        public string model;
        public string refraction;
        public string skybox;
    }

    [Serializable]
    public class PreFab : EditorInput
    {
        public int id;
    }

    [Serializable]
    public class Quad3D_Texture : EditorInput
    {
        public string texture;
    }


    /// <summary> This is the main xml I/O container, pretty damn robust huh</summary>
    [Serializable]
    public class EditorAssetList
    {
        /// <summary>the loaded list of possible assets</summary>
        public List<EditorInput> assetList = new List<EditorInput>();
    }


    /// <summary>this class contains the main API function in order to correctly create the EditorInput asset object</summary>
    /// <author>Daniel Cuccia</author>
    public class EditorAssetCreator
    {
        /// <summary> Get Correct asset loaded and ready to go from XML EditorInput object
        /// <remarks>may return null</remarks>
        /// <remarks>This is the ugliest code I've ever written, totally NOT pround of this.</remarks>
        /// <param name="content">xna asset manager</param>
        /// <param name="asset">xml input object</param>
        /// <returns>Object3D corresponding to the EditorInput object</returns>
        public static Object3D GetAsset(ContentManager content, EditorInput asset)
        {
            Object3D output = null;

            TextureManager texManager = TextureManager.getInstance();

            if (asset.GetType() == typeof(StaticObject3D_Null))
            {
                output = new StaticObject3D();
                (output as StaticObject3D).Initialize(content, (asset as StaticObject3D_Null).model);
            }
            else if (asset.GetType() == typeof(StaticObject3D_Diffuse))
            {
                output = new StaticObject3D();
                (output as StaticObject3D).Initialize(content, (asset as StaticObject3D_Diffuse).model);
                output.Material = new DiffuseMaterial(output,
                    texManager.GetTexture((asset as StaticObject3D_Diffuse).diffuse));
            }
            else if (asset.GetType() == typeof(StaticObject3D_NormalMap))
            {
                output = new StaticObject3D();
                (output as StaticObject3D).Initialize(content, (asset as StaticObject3D_NormalMap).model);
                output.Material = new NormalMappedMaterial(output,
                    texManager.GetTexture((asset as StaticObject3D_NormalMap).diffuse),
                    texManager.GetTexture((asset as StaticObject3D_NormalMap).normal));
            }
            else if (asset.GetType() == typeof(StaticObject3D_POM))
            {
                output = new StaticObject3D();
                (output as StaticObject3D).Initialize(content, (asset as StaticObject3D_POM).model);
                output.Material = new ParallaxOcclusionMaterial(output,
                    texManager.GetTexture((asset as StaticObject3D_POM).diffuse),
                    texManager.GetTexture((asset as StaticObject3D_POM).normal));
            }
            else if (asset.GetType() == typeof(StaticObject3D_Water))
            {
                output = new StaticObject3D();
                (output as StaticObject3D).Initialize(content, (asset as StaticObject3D_Water).model);
                output.Material = new WaterMaterial(output,
                    texManager.GetTexture((asset as StaticObject3D_Water).refraction),
                    texManager.GetTextureCube((asset as StaticObject3D_Water).skybox));
            }
            else if (asset.GetType() == typeof(AnimatedObject3D_Null))
            {
                output = new AnimatedObject3D();
                (output as AnimatedObject3D).Initialize(content, (asset as AnimatedObject3D_Null).model);
            }
            else if (asset.GetType() == typeof(AnimatedObject3D_Diffuse))
            {
                output = new AnimatedObject3D();
                (output as AnimatedObject3D).Initialize(content, (asset as AnimatedObject3D_Diffuse).model);
                output.Material = new DiffuseAnimatedMaterial(output,
                    texManager.GetTexture((asset as AnimatedObject3D_Diffuse).diffuse));
            }
            else if (asset.GetType() == typeof(AnimatedObject3D_NormalMap))
            {
                output = new AnimatedObject3D();
                (output as AnimatedObject3D).Initialize(content, (asset as AnimatedObject3D_NormalMap).model);
                output.Material = new NormalMappedAnimatedMaterial(output,
                    texManager.GetTexture((asset as AnimatedObject3D_NormalMap).diffuse),
                    texManager.GetTexture((asset as AnimatedObject3D_NormalMap).normal));
            }
            else if (asset.GetType() == typeof(PreFab))
            {
                output = getPrefab(content, asset);
            }
            else if (asset.GetType() == typeof(Quad3D_Texture))
            {
                output = new Quad3D(Quad3DOrigin.CENTER);
                (output as Quad3D).Initialize(texManager.GetTexture((asset as Quad3D_Texture).texture));
            }
            else if (asset.GetType() == typeof(Platform_Null))
            {
                output = new Platform();
                (output as Platform).Initialize(content, (asset as Platform_Null).model);
                Platform p = output as Platform;
                AssignPlatformType(ref p, (asset as Platform_Null).behavior);
            }
            else if (asset.GetType() == typeof(Platform_Diffuse))
            {
                output = new Platform();
                (output as Platform).Initialize(content, (asset as Platform_Diffuse).model);
                output.Material = new DiffuseMaterial(output, 
                    texManager.GetTexture((asset as Platform_Diffuse).diffuse));
                Platform p = output as Platform;
                AssignPlatformType(ref p, (asset as Platform_Diffuse).behavior);
            }
            else if (asset.GetType() == typeof(Platform_NormalMap))
            {
                output = new Platform();
                (output as Platform).Initialize(content, (asset as Platform_NormalMap).model);
                output.Material = new NormalMappedMaterial(output, 
                    texManager.GetTexture((asset as Platform_NormalMap).diffuse),
                    texManager.GetTexture((asset as Platform_NormalMap).normal));
                Platform p = output as Platform;
                AssignPlatformType(ref p, (asset as Platform_NormalMap).behavior);
            }
            else if (asset.GetType() == typeof(Platform_NormalMap))
            {
                output = new Platform();
                (output as Platform).Initialize(content, (asset as Platform_POM).model);
                output.Material = new ParallaxOcclusionMaterial(output,
                    texManager.GetTexture((asset as Platform_POM).diffuse),
                    texManager.GetTexture((asset as Platform_POM).normal));
                Platform p = output as Platform;
                AssignPlatformType(ref p, (asset as Platform_POM).behavior);
            }

            if (output != null)
            {
                output.GenerateBoundingBox();
                output.UpdateBoundingBox();
            }

            return output;
        }

        /// <summary>assign the correct platform behavior based from integer-to-enum value</summary>
        /// <param name="platform">the platform to assign behavior</param>
        /// <param name="type">the integer (enum) type of behavior</param>
        private static void AssignPlatformType(ref Platform platform, int type)
        {
            switch (type)
            {
                case 0:
                    platform.Behavior = new IStationaryPlatform(platform); break;
                case 1:
                    platform.Behavior = new IHorizontalPlatform(platform); break;
                case 2:
                    platform.Behavior = new IVerticalPlatform(platform); break;
                case 3: 
                    platform.Behavior = new IRotatingPlatform(platform); break;
            }
        }

        /// <summary>PreFabs only come in with an integer id, so we deal with what prefab is what in here</summary>
        /// <param name="asset">the asset from xml</param>
        /// <returns>fully constructed prefabrication object</returns>
        /// <param name="content">xna content manager</param>
        private static Object3D getPrefab(ContentManager content, EditorInput asset)
        {
            PreFabrication output = null;
            PreFab input = asset as PreFab;

            switch (input.id)
            {
                case GameIDList.PreFab_SpawnPoint: output = new PFSpawnPoint(content);
                    break;
                //add as neccessary
            }

            return output;
        }

    }
}
