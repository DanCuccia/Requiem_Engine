using System;
using Engine.Drawing_Objects.Materials;
using Engine.Managers;
using Engine.Managers.Camera;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#pragma warning disable 1591

namespace Engine.Drawing_Objects
{
    /// <summary>serializable class required to replicate this quad</summary>
    [Serializable]
    public class QuadXml : XMLMedium
    {
        public string textureFilepath = "";
        public WorldMatrixXml worldMatrix = null;
        public int origin = 0;
    }

    /// <summary>enumeration depicting where the center of this billboard is</summary>
    public enum Quad3DOrigin
    {
        CENTER,
        BOTTOM_CENTER,
        TOP_LEFT
    }

    /// <summary>This is a QuadBillboard in 3D space</summary>
    /// <author>Daniel Cuccia</author>
    public class Quad3D : Object3D
    {
        #region Member Variables

        GraphicsDevice                  device;
        Color                           color       = Color.White;

        Quad3DOrigin                    origin      = Quad3DOrigin.CENTER;
        VertexPositionColorTexture[]    vertices    = new VertexPositionColorTexture[4];
        int[]                           indices     = { 1, 0, 3, 2 };

        float                           width       = 50f;
        float                           height      = 75f;

        #endregion Member Variables

        #region Init

        /// <summary>Default CTOR</summary>
        public Quad3D(Quad3DOrigin quadOrigin = Quad3DOrigin.CENTER)
            : base()
        {
            this.origin = quadOrigin;
            device = Renderer.getInstance().Device;
        }

        /// <summary>Load assets, assign the world matrix callback</summary>
        /// <param name="texture">the texture to be drawn on this billboard (can be null, will use members: width / height</param>
        public void Initialize(Texture2D texture)
        {
            if (texture == null)
                texture = TextureManager.getInstance().GetTexture("error");

            base.Material = new TexturedQuadMaterial(this, texture);
            this.width = texture.Width;
            this.height = texture.Height;

            updateBoundingSphere();

            this.initializeVertices();
        }

        //public override Material Material
        //{
        //    get { return base.Material; }
        //    set
        //    {
        //        base.Material = value;
        //        Texture2D tex = (value as TexturedQuadMaterial).Texture;
        //        this.width = tex.Width;
        //        this.height = tex.Height;

        //        base.boundingSphere.Center.X = WorldMatrix.X;
        //        base.boundingSphere.Center.Y = WorldMatrix.Y + (height * .5f);
        //        base.boundingSphere.Center.Z = WorldMatrix.Z;
        //        base.boundingSphere.Radius = width > height ? width : height;

        //        this.initializeVertices();
        //    }
        //}

        /// <summary>initialize the vertices</summary>
        private void initializeVertices()
        {
            switch (origin)
            {
                case Quad3DOrigin.BOTTOM_CENTER:
                    vertices[0] = new VertexPositionColorTexture(new Vector3(-width / 2, 0, 0), this.color, new Vector2(0, 1));
                    vertices[1] = new VertexPositionColorTexture(new Vector3(width / 2, 0, 0), this.color, new Vector2(1, 1));
                    vertices[2] = new VertexPositionColorTexture(new Vector3(-width / 2, height, 0), this.color, new Vector2(0, 0));
                    vertices[3] = new VertexPositionColorTexture(new Vector3(width / 2, height, 0), this.color, new Vector2(1, 0));
                    break;

                case Quad3DOrigin.CENTER:
                    vertices[0] = new VertexPositionColorTexture(new Vector3(-width / 2, -height / 2, 0), this.color, new Vector2(0, 1));
                    vertices[1] = new VertexPositionColorTexture(new Vector3(width / 2, -height / 2, 0), this.color, new Vector2(1, 1));
                    vertices[2] = new VertexPositionColorTexture(new Vector3(-width / 2, height / 2, 0), this.color, new Vector2(0, 0));
                    vertices[3] = new VertexPositionColorTexture(new Vector3(width / 2, height / 2, 0), this.color, new Vector2(1, 0));
                    break;

                case Quad3DOrigin.TOP_LEFT:
                    vertices[0] = new VertexPositionColorTexture(new Vector3(0, -height, 0), this.color, new Vector2(0, 1));
                    vertices[1] = new VertexPositionColorTexture(new Vector3(width, -height, 0), this.color, new Vector2(1, 1));
                    vertices[2] = new VertexPositionColorTexture(new Vector3(0, 0, 0), this.color, new Vector2(0, 0));
                    vertices[3] = new VertexPositionColorTexture(new Vector3(width, 0, 0), this.color, new Vector2(1, 0));
                    break;
            }
            
        }

        #endregion Init

        #region API

        /// <summary>main update logic</summary>
        /// <param name="camera">camera used for culling</param>
        /// <param name="time">current time values</param>
        public override void Update(ref Camera camera, GameTime time)
        {
            updateBoundingSphere();
            base.CullObject(ref camera);
        }

