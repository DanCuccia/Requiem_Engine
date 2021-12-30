using Engine.Managers.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine.Drawing_Objects.Materials;
using Engine.Math_Physics;
#pragma warning disable 1591

namespace Engine.Drawing_Objects
{
    /// <summary>This sky-sphere will render animated clouds</summary>
    public sealed class AnimatedClouds3D : Object3D
    {
        StaticObject3D sphere;

        /// <summary>default ctor</summary>
        public AnimatedClouds3D()
            : base()
        { }

        /// <summary>init member vars</summary>
        /// <param name="content">xna content manager</param>
        public void Initialize(ContentManager content)
        {
            sphere = new StaticObject3D();
            sphere.Initialize(content, "particle//megaSphere");
            sphere.Material = new AnimatedCloudMaterial(this);
        }

        /// <summary>update</summary>
        public override void Update(ref Camera camera, GameTime time) 
        {
            sphere.InsideFrustum = true;
        }

        /// <summary>draw by queuing themselves within renderer's material batchs</summary>
        public override void Render()
        {
            if (sphere != null)
                sphere.Render();
        }

        #region overrides
        public override Material Material
        {
            get { return sphere.Material; }
            set { sphere.Material = value; }
        }
        public override Material DepthMaterial
        {
            get { return sphere.DepthMaterial; }
            set { sphere.DepthMaterial = value; }
        }
        public override BoundingSphere BoundingSphere
        {
            get { return base.BoundingSphere; }
            set { base.BoundingSphere = value; }
        }
        public override bool RenderDepth
        {
            get { return sphere.RenderDepth; }
            set { sphere.RenderDepth = value; }
        }
        public override WorldMatrix WorldMatrix
        {
            get { return sphere.WorldMatrix; }
            set { sphere.WorldMatrix = value; }
        }
        #endregion

        #region unused
        /// <summary>generate obb</summary>
        public override void GenerateBoundingBox() { }
        /// <summary>update obb</summary>
        public override void UpdateBoundingBox() { }
        /// <summary>Serialize the return from this for easy saving</summary>
        /// <returns>specific values</returns>
        public override XMLMedium GetXML() { return null; }
        /// <summary>override and initialize member variables using the data from the xml medium input</summary>
        /// <param name="inputXml">all specific data needed to initialize a 3d drawing object</param>
        /// <param name="content">xna content manager</param>
        public override void CreateFromXML(ContentManager content, XMLMedium inputXml) { }
        /// <summary>Override this and return a copy of whatever drawable this is</summary>
        /// <returns>Copy of this drawable</returns>
        public override Object3D GetCopy(ContentManager content) { return null; }
        /// <summary>Renderer calls this to actually draw the objects to screen</summary>
        /// <param name="effect">the current material being drawn</param>
        public override void RenderImplicit(Effect effect) { }
        /// <summary>draw debugging information</summary>
        public override void RenderDebug() { }
        #endregion
    }
}
