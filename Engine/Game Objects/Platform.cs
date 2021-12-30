using System;
using Engine.Drawing_Objects;
using Engine.Game_Objects.PlatformBehaviors;
using Engine.Managers.Camera;
using Engine.Managers.Factories;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StillDesign.PhysX;

namespace Engine.Game_Objects
{

    /// <summary>serialized platform medium</summary>
    /// <author>Daniel Cuccia</author>
    [Serializable]
    public sealed class PlatformXml : XMLMedium
    {
        /// <summary>world xml object</summary>
        public StaticObject3DXML modelXml;
        /// <summary>settings to the behavior</summary>
        public PlatformBehaviorXML behaviorXml;
    }

    /// <summary>Base class for all Platforms to derive from</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class Platform : Object3D
    {
        #region Member Vars

        /// <summary>the main drawable</summary>
        StaticObject3D model;
        /// <summary>the behavior of this platform</summary>
        public IPlatform behavior;

        #endregion

        #region Init

        /// <summary>Default CTOR</summary>
        public Platform()
        {
            this.Behavior = new IStationaryPlatform(this);
            this.model = new StaticObject3D();
        }

        /// <summary>Destructor needed here to properly remove the actor</summary>
        ~Platform()
        {
            if(this.Actor != null)
            {
                this.Actor.Dispose();
            }
        }

        /// <summary>initialize assets</summary>
        /// <param name="content">content manager</param>
        /// <param name="filename">model filepath</param>
        public void Initialize(ContentManager content, string filename)
        {
            this.model.Initialize(content, filename);
            this.Collidable = true;
            this.model.GenerateBoundingBox();
            this.model.UpdateBoundingBox();
            this.Actor = PhysXPrimitive.GetActor(model.OBB, model.WorldMatrix);
        }

        /// <summary>Generate the bounding box</summary>
        /// <remarks>unused - platforms get initialized with bounding information</remarks>
        public override sealed void GenerateBoundingBox() { }

        #endregion

        #region Run-Time
        /// <summary>main update call</summary>
        /// <param name="camera">camera reference used for culling</param>
        /// <param name="time">game time</param>
        public override sealed void Update(ref Camera camera, GameTime time)
        {
            Behavior.Update(ref camera, time);
            if(model != null)
                model.Update(ref camera, time);
        }

        /// <summary>update the bounding box</summary>
        public override sealed void UpdateBoundingBox()
        {
            if (model.OBB != null)
                model.OBB.Update(this.WorldMatrix.GetWorldMatrix());
            if (Actor != null)
            {
                StillDesign.PhysX.MathPrimitives.Vector3 v = Actor.Shapes[0].LocalPosition;
                v.X = WorldMatrix.X; v.Y = WorldMatrix.Y - ((this.OBB.Max.Y - this.OBB.Min.Y)*.5f); v.Z = WorldMatrix.Z;
                Actor.Shapes[0].LocalPosition = v;
            }
        }

        /// <summary>queue to render</summary>
        public override sealed void Render()
        {
            model.Render();
        }

        /// <summary>render debugging data</summary>
        public override sealed void RenderDebug()
        {
            if (model.OBB != null)
                model.OBB.Render();
        }

        /// <summary>unused</summary>
        /// <param name="effect">unused</param>
        public override sealed void RenderImplicit(Effect effect) 
        {
            this.model.RenderImplicit(effect);
        }

        #endregion

        #region Editing

        /// <summary>create a duplicate of this object</summary>
        /// <param name="content">content manager</param>
        /// <returns>drawable object</returns>
        public override sealed Object3D GetCopy(ContentManager content)
        {
            Platform output = new Platform();
            output.Initialize(content, (this.model.GetXML() as StaticObject3DXML).filepath);
            output.WorldMatrix = this.WorldMatrix.Clone();
            output.Material = this.Material.CopyAndAttach(output);
            return output;
        }

        /// <summary>Get the XML object </summary>
        /// <returns>serializable object</returns>
        public override sealed XMLMedium GetXML()
        {
            PlatformXml output = new PlatformXml();
            output.modelXml = this.model.GetXML() as StaticObject3DXML;
            output.behaviorXml = this.Behavior.GetBehaviorXml();
            return output;
        }

        /// <summary>create this item using xml parameters</summary>
        /// <param name="content">content manger</param>
        /// <param name="inputXml">object from xml</param>
        public override sealed void CreateFromXML(ContentManager content, XMLMedium inputXml)
        {
            PlatformXml input = inputXml as PlatformXml;

            this.Initialize(content, input.modelXml.filepath);
            MaterialBinder.getInstance().BindMaterial(model, input.modelXml.material);
            this.WorldMatrix.FromXML(input.modelXml.world);
            this.Behavior = PlatformBehaviorFactory.GetPlatformBehavior(input.behaviorXml, this);
            this.RenderDepth = input.modelXml.rendersToDepth;
            this.collidable = true;
        }

        #endregion

        #region Mutators and Virtual Overrides
        /// <summary>Platform Behavior logic</summary>
        public IPlatform Behavior
        {
            get { return this.behavior; }
            set { this.behavior = value; }
        }
        /// <summary>World Matrix virtual override</summary>
        public override sealed WorldMatrix WorldMatrix
        {
            get { return model.WorldMatrix; }
            set { model.WorldMatrix = value; }
        }
        /// <summary>Material virtual override</summary>
        public override sealed Engine.Drawing_Objects.Materials.Material Material
        {
            get { return model.Material; }
            set { model.Material = value; }
        }
        /// <summary>DepthMaterial virtual override</summary>
        public override sealed Engine.Drawing_Objects.Materials.Material DepthMaterial
        {
            get { return model.DepthMaterial; }
            set { model.DepthMaterial = value; }
        }
        /// <summary>BoundingSphere virtual override</summary>
        public override sealed BoundingSphere BoundingSphere
        {
            get { return model.BoundingSphere; }
            set { model.BoundingSphere = value; }
        }
        /// <summary>RenderDepth virtual override</summary>
        public override sealed bool RenderDepth
        {
            get { return model.RenderDepth; }
            set { model.RenderDepth = value; }
        }
        /// <summary>TriangleCount virtual override</summary>
        public override sealed int TriangleCount
        {
            get { return model.TriangleCount; }
            set { model.TriangleCount = value; }
        }
        /// <summary>Collidable virtual override</summary>
        public override sealed bool Collidable
        {
            get { return model.Collidable; }
            set { model.Collidable = value; }
        }
        /// <summary>OrientedBoundingBox virtual override</summary>
        public override sealed OrientedBoundingBox OBB
        {
            get { return model.OBB; }
            set { model.OBB = value; }
        }
        #endregion
    }
}