        /// <summary>updates the frustum culling bounding sphere depending on quad origin</summary>
        private void updateBoundingSphere()
        {
            switch (this.origin)
            {
                case Quad3DOrigin.BOTTOM_CENTER:
                    base.boundingSphere.Center.X = WorldMatrix.X;
                    base.boundingSphere.Center.Y = WorldMatrix.Y + (height * .5f);
                    base.boundingSphere.Center.Z = WorldMatrix.Z;
                    break;

                case Quad3DOrigin.CENTER:
                    base.boundingSphere.Center.X = WorldMatrix.X;
                    base.boundingSphere.Center.Y = WorldMatrix.Y;
                    base.boundingSphere.Center.Z = WorldMatrix.Z;
                    break;

                case Quad3DOrigin.TOP_LEFT:
                    base.boundingSphere.Center.X = WorldMatrix.X + (width * 0.5f);
                    base.boundingSphere.Center.Y = WorldMatrix.Y - (height * 0.5f);
                    base.boundingSphere.Center.Z = WorldMatrix.Z;
                    break;
            }
            base.boundingSphere.Radius = width > height ? width * WorldMatrix.UniformScale * .5f : height * WorldMatrix.UniformScale * .5f;
        }

        /// <summary>Add this drawable to Renderer's material batch</summary>
        public override void Render()
        {
            if(base.inFrustum)
                renderer.AddToBatch(this, material.ID);
        }

        /// <summary>Draw this billboards OBB if instantiated</summary>
        public override void RenderDebug() 
        {
            if(this.OBB != null && base.inFrustum == true)
                OBB.Render();
        }

        /// <summary>Draw the verts</summary>
        /// <param name="effect">the currently applied effect</param>
        public override void RenderImplicit(Effect effect)
        {
            device.DrawUserIndexedPrimitives<VertexPositionColorTexture>(
                PrimitiveType.TriangleStrip, vertices, 0, 4, indices, 0, 2);
        }

        /// <summary>Get the serializable class</summary>
        /// <returns>serializable class to replicate this quad</returns>
        public override XMLMedium GetXML()
        {
            QuadXml output = new QuadXml();
            output.worldMatrix = this.worldMatrix.GetXml();
            output.origin = (int)this.origin;
            output.textureFilepath = TextureManager.getInstance().GetFilepath((Material as TexturedQuadMaterial).Texture);
            return output;
        }

        /// <summary>renew this quad from xml</summary>
        /// <param name="inputXml">deserialized quad from xml</param>
        /// <param name="content">xna content manager</param>
        public override void CreateFromXML(ContentManager content, XMLMedium inputXml)
        {
            QuadXml input = (QuadXml)inputXml;
            this.Initialize(TextureManager.getInstance().GetTexture(input.textureFilepath));
            this.worldMatrix.FromXML(input.worldMatrix);
        }

        /// <summary>Get a replication of this quad</summary>
        /// <returns>an exact copy of this quad, not referencing each other except for texture</returns>
        public override Object3D GetCopy(ContentManager content)
        {
            Quad3D quad = new Quad3D(this.origin);
            quad.Initialize((material as TexturedQuadMaterial).Texture);
            quad.Color = this.Color;
            quad.WorldMatrix = this.worldMatrix.Clone();
            return quad;
        }

        /// <summary>Generate OBB accordingly to origin</summary>
        public override void GenerateBoundingBox() 
        {
            switch(origin)
            {
                case Quad3DOrigin.BOTTOM_CENTER:
                    this.OBB = new OrientedBoundingBox(
                        new Vector3(-width / 2, 0, -1f),
                        new Vector3(width / 2, height, 1f));
                    break;

                case Quad3DOrigin.CENTER:
                    this.OBB = new OrientedBoundingBox(
                        new Vector3(-width / 2, -height / 2, -1f),
                        new Vector3(width / 2, height / 2, 1f));
                    break;

                case Quad3DOrigin.TOP_LEFT:
                    this.OBB = new OrientedBoundingBox(
                        new Vector3(0, 0, 1f),
                        new Vector3(width, -height, -1f));
                    break;
            }
        }

        /// <summary>Update the OBB's matrix</summary>
        public override void UpdateBoundingBox() 
        {
            if(this.OBB != null)
                OBB.Update(this.WorldMatrix.GetWorldMatrix());

        }

        #endregion API


        /// <summary>The color assigned to vertices</summary>
        public Color Color
        {
            get { return this.color; }
            set 
            { 
                this.color = value;
                for (int i = 0; i < this.vertices.Length; i++)
                    vertices[i].Color = value;
            
            }
        }
        /// <summary>Red component</summary>
        public float Red
        {
            get { return (float)this.color.R ; }
            set 
            {
                for (int i = 0; i < this.vertices.Length; i++)
                    vertices[i].Color.R = (byte)value; 
            }
        }
        /// <summary>Green component</summary>
        public float Green
        {
            get { return (float)this.color.G; }
            set
            {
                for (int i = 0; i < this.vertices.Length; i++)
                    vertices[i].Color.G = (byte)value;
            }
        }
        /// <summary>Blue component</summary>
        public float Blue
        {
            get { return (float)this.color.B; }
            set
            {
                for (int i = 0; i < this.vertices.Length; i++)
                    vertices[i].Color.B = (byte)value;
            }
        }
        
        /// <summary>overall quad opacity, 0-1f</summary>
        public float Alpha
        {
            set
            {
                for (int i = 0; i < this.vertices.Length; i++)
                    vertices[i].Color.A = (byte)(value * 255);
            }
            get { return (float) (color.A * 255); }
        }
    }
}
