using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Engine.Drawing_Objects;
using Engine.Game_Objects.PreFabs;

namespace Engine.Managers.Factories
{
    /// <summary>This uncreatable object acts as the pre fab factory</summary>
    public abstract class PreFabFactory
    {
        private PreFabFactory() { }

        /// <summary>Main API to create a PreFab from XML</summary>
        /// <param name="content">xna's content manager</param>
        /// <param name="asset">the asset created from xml</param>
        /// <returns>fully constructed and ready prefab</returns>
        public static Object3D GetPreFabrication(ContentManager content, XMLMedium asset)
        {
            PreFabricationXML input = asset as PreFabricationXML;

            Object3D output = null;

            switch (input.id)
            {
                case GameIDList.PreFab_SpawnPoint:
                    output = new PFSpawnPoint(content); break;
            }

            if (output != null)
            {
                output.CreateFromXML(content, asset);
            }

            return output;
        }
    }
}
