using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Engine.Managers
{
    /// <summary>A wrapped effect with an ID (RenderEffect enum), used in the manager</summary>
    public sealed class BEffect
    {
        /// <summary>name of effect and identifier</summary>
        public string Name;
        /// <summary>main reference of the effect</summary>
        public Effect Effect;
    }

    /// <summary>Manages all loaded effect</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class EffectManager
    {
        List<BEffect> fxList = new List<BEffect>();
        ContentManager content;

        private static EffectManager myInstance;
        /// <summary>singleton getter</summary>
        /// <returns>singleton instance of this object</returns>
        public static EffectManager getInstance()
        {
            if (myInstance == null)
                myInstance = new EffectManager();
            return myInstance;
        }
        /// <summary>null CTOR</summary>
        private EffectManager() { }

        /// <summary>initialize contentmanager pointer</summary>
        /// <param name="content">pointer to xna asset manager</param>
        public void Initialize(ContentManager content)
        {
            this.content = content;
        }

        /// <summary>Get a loaded Effect from the list</summary>
        /// <param name="filepath">filepath to effect</param>
        /// <returns>the loaded ready-to-use effect</returns>
        public Effect GetEffect(string filepath)
        {
            foreach (BEffect effect in fxList)
            {
                if (effect.Name == filepath)
                    return effect.Effect;
            }
            Effect output = loadEffect(filepath);
            if (output == null)
                throw new ArgumentNullException("EffectManager::GetEffect() unable to find or load the effect");
            return output;
        }

        /// <summary>If the effect was not found in the list, try to load it</summary>
        /// <param name="filepath">filepath of the effect</param>
        private Effect loadEffect(string filepath)
        {
            BEffect fx = new BEffect();
            fx.Effect = content.Load<Effect>(filepath);
            fx.Name = filepath;
            this.fxList.Add(fx);
            return fx.Effect;
        }

        /// <summary>An an effect to the list</summary>
        /// <param name="filepath">filepath to effect</param>
        public void AddEffect(string filepath)
        {
            BEffect t = new BEffect();
            t.Effect = this.content.Load<Effect>(filepath);
            if (t.Effect == null)
                throw new ArgumentNullException("EffectManager::AddEffect could not load the effect from file");
            t.Name = filepath;
            fxList.Add(t);
        }

        /// <summary>Purge all loaded effects</summary>
        public void ClearAll()
        {
            fxList.Clear();
        }

    }

}
