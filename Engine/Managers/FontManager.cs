using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Managers.Factories
{
    /// <summary>Singleton Font factory to load and get SpriteFonts from the ContentManager,
    /// anywhere from within the project through FontFactory.myInstance</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class FontManager
    {
        /// <summary>The wrapped font, filename acts as ID</summary>
        private class BFont
        {
            public string filename = "";
            public SpriteFont font = null;
        }

        private static FontManager myInstance;
        /// <summary>singleton instance getting</summary>
        /// <returns>singleton instance</returns>
        public static FontManager getInstance()
        {
            if(myInstance == null)
                myInstance = new FontManager();
            return myInstance;
        }

        #region MemberVariables

        private ContentManager content; //ref

        private List<BFont> fontList = new List<BFont>();

        #endregion MemberVariables


        #region Construction

        /// <summary>Private CTOR, getInstance() must be used to retrieve this Factory
        /// This object should be among the first to be initialize in game's initial load</summary>
        private FontManager() { }

        /// <summary>Initialize pointer to games Content</summary>
        /// <param name="content">the xna asset manager</param>
        public void Initialize(ContentManager content)
        {
            this.content = content;
        }

        /// <summary>Clears both 2D and 3D texture list</summary>
        public void ClearList()
        {
            fontList.Clear();
        }

        #endregion Construction


        #region API

        /// <summary>Call this function to get the pointer of a texture2D, which is loaded in a list found here,</summary>
        /// <param name="filepath">the filepath of the texture2D you are trying to get</param>
        /// <returns>a loaded texture, argumentNullException thrown if unable to find or create</returns>
        public SpriteFont Get(string filepath)
        {
            if (content == null)
                throw new ArgumentNullException("TextureFactory::GetTexture() is being called without this factory being Initialized()");

            SpriteFont output = getFont(filepath);
            if (output == null)
            {
                output = addFont(filepath);
            }
            if (output == null)
                throw new ArgumentNullException("TextureFactory::GetTexture() could not get or create a texture2D from filepath: " + filepath);
            
            return output;
        }

        /// <summary>This is privately called to search the 2DTexture List for an existing texture</summary>
        /// <param name="filepath">filepath of the texture</param>
        /// <returns>a loaded texture, or null</returns>
        private SpriteFont getFont(string filepath)
        {
            foreach (BFont tex in fontList)
            {
                if (tex.filename == filepath)
                {
                    return tex.font;
                }
            }
            return null;
        }

        /// <summary>privately called to add a texture into the master list</summary>
        /// <param name="filepath">filepath to the called for texture</param>
        /// <returns>a loaded texture or null</returns>
        private SpriteFont addFont(string filepath)
        {
            BFont f = new BFont();
            f.font = content.Load<SpriteFont>(filepath);
            if (f.font == null)
                return null;
            f.filename = filepath;
            this.fontList.Add(f);
            return f.font;
        }

        /// <summary>remove a texture from the master list</summary>
        /// <param name="filepath">filepath of the texture you want to remove</param>
        public void Remove(string filepath)
        {
            int index = getIndex(filepath);
            if (index == -1)
                throw new ArgumentNullException("TextureFactory::RemoveTexture() the texture was not found within the list");
            else
                fontList.RemoveAt(index);
        }

        /// <summary>privately called to get the index of the a texture from a masterlist</summary>
        /// <param name="filepath">the filepath of the texture being seeked</param>
        /// <returns>the index of the texture within the masterlist, or -1 for not found</returns>
        private int getIndex(string filepath)
        {
            int index = 0;
            foreach (BFont font in this.fontList)
            {
                if (font.filename == filepath)
                    return index;
                index++;
            }
            return -1;
        }

        #endregion API
    }
}
