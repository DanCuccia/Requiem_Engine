using System;
using System.Collections.Generic;
using Engine.Drawing_Objects.Materials;
using Engine.Managers.Camera;
using Engine.Managers.Factories;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Drawing_Objects
{
    /// <summary>This is what StaticObject needs to save and load</summary>
    [Serializable]
    public class StaticObject3DXML : XMLMedium
    {
        /// <summary>model filepath</summary>
        public string filepath;
        /// <summary>world xml</summary>
        public WorldMatrixXml world;
        /// <summary>material xml</summary>
        public MaterialXML material;
        /// <summary>collidable bool</summary>
        public bool collidable;
        /// <summary>render to depth bool</summary>
        public bool rendersToDepth;
    }

    /// <summary>Non-Animating 3D drawing object</summary>
    /// <author>Daniel Cuccia</author>
    public class StaticObject3D : Object3D
    {
        #region Member Variables
        string filepath;
        /// <summary>main model object to be drawn</summary>
        protected Model         model           = null;
        Matrix[]                transforms      = null;
        float                   radius          = 10f;
        #endregion Member Variables

        #region Construction

        /// <summary>Default CTOR, all parameters initialized null or 0</summary>
        public StaticObject3D()
            :base() { }

        /// <summary>Load the model asset and assigns a nullMaterial</summary>
        /// <param name="modelFilepath">content filepath to requested model</param>
        /// <param name="content">xna content manager</param>
        public void Initialize(ContentManager content, string modelFilepath)
        {
            model = content.Load<Model>(modelFilepath);
            filepath = modelFilepath;
            transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
                foreach (ModelMeshPart part in mesh.MeshParts)
                    triangleCount += part.IndexBuffer.IndexCount;
            base.triangleCount /= 3;

            material = new NullMaterial(this);
        }

        #endregion Construction

        #region API

        /// <summary>Update logic -- updates the OBB if it exists</summary>
        /// <param name="time">current gaming time</param>
        /// <param name="camera">reference to camera used for culling</param>
        public override void Update(ref Camera camera, GameTime time)
        {
            updateBoundingSphere();
            
            base.CullObject(ref camera);
        }

        /// <summary>update the bounding sphere, accounting for any scaling / rotation changes</summary>
        private void updateBoundingSphere()
        {
            base.boundingSphere.Radius = (this.radius + 100f) * WorldMatrix.UniformScale;
            base.boundingSphere.Center.X = WorldMatrix.X;
            base.boundingSphere.Center.Y = WorldMatrix.Y;
            base.boundingSphere.Center.Z = WorldMatrix.Z;
        }

        /// <summary>Main Render to screen call</summary>
        public override void Render()
        {
            if (base.inFrustum)
            {
                renderer.AddToBatch(this, base.material.ID);
                if (base.depthMaterial != null)
                    renderer.AddToDepthBatch(this, base.depthMaterial.ID);
            }
        }

        /// <summary>This is call which renderer makes to actually draw the 3D model</summary>
        public override void RenderImplicit(Effect effect)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                }
                effect.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index] *
                    this.WorldMatrix.GetWorldMatrix());
                mesh.Draw();
            }
        }

        /// <summary>Draws the OrientedBoundingBox of this Object to the screen</summary>
        public override void RenderDebug()
        {
            if (OBB != null && base.inFrustum == true)
                 OBB.Render();
        }

        /// <summary>Generate the Oriented Bounding Box, OBB will be null until this is called</summary>
        public override void GenerateBoundingBox()
        {
            if (model == null)
                throw new ArgumentNullException("Object3D::GenerateBoundingBox:model is null" +
                        "cannot generate bounding box if the object is not initialized");

            Vector3 modelMin = new Vector3(float.MaxValue);
            Vector3 modelMax = new Vector3(float.MinValue);

            int stride = -1;
            stride = model.Meshes[0].MeshParts[0].VertexBuffer.VertexDeclaration.VertexStride;

            List<OrientedBoundingBox> output = new List<OrientedBoundingBox>();

            if (stride == -1)
                throw new Exception("StaticObject3D::GenerateBoundingBox error retrieving vertex stride");

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    int numberOfVertices = part.NumVertices;
                    Vector3[] posArray = null;

                    switch (stride)
                    {
                        case 56://position, normal, texture, tangent, binormal
                            VertexPositionNormalTextureTangentBinormal[] vertices = new VertexPositionNormalTextureTangentBinormal[numberOfVertices];
                            part.VertexBuffer.GetData<VertexPositionNormalTextureTangentBinormal>(vertices);
                            posArray = MyMath.GenerateVertexPositionArray(vertices);
                            Matrix boneMat = transforms[mesh.ParentBone.Index];
                            Vector3.Transform(posArray, ref boneMat, posArray);
                            break;

                        case 32://position, normal, texture
                            VertexPositionNormalTexture[] vertices2 = new VertexPositionNormalTexture[numberOfVertices];
                            part.VertexBuffer.GetData<VertexPositionNormalTexture>(vertices2);
                            posArray = MyMath.GenerateVertexPositionArray(vertices2);
                            Matrix boneMat2 = transforms[mesh.ParentBone.Index];
                            Vector3.Transform(posArray, ref boneMat2, posArray);
                            break;

                        case 12://position, normal
                            VertexPositionNormal[] vertices3 = new VertexPositionNormal[numberOfVertices];
                            part.VertexBuffer.GetData<VertexPositionNormal>(vertices3);
                            posArray = MyMath.GenerateVertexPositionArray(vertices3);
                            Matrix boneMat3 = transforms[mesh.ParentBone.Index];
                            Vector3.Transform(posArray, ref boneMat3, posArray);
                            break;
                    }

                    foreach (Vector3 vPos in posArray)
                    {
                        modelMin = Vector3.Min(vPos, modelMin);
                        modelMax = Vector3.Max(vPos, modelMax);
                    }
                }
            }

            OBB = new OrientedBoundingBox(modelMin, modelMax);

            float t, r;
            t = r = (modelMax.X - modelMin.X) * 0.5f;
            t = (modelMax.Y - modelMin.Y) * 0.5f;
            if (t > r) r = t;
            t = (modelMax.Z - modelMin.Z) * 0.5f;
            if (t > r) r = t;
            BoundingSphere = new BoundingSphere(WorldMatrix.Position, Math.Abs(r));
            this.radius = r;
        }

        /// <summary>Update the OBB's world matrix, to match this model's world matrix,</summary>
        public override void  UpdateBoundingBox()
        {
            if (OBB != null)
                OBB.Update(this.worldMatrix.GetWorldMatrix());
        }

        /// <summary>Get the XmlMedium class that holds all data needed to save and load</summary>
        /// <returns>serializable structure with all related values</returns>
        public override XMLMedium GetXML()
        {
            StaticObject3DXML output = new StaticObject3DXML();
            output.filepath = this.filepath;
            output.material = this.material.GetXml();
            output.world = this.worldMatrix.GetXml();
            output.collidable = this.collidable;
            output.rendersToDepth = this.renderDepth;
            return output;
        }

        /// <summary>initialize all member variables according the input xml medium</summary>
        /// <param name="inputXml">all needed values to save and load this object</param>
        /// <param name="content">xna content manager</param>
        public override void CreateFromXML(ContentManager content, XMLMedium inputXml)
        {
            StaticObject3DXML input = inputXml as StaticObject3DXML;
            this.Initialize(content, input.filepath);
            this.worldMatrix.FromXML(input.world);
            this.collidable = input.collidable;
            this.RenderDepth = input.rendersToDepth;
            MaterialBinder.getInstance().BindMaterial(this, (inputXml as StaticObject3DXML).material);
        }

        /// <summary> Get a copy of this static 3D object </summary>
        /// <returns>exact copy of this drawable, with correct references</returns>
        public override Object3D GetCopy(ContentManager content)
        {
            StaticObject3D output = new StaticObject3D();
            output.Initialize(content, this.filepath);
            output.Material = this.Material.CopyAndAttach(output);
            output.WorldMatrix = this.WorldMatrix.Clone();
            return output;
        }

        /// <summary>override RenderDepth bool to create a Depth Material or dispose it</summary>
        public override bool RenderDepth
        {
            get { return base.renderDepth; }
            set
            {
                base.renderDepth = value;
                if (value == true)
                {
                    base.depthMaterial = new DepthMaterial(this);
                }
                else
                {
                    base.depthMaterial = null;
                }
            }
        }

        #endregion API
  
    }
}
