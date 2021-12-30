using System;
using Engine.Drawing_Objects.Materials;
using Engine.Managers.Camera;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Drawing_Objects
{
    /// <summary>All data needed to save this point light to xml</summary>
    [Serializable]
    public class PointLightXML : XMLMedium
    {
        /// <summary>toggle this to render the bulb model</summary>
        public bool showBulb;
        /// <summary>world matrix xml object</summary>
        public WorldMatrixXml world;
        /// <summary>color of this light</summary>
        public Vector4 color;
        /// <summary>This is used as a distance value, overall lighting multiplier</summary>
        public float intensity;
        /// <summary>falloff value of this light</summary>
        public float falloff;
    }

    /// <summary>This is the PointLight class that I use within our own shader architecture</summary>
    /// <author>Daniel Cuccia</author>
    public class PointLight : Object3D
    {
        /// <summary>the actual model of this light</summary>
        public StaticObject3D   bulb            = null;
        /// <summary>toggle this to render the bulb model</summary>
        public bool             showBulb        = true;
        /// <summary>color of this light</summary>
        public Vector4          color           = Vector4.One;
        /// <summary>This is used as a distance value, overall lighting multiplier</summary>
        public float            intensity       = 2f;
        /// <summary>falloff value of this light</summary>
        public float            falloff         = 200f;
        
        /// <summary>min intensity</summary>
        public const float Min_Intensity = 0.5f;
        /// <summary>max intensity</summary>
        public const float Max_Intensity = 4f;
        /// <summary>min falloff</summary>
        public const float Min_FallOff = 50f;
        /// <summary>max falloff</summary>
        public const float Max_FallOff = 1000f;

        /// <summary>Default CTOR</summary>
        public PointLight()
        {        }

        /// <summary>Initialize all assets and member variables</summary>
        public void Initialize(ContentManager content)
        {
            bulb = new StaticObject3D();
            bulb.Initialize(content, "models//lightbulb");
            bulb.Material = new NullMaterial(this);
            bulb.Material.Ambient = 1f;

            base.boundingSphere.Center.X = WorldMatrix.X;
            base.boundingSphere.Center.Y = WorldMatrix.Y;
            base.boundingSphere.Center.Z = WorldMatrix.Z;
            base.boundingSphere.Radius = this.falloff * 2f;
        }

        /// <summary>major update logic</summary>
        /// <param name="camera">camera reference for culling</param>
        /// <param name="time">current timing values</param>
        public override void Update(ref Camera camera, GameTime time)
        {
            bulb.WorldMatrix.Position = this.worldMatrix.Position;
            bulb.WorldMatrix.Rotation = this.worldMatrix.Rotation;
            bulb.WorldMatrix.Scale = this.worldMatrix.Scale;
            bulb.Update(ref camera, time);

            if (bulb.Material.GetType() == typeof(NullMaterial))
            {
                ((NullMaterial)bulb.Material).color = this.color;
            }
        }

        /// <summary>Queue this light into the proper material batch</summary>
        public override void Render()
        {
            if (EngineFlags.forceBulbs == true)
            {
                bulb.Render();
                return;
            }

            if(showBulb ==  true)
                bulb.Render();
        }

        /// <summary>Draw any debugging information used for this light</summary>
        public override void RenderDebug()
        {
            if(bulb.OBB != null)
                bulb.OBB.Render();
        }

        /// <summary>Draws this light to screen (lightStreak)</summary>
        public override void RenderImplicit(Effect effect) { }

        /// <summary>Generate bounding box (only used for level editing)</summary>
        public override void GenerateBoundingBox()
        {
            bulb.GenerateBoundingBox();
        }

        /// <summary>Update bounding information (only used for level editing</summary>
        public override void UpdateBoundingBox()
        {
            bulb.WorldMatrix.Position = this.worldMatrix.Position;
            bulb.WorldMatrix.Rotation = this.worldMatrix.Rotation;
            bulb.WorldMatrix.Scale = this.worldMatrix.Scale;
            bulb.UpdateBoundingBox();
        }

        /// <summary>Get the structure used to save this light's information</summary>
        /// <returns>all relavant data needed for this light</returns>
        public override XMLMedium GetXML()
        {
            PointLightXML output = new PointLightXML();
            output.world = this.worldMatrix.GetXml();
            output.showBulb = this.showBulb;
            output.color = this.color;
            output.intensity = this.intensity;
            output.falloff = this.falloff;
            return output;
        }

        /// <summary>Recreate this light from a deserialized light structure</summary>
        /// <param name="content">xna content manager</param>
        /// <param name="inputXml">data needed to recreate this light</param>
        public override void CreateFromXML(ContentManager content, XMLMedium inputXml)
        {
            PointLightXML input = (PointLightXML)inputXml;
            this.Initialize(content);
            this.showBulb = input.showBulb;
            this.color = input.color;
            this.worldMatrix.FromXML(input.world);
            this.intensity = input.intensity;
            this.falloff = input.falloff;
            this.material = new NullMaterial(this);
        }

        /// <summary>used in level editor to copy this object</summary>
        /// <param name="content">xna content manager</param>
        /// <returns>a copy of this object</returns>
        public override Object3D GetCopy(ContentManager content)
        {
            PointLight output = new PointLight();
            output.Initialize(content);
            output.showBulb = this.showBulb;
            output.color = this.color;
            output.falloff = this.falloff;
            output.intensity = this.intensity;
            output.Material = this.Material.CopyAndAttach(output);
            output.WorldMatrix = this.WorldMatrix.Clone();
            return output;
        }

        /// <summary>Overriden to return the bulb's obb instead of (this)</summary>
        public override OrientedBoundingBox OBB
        {
            get { return bulb.OBB; }
            set { bulb.OBB = value; }
        }

        /// <summary>Overriden to return the bulb's bounding sphere instead of (this)</summary>
        public override BoundingSphere BoundingSphere
        {
            get { return bulb.BoundingSphere; }
            set { bulb.BoundingSphere = value; }
        }

        /// <summary> Overriden to return the bulb's material </summary>
        public override Material Material
        {
            get { return this.bulb.Material; }
            set { this.bulb.Material = value; }
        }

        /// <summary>Override triangle count to this PointLight's bulb</summary>
        public override int TriangleCount
        {
            get { return bulb.TriangleCount; }
            set { bulb.TriangleCount = value; }
        }
    }
}
