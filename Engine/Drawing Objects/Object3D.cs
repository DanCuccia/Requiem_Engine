using System;
using System.Xml.Serialization;
using Engine.Drawing_Objects.Materials;
using Engine.Drawing_Objects.ParticleSystems;
using Engine.Game_Objects;
using Engine.Game_Objects.PreFabs;
using Engine.Managers;
using Engine.Managers.Camera;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Drawing_Objects
{
    /// <summary>This is the base object that all xml mediums inherit from,
    /// that way we can be generalize serialization from 1 call to all objects</summary>
    [Serializable]
    [XmlInclude(typeof(AnimatedObject3DXML))]
    [XmlInclude(typeof(StaticObject3DXML))]
    [XmlInclude(typeof(Line3DXML))]
    [XmlInclude(typeof(StaticObject3DXML))]
    [XmlInclude(typeof(TriggerXML))]
    [XmlInclude(typeof(PointLightXML))]
    [XmlInclude(typeof(QuadXml))]
    [XmlInclude(typeof(ParticleSystemXML))]
    [XmlInclude(typeof(PreFabricationXML))]
    [XmlInclude(typeof(PlatformXml))]
    [XmlInclude(typeof(EnemySpawnLocationXml))]
    public abstract class XMLMedium { }

    /// <summary>This Object3D abstract class is the base inheritence of all 3D renderables
    /// Renderer depends on the materials this object has in order to draw</summary>
    /// <author>Daniel Cuccia</author>
    public abstract class Object3D
    {
        /// <summary>world space location</summary>
        protected WorldMatrix           worldMatrix     = new WorldMatrix();
        /// <summary>drawing material</summary>
        protected Material              material        = null;
        /// <summary>whether to render twice into second depth buffer</summary>
        protected bool                  renderDepth     = false;
        /// <summary>drawing material used for depth only drawing</summary>
        protected Material              depthMaterial   = null;
        /// <summary>whether this object is collidable</summary>
        protected bool                  collidable      = false;
        /// <summary>oriented bounding box object</summary>
        protected OrientedBoundingBox   obb             = null;
        /// <summary>xna bounding sphere (for culling)</summary>
        protected BoundingSphere        boundingSphere;
        /// <summary>triangle count of drawable</summary>
        protected int                   triangleCount   = 0;
        /// <summary>reference to the main renderer</summary>
        protected Renderer              renderer        = null;
        /// <summary>whether this object is within the camera's frustum</summary>
        protected bool                  inFrustum       = true;
        /// <summary>collidable objects will generate a PhysX Actor</summary>
        public virtual StillDesign.PhysX.Actor Actor { set; get; }

        /// <summary>Default CTOR - initializes renderer pointer</summary>
        public Object3D()
        {
            this.renderer = Renderer.getInstance();
        }

        /// <summary>Extra destruction logic for PhysX Actor</summary>
        ~Object3D()
        {
            if (this.Actor != null)
            {
                this.Actor.Dispose();
                this.Actor = null;
            }
        }

        /// <summary>All 3D objects must update theirselves</summary>
        public abstract void Update(ref Camera camera, GameTime time);
        
        /// <summary>All 3D objects draw by queuing themselves within renderer's material batchs</summary>
        public abstract void Render();
        
        /// <summary>Renderer calls this to actually draw the objects to screen</summary>
        /// <param name="effect">the current material being drawn</param>
        public abstract void RenderImplicit(Effect effect);
        
        /// <summary>All 3D objects have an option to draw debugging information, such as bounding information</summary>
        public abstract void RenderDebug();
        
        /// <summary>All 3D objects generate their own obb differently</summary>
        public abstract void GenerateBoundingBox();

        /// <summary>Cull the object</summary>
        /// <param name="camera">what camera your testing if this object is within</param>
        protected void CullObject(ref Camera camera)
        {
            if(camera != null)
                inFrustum = camera.IsInFrustum(boundingSphere);
        }
        
        /// <summary>All 3D objects may have to update their obb differently</summary>
        public abstract void UpdateBoundingBox();

        /// <summary>Serialize the return from this for easy saving</summary>
        /// <returns>specific values that 3D drawing objects need to save</returns>
        public abstract XMLMedium GetXML();

        /// <summary>override and initialize member variables using the data from the xml medium input</summary>
        /// <param name="inputXml">all specific data needed to initialize a 3d drawing object</param>
        /// <param name="content">xna content manager</param>
        public abstract void CreateFromXML(ContentManager content, XMLMedium inputXml);

        /// <summary>Override this and return a copy of whatever drawable this is</summary>
        /// <returns>Copy of this drawable</returns>
        public abstract Object3D GetCopy(ContentManager content);

        /// <summary>World Matrix getter - all 3D position, rotation, scaling information found here</summary>
        public virtual WorldMatrix WorldMatrix
        {
            get { return this.worldMatrix; }
            set { this.worldMatrix = value; }
        }
        
        /// <summary>All 3D drawing objects have a material (shader) to draw with - will default to NullMaterials</summary>
        public virtual Material Material
        {
            get { return this.material; }
            set { this.material = value; }
        }

        /// <summary>All 3D drawing objects have the option to draw into the depth texture, used for evaluating all sorts of graphical stuff</summary>
        public virtual Material DepthMaterial
        {
            get { return this.depthMaterial; }
            set { this.depthMaterial = value; }
        }

        /// <summary>This will flag the drawable to render again into the depth texture</summary>
        public virtual bool RenderDepth
        {
            get { return this.renderDepth; }
            set { this.renderDepth = value; }
        }
        
        /// <summary>Oriented Bounding Box</summary>
        public virtual OrientedBoundingBox OBB
        {
            get { return this.obb; }
            set { this.obb = value; }
        }

        /// <summary>XNA's Bounding Sphere</summary>
        public virtual BoundingSphere BoundingSphere
        {
            get { return this.boundingSphere; }
            set { this.boundingSphere = value; }
        }
        /// <summary>Objects need to have the OBB for picking, therefore, we need this to say if we actually collide with our OBB too</summary>
        public virtual bool Collidable
        {
            get { return this.collidable; }
            set { this.collidable = value; }
        }

        /// <summary>This must be set by any inherited classes, How many Triangles this Object3D contains</summary>
        public virtual int TriangleCount
        {
            get { return this.triangleCount; }
            set { this.triangleCount = value; }
        }

        /// <summary>getter only - true if inside of frustum</summary>
        public bool InsideFrustum
        {
            get { return this.inFrustum; }
            set { this.inFrustum = value; }
        }
    }
}
