using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#pragma warning disable 0168 //unused Exception e

namespace Engine.Managers
{
    /// <summary>The wrapped 2D texture class</summary>
    public sealed class Tex2D
    {
        /// <summary>name and identifier</summary>
        public string filename = "";
        /// <summary>main reference of the texture</summary>
        public Texture2D texture = null;
    }

    /// <summary>The wrapped 3D texture class</summary>
    public sealed class Tex3D
    {
        /// <summary>name and identifier</summary>
        public string filename = "";
        /// <summary>main reference of the texture</summary>
        public TextureCube texture = null;
    }

    /// <summary>The main TextureFactory which all 2D and 3D textures will be loaded from</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class TextureManager
    {
        private static TextureManager myInstance;
        /// <summary>get instance of this singleton</summary>
        /// <returns>singleton instance</returns>
        public static TextureManager getInstance()
        {
            if(myInstance == null)
                myInstance = new TextureManager();
            return myInstance;
        }

        #region MemberVariables

        private ContentManager content; //ref

        private List<Tex2D> texture2DList = new List<Tex2D>();
        private List<Tex3D> texture3DList = new List<Tex3D>();

        private Texture2D errorTexture = null;

        #endregion MemberVariables

        #region Construction

        /// <summary>Private CTOR, getInstance() must be used to retrieve this Factory
        /// This object should be among the first to be initialize in game's initial load</summary>
        private TextureManager() { }

        /// <summary>Initialize pointer to games Content</summary>
        /// <param name="content">the xna asset manager</param>
        public void Initialize(ContentManager content)
        {
            this.content = content;
            errorTexture = content.Load<Texture2D>("textures//error_diffuse");
        }

        /// <summary>Clears both 2D and 3D texture list</summary>
        public void ClearLists()
        {
            texture2DList.Clear();
            texture3DList.Clear();
        }

        #endregion Construction

        #region API

        /// <summary>Call this function to get the pointer of a texture2D, which is loaded in a list found here,</summary>
        /// <param name="filepath">the filepath of the texture2D you are trying to get</param>
        /// <returns>a loaded texture, argumentNullException thrown if unable to find or create</returns>
        public Texture2D GetTexture(string filepath)
        {
            if (content == null)
                throw new ArgumentNullException("TextureFactory::GetTexture() is being called without this factory being Initialized()");

            if (filepath == "error")
                return errorTexture;

            foreach (Tex2D tex in texture2DList)
            {
                if (tex.filename == filepath)
                    return tex.texture;
            }

            Tex2D input = new Tex2D();
            try
            {
                input.texture = content.Load<Texture2D>(filepath);
            }
            catch (Exception e)
            { return errorTexture; }

            input.filename = filepath;
            texture2DList.Add(input);
            return input.texture;
        }

        /// <summary>Get a TextureCube object</summary>
        public TextureCube GetTextureCube(string filepath)
        {
            if (content == null)
                throw new ArgumentNullException("TextureFactory::GetTexture() is being called without this factory being Initialized");

            if (filepath == "error")
                return null;

            foreach (Tex3D cube in texture3DList)
            {
                if (cube.filename == filepath)
                    return cube.texture;
            }

            Tex3D tex = new Tex3D();

            try
            {
                tex.texture = content.Load<TextureCube>(filepath);
                tex.filename = filepath;
            }
            catch (Exception e)
            { return null; }

            texture3DList.Add(tex);

            return tex.texture;
        }

        /// <summary>To get the content filepath of a texture you already have</summary>
        /// <param name="texture">the filepath you want from this textures</param>
        /// <returns>content filepath used for this texture</returns>
        public string GetFilepath(Texture2D texture)
        {
            foreach (Tex2D tex in texture2DList)
            {
                if (tex.texture == texture)
                {
                    return tex.filename;
                }
            }
            return "error";
        }

        /// <summary>To get the content filepath of the texture cube you already have</summary>
        /// <param name="texture">the texture cube you want the filepath to</param>
        /// <returns>string filepath to the content's asset file</returns>
        public string GetFilepathCube(TextureCube texture)
        {
            foreach (Tex3D tex in texture3DList)
            {
                if (tex.texture == texture)
                {
                    return tex.filename;
                }
            }
            return null;
        }

        /// <summary>remove a texture from the master list</summary>
        /// <param name="filepath">filepath of the texture you want to remove</param>
        public void RemoveTexture(string filepath)
        {
            int index = getTextureIndex(filepath);
            if (index == -1)
                throw new ArgumentNullException("TextureFactory::RemoveTexture() the texture was not found within the list");
            else
                texture2DList.RemoveAt(index);
        }

        /// <summary>privately called to get the index of the a texture from a masterlist</summary>
        /// <param name="filepath">the filepath of the texture being seeked</param>
        /// <returns>the index of the texture within the masterlist, or -1 for not found</returns>
        private int getTextureIndex(string filepath)
        {
            int index = 0;
            foreach (Tex2D tex in this.texture2DList)
            {
                if (tex.filename == filepath)
                    return index;
                index++;
            }
            return -1;
        }

        #endregion API

        #region Mutators

        /// <summary>Get-only</summary>
        public List<Tex2D> Texture2DList
        {
            get { return texture2DList; }
        }
        /// <summary>Get-only</summary>
        public List<Tex3D> Texture3DList
        {
            get { return texture3DList; }
        }
        /// <summary>just in case... Get-only</summary>
        public ContentManager Content
        {
            get { return this.content; }
        }

        #endregion Mutators

    }
}
