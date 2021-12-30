using System;
using Engine.Drawing_Objects.Materials;
using Engine.Managers;
using Engine.Managers.Camera;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Drawing_Objects
{
    /// <summary>All values needed to save and load this drawing object</summary>
    [Serializable]
    public class Line3DXML : XMLMedium
    {
        /// <summary>start position component</summary>
        public Vector3 start;
        /// <summary>end position component</summary>
        public Vector3 end;
        /// <summary>color component</summary>
        public Color color;
        /// <summary>world matrix xml</summary>
        public WorldMatrixXml world;
        /// <summary>material of this drawable</summary>
        public MaterialXML material;
    }

    /// <summary>Draws a Line in 3D space, using the Renderer's Material system</summary>
    /// <author>Daniel Cuccia</author>
    public class Line3D : Object3D
    {
        #region Member Variables

        GraphicsDevice          device;///ref
        VertexPositionColor[]   pointList;
        VertexBuffer            vertexBuffer;
        int                     points              = 2;
        short[]                 indicies;

        /// <summary>World Matrix takes care of positioning, this takes care of direction and length</summary>
        Vector3                 vector               = Vector3.One;

        #endregion Member Variables

        #region Initialization

        /// <summary>Default CTOR</summary>
        public Line3D()
            :base()
        {
            device = Renderer.getInstance().Device;
        }

        /// <summary>Load assets and parameters</summary>
        public void Initialize()
        {
            base.material = new Line3DMaterial(this);
            if (EngineFlags.drawDevelopment == true)
                base.depthMaterial = new DepthLineMaterial(this);
            this.initializeLineList();
        }

        #endregion Initialization

        #region API

        /// <summary>Default Main Call Draw - queues within Renderer</summary>
        public override void Render()
        {
            renderer.AddToBatch(this, Material.ID);
            if (base.DepthMaterial != null)
                renderer.AddToDepthBatch(this, DepthMaterial.ID);
        }

        /// <summary>Render any debugging information here, in 3D</summary>
        public override void RenderDebug()
        {
            //WALDO!
        }

        /// <summary>Called by renderer to actually draw the line to screen</summary>
        /// <param name="effect">the current shared and active effect</param>
        public override void RenderImplicit(Effect effect)
        {
            if (pointList == null)
                throw new ArgumentNullException("Line3D::RenderImplicit pointList is null");

            effect.Parameters["World"].SetValue(worldMatrix.GetWorldMatrix());

            device.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.LineList, pointList, 0, 2, indicies, 0, 1);
        }

        /// <summary>Update Logic - nothing in here</summary>
        /// <param name="time">current game time values</param>
        /// <param name="camera">camera reference used for culling</param>
        public override void Update(ref Camera camera, GameTime time) { }

        /// <summary>Set the 3D world values</summary>
        /// <param name="position">the beginning position of the line</param>
        /// <param name="vector">the vector of the line (direction and magnitude)</param>
        /// <param name="color1">starting color, gradients into end color</param>
        /// <param name="color2">ending color</param>
        public void SetWorldVector(Vector3 position, Vector3 vector, Color color1, Color color2)
        {
            worldMatrix.Position = position;
            if (pointList == null)
            {
                pointList = new VertexPositionColor[points];
                pointList[0] = new VertexPositionColor(new Vector3(), color1);
                pointList[1] = new VertexPositionColor(new Vector3(vector.X, vector.Y, vector.Z), color2);
            }
            else
            {
                pointList[0].Color = color1;
                pointList[0].Position.X = pointList[0].Position.Y = pointList[0].Position.Z = 0f;

                pointList[1].Color = color2;
                pointList[1].Position.X = vector.X;
                pointList[1].Position.Y = vector.Y;
                pointList[1].Position.Z = vector.Z;
                
            }

            if (vertexBuffer == null)
            {
                vertexBuffer = new VertexBuffer(device,
                    VertexPositionColor.VertexDeclaration,
                    pointList.Length, BufferUsage.None);
                vertexBuffer.SetData<VertexPositionColor>(pointList);
            }

            GenerateBoundingBox();
        }

        /// <summary>Set the Positions of the line using worldSace values</summary>
        /// <param name="start">beginning world position of the line</param>
        /// <param name="end">end world position of the line</param>
        /// <param name="color1">starting color of the line</param>
        /// <param name="color2">ending color of the line</param>
        public void SetWorldStartEnd(Vector3 start, Vector3 end, Color color1, Color color2)
        {
            worldMatrix.Zero();

            if (pointList == null)
            {
                pointList = new VertexPositionColor[points];
                pointList[0] = new VertexPositionColor(new Vector3(start.X, start.Y, start.Z), color1);
                pointList[1] = new VertexPositionColor(new Vector3(end.X, end.Y, end.Z), color2);
            }
            else
            {
                pointList[0].Position.X = start.X;
                pointList[0].Position.Y = start.Y;
                pointList[0].Position.Z = start.Z;
                pointList[0].Color = color1;

                pointList[1].Position.X = end.X;
                pointList[1].Position.Y = end.Y;
                pointList[1].Position.Z = end.Z;
                pointList[1].Color = color2;
            }

            if (vertexBuffer == null)
            {
                vertexBuffer = new VertexBuffer(device,
                    VertexPositionColor.VertexDeclaration,
                    pointList.Length, BufferUsage.None);
                vertexBuffer.SetData<VertexPositionColor>(pointList);
            }

            GenerateBoundingBox();
        }

        /// <summary>Update the line3D without allocating new memory</summary>
        /// <param name="start">world starting position</param>
        /// <param name="end">world ending position</param>
        public void UpdateWorldStartEnd(Vector3 start, Vector3 end)
        {
            pointList[0].Position.X = start.X;
            pointList[0].Position.Y = start.Y;
            pointList[0].Position.Z = start.Z;
            pointList[1].Position.X = end.X;
            pointList[1].Position.Y = end.Y;
            pointList[1].Position.Z = end.Z;
        }

        /// <summary>generate indices</summary>
        private void initializeLineList()
        {
            indicies = new short[2];
            for (short i = 0; i < 1; i++)
            {
                indicies[i * 2] = (i);
                indicies[(i * 2) + 1] = (short)(i + 1);
            }
        }

        /// <summary>Set color to all vertices</summary>
        /// <param name="color">color to apply to all verts</param>
        public void SetColor(Color color)
        {
            pointList[0].Color = color;
            pointList[1].Color = color;
        }

        /// <summary>get color (uses first vertex in the list)</summary>
        /// <returns>color of this line</returns>
        public Color GetColor()
        {
            if(pointList != null)
                return pointList[0].Color;
            return Color.Red;
        }

        /// <summary>generates the boundingSphere used for culling</summary>
        public override void GenerateBoundingBox() 
        {
            boundingSphere.Center = Vector3.Lerp(this.pointList[0].Position, this.pointList[1].Position, .5f); 
            boundingSphere.Radius = Vector3.Distance(this.pointList[0].Position, this.pointList[1].Position) * .5f;
        }
        
        /// <summary>update the bounding sphere used for culling</summary>
        public override void UpdateBoundingBox() 
        {
            if (BoundingSphere != null)
            {
                boundingSphere.Center = Vector3.Lerp(this.pointList[0].Position, this.pointList[1].Position, .5f);
                boundingSphere.Radius = Vector3.Distance(this.pointList[0].Position, this.pointList[1].Position) * .5f;
            }
        }

        /// <summary>Get the XML structure containing all information needed save and load this object</summary>
        /// <returns>a serializable class containing all data needed for this object</returns>
        public override XMLMedium GetXML()
        {
            Line3DXML output = new Line3DXML();
            output.start = pointList[0].Position;
            output.end = pointList[1].Position;
            output.color = pointList[0].Color;
            output.world = worldMatrix.GetXml();
            output.material = material.GetXml();
            return output;
        }

        /// <summary>Load all member variables from the input parameters</summary>
        /// <param name="content">xna content manager</param>
        /// <param name="inputXml">asset from xml</param>
        public override void CreateFromXML(ContentManager content, XMLMedium inputXml)
        {
            Line3DXML input = (Line3DXML)inputXml;
            this.Initialize();
            this.SetWorldStartEnd(input.start, input.end, input.color, input.color);
            this.worldMatrix.FromXML(input.world);
        }

        /// <summary> Un-Used - Do Not Call </summary>
        public override Object3D GetCopy(ContentManager content)
        { return null; }

        #endregion API
    }
}
