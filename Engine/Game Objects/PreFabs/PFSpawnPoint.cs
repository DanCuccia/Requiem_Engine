using Engine.Drawing_Objects;
using Engine.Drawing_Objects.ParticleSystems;
using Engine.Managers;
using Engine.Managers.Camera;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Engine.Game_Objects.PreFabs
{
    /// <summary>All visuals and interaction with a spawn point is in here</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class PFSpawnPoint : PreFabrication
    {
        StaticObject3D model = new StaticObject3D();
        TornadoGlitterEmitter tornado;
        
        /// <summary>Default CTOR</summary>
        public PFSpawnPoint(ContentManager content)
            : base(GameIDList.PreFab_SpawnPoint)
        {
            model.Initialize(content, "models//spawnPoint");
            this.WorldMatrix.OnChangeCallBack = onWorldMatrixChange;
            tornado = new TornadoGlitterEmitter(content);
        }

        /// <summary>callback when the worldMatrix changes</summary>
        private void onWorldMatrixChange()
        {
            model.WorldMatrix.X = this.WorldMatrix.X;
            model.WorldMatrix.Y = this.WorldMatrix.Y;
            model.WorldMatrix.Z = this.WorldMatrix.Z;
            model.WorldMatrix.rX = this.WorldMatrix.rX;
            model.WorldMatrix.rY = this.WorldMatrix.rY;
            model.WorldMatrix.rZ = this.WorldMatrix.rZ;
            model.WorldMatrix.sX = this.WorldMatrix.sX;
            model.WorldMatrix.sY = this.WorldMatrix.sY;
            model.WorldMatrix.sZ = this.WorldMatrix.sZ;
            tornado.WorldMatrix.X = this.WorldMatrix.X;
            tornado.WorldMatrix.Y = this.WorldMatrix.Y;
            tornado.WorldMatrix.Z = this.WorldMatrix.Z;
        }

        /// <summary>Major Update</summary>
        public override void Update(ref Camera camera, GameTime time)
        {
            model.Update(ref camera, time);
            tornado.Update(ref camera, time);
        }

        /// <summary>Major Render Call</summary>
        public override void Render()
        {
            model.Render();
            tornado.Render();
        }

        /// <summary>draw 3D debugging info</summary>
        public override void RenderDebug()
        {
            model.RenderDebug();
        }

        /// <summary>skip renderer's batch process and draw immediately</summary>
        public override void BatchSkipRender()
        {
            model.Material.InitializeShader();
            model.Material.PreRenderUpdate();
            model.Material.ApplyTechnique();
            model.RenderImplicit(model.Material.Effect);
        }

        /// <summary>Generate bounding data</summary>
        public override void GenerateBoundingBox()
        {
            model.GenerateBoundingBox();
            tornado.GenerateBoundingBox();
        }

        /// <summary>Update the bounding data's matrices</summary>
        public override void UpdateBoundingBox()
        {
            model.UpdateBoundingBox();
            tornado.UpdateBoundingBox();
        }

        /// <summary>make a copy of this object for the level editor</summary>
        public override Object3D GetCopy(ContentManager content)
        {
            PFSpawnPoint output = new PFSpawnPoint(content);
            output.WorldMatrix = this.WorldMatrix.Clone();
            return output;
        }

        /// <summary>rebuild the obect from xml</summary>
        /// <param name="content">xna content manager</param>
        /// <param name="inputXml">asset from xml</param>
        public override void CreateFromXML(ContentManager content, XMLMedium inputXml)
        {
            base.CreateFromXML(content, inputXml);
            this.onWorldMatrixChange();
        }

        /// <summary>the main associated OBB to this prefab</summary>
        public override OrientedBoundingBox OBB
        {
            get { return model.OBB; }
            set { model.OBB = value; }
        }
    }
}
