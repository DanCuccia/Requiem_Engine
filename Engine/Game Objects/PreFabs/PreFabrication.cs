using System;
using Engine.Drawing_Objects;
using Engine.Math_Physics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Game_Objects.PreFabs
{
    /// <summary>all prefabs use this object to save and load</summary>
    [Serializable]
    public class PreFabricationXML : XMLMedium
    {
        /// <summary>identification</summary>
        public int id;
        /// <summary>world xml object</summary>
        public WorldMatrixXml worldMatrix;
        /// <summary>whether this is collidable or not</summary>
        public bool collidable;
    }

    /// <summary>The base class which all prefabrications enherit from</summary>
    /// <author>Daniel Cuccia</author>
    public abstract class PreFabrication : Object3D
    {
        /// <summary>identification</summary>
        protected int id = -1;

        /// <summary>Default CTOR</summary>
        /// <param name="id">pre fabrication id</param>
        public PreFabrication(int id)
            :base()
        {
            this.id = id;
            this.Material = null;
            this.Collidable = true;
        }

        /// <summary>Unused - must be overriden by dericed class</summary>
        public override void RenderDebug() { }
        /// <summary>Unused - must be overriden by dericed class</summary>
        public override void RenderImplicit(Effect effect) { }

        /// <summary>overriden in base class, all derived prefabs will save the same way</summary>
        public override XMLMedium GetXML()
        {
            PreFabricationXML output = new PreFabricationXML();
            output.id = this.id;
            output.worldMatrix = this.WorldMatrix.GetXml();
            output.collidable = this.Collidable;
            return output;
        }

        /// <summary>overriden in base class, all derived prefabs will construct themselves besides worldMatrix</summary>
        public override void CreateFromXML(ContentManager content, XMLMedium inputXml)
        {
            PreFabricationXML input = inputXml as PreFabricationXML;
            this.WorldMatrix.FromXML(input.worldMatrix);
            this.Collidable = input.collidable;
            this.id = input.id;
        }

        /// <summary>This is used for the level editor's ThumbnailRenderer</summary>
        /// <remarks>completely skips renderer's batch process and draws immediately to screen</remarks>
        public abstract void BatchSkipRender();
    }
}
