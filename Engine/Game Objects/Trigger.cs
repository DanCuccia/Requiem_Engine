using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using Engine.Drawing_Objects;
using Engine.Drawing_Objects.Materials;
using Engine.Managers;
using Engine.Math_Physics;

namespace Engine.Game_Objects
{
    /// <summary>These are the values that are stored in xml</summary>
    [Serializable]
    public class TriggerXML : XMLMedium
    {
        /// <summary>world matrix xml obj</summary>
        public WorldMatrixXml   worldMatrix;
        /// <summary>material xml obj</summary>
        public MaterialXML      material;
        /// <summary>trigger callback id</summary>
        public int              id;
        /// <summary>repeateable component</summary>
        public bool             repeatable;
        /// <summary>has been called component</summary>
        public bool             hasTriggered;
    }

    /// <summary>This is the main trigger which when collided calls the callback</summary>
    /// <author>Daniel Cuccia</author>
    public class Trigger : StaticObject3D
    {
        int id = -1;

        bool hasTriggered = false;
        bool repeatable = false;

        /// <summary>onCollision callback</summary>
        public delegate void CallBack();
        /// <summary>collision callback</summary>
        public CallBack triggerCallback = null;

        /// <summary>Default CTOR</summary>
        public Trigger()
            :base()
        {    }

        /// <summary>Initialize assets</summary>
        public void Initialize(ContentManager content)
        {
            base.Initialize(content, "models//boxn");
            base.collidable = true;
            base.Material = new DiffuseMaterial(this, TextureManager.getInstance().GetTexture("textures//trigger_diffuse"));
        }

        /// <summary>Render the trigger to the screen</summary>
        public override void Render()
        {
            base.Render();
            base.RenderDebug();
        }

        /// <summary>Execute the trigger's callback</summary>
        public void Execute()
        {
            if (hasTriggered == false && triggerCallback != null)
            {
                triggerCallback();
                if (repeatable == false)
                    hasTriggered = true;
            }
        }

        /// <summary>Get the XML structure containing all information needed save and load this object</summary>
        /// <returns>a serializable class containing all data needed for this object</returns>
        public override XMLMedium GetXML()
        {
            TriggerXML output = new TriggerXML();
            output.id = this.id;
            output.hasTriggered = this.hasTriggered;
            output.repeatable = this.repeatable;
            output.worldMatrix = this.worldMatrix.GetXml();
            output.material = material.GetXml();
            return output;
        }

        /// <summary>Load all member variables from the input parameters</summary>
        /// <param name="inputXml">all information needed to load this object is found here</param>
        /// <param name="content">xna content manager</param>
        public override void CreateFromXML(ContentManager content, XMLMedium inputXml)
        {
            TriggerXML input = (TriggerXML)inputXml;
            this.Initialize(content);

            id = input.id;
            hasTriggered = input.hasTriggered;
            repeatable = input.repeatable;
            worldMatrix.FromXML(input.worldMatrix);
        }

        /// <summary> Triggers are not allowed to be copied, this will return null </summary>
        public override Object3D GetCopy(ContentManager content)
        {
            return null;
        }
        /// <summary>callback ID</summary>
        public int ID
        {
            get { return this.id; }
            set { this.id = value; }
        }
        /// <summary>has triggered component</summary>
        public bool HasTriggered
        {
            get { return this.hasTriggered; }
            set { this.hasTriggered = value; }
        }
        /// <summary>is this repeatable yes/no</summary>
        public bool Repeatable
        {
            get { return this.repeatable; }
            set { this.repeatable = value; }
        }
    }
}
