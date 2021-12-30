using System;
using Engine.Managers.Camera;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Drawing_Objects
{
    /// <summary>This is used to draw a sphere of lines in 3D space</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class LineSphere3D : Object3D
    {
        Line3D[] lineList;
        float radius = 50f;
        int linesPerAxis = 12;

        /// <summary> Default CTOR, allocates and initlaizes all lines</summary>
        public LineSphere3D(int axisCount = 12)
        {
            linesPerAxis = axisCount;
            lineList = new Line3D[axisCount*3];
            for (short i = 0; i < axisCount * 3; i++)
            {
                lineList[i] = new Line3D();
                lineList[i].Initialize();
            }
        }

        /// <summary>Initialize all line positions to form a sphere, recall this to change radius</summary>
        /// <param name="radius">distance from center</param>
        /// <param name="color">color of the sphere</param>
        /// <param name="position">center position of the sphere</param>
        public void Initialize(Vector3 position, float radius, Color color)
        {
            this.radius = radius;
            float angleStep = 360 / this.linesPerAxis;
            for (short i = 0; i < linesPerAxis; i++)
            {
                lineList[i].SetWorldStartEnd(
                    new Vector3(position.X + (this.radius * (float)Math.Cos((double)MathHelper.ToRadians(i * angleStep))),
                        position.Y,
                        position.Z + (this.radius * (float)Math.Sin(MathHelper.ToRadians(i * angleStep)))),
                    new Vector3(position.X + (this.radius * (float)Math.Cos(MathHelper.ToRadians((i + 1) * angleStep))),
                        position.Y,
                        position.Z + (this.radius * (float)Math.Sin(MathHelper.ToRadians((i + 1) * angleStep)))),
                        color, color);

                lineList[i + linesPerAxis].SetWorldStartEnd(
                    new Vector3(position.X,
                        position.Y + (this.radius * (float)Math.Cos(MathHelper.ToRadians(i * angleStep))),
                        position.Z + (this.radius * (float)Math.Sin(MathHelper.ToRadians(i * angleStep)))),
                    new Vector3(position.X,
                        position.Y + (this.radius * (float)Math.Cos(MathHelper.ToRadians((i + 1) * angleStep))),
                        position.Z + (this.radius * (float)Math.Sin(MathHelper.ToRadians((i + 1) * angleStep)))),
                        color, color);

                lineList[i + (linesPerAxis*2)].SetWorldStartEnd(
                    new Vector3(position.X + (this.radius * (float)Math.Cos(MathHelper.ToRadians(i * angleStep))),
                        position.Y + (this.radius * (float)Math.Sin(MathHelper.ToRadians(i * angleStep))),
                        position.Z),
                    new Vector3(position.X + (this.radius * (float)Math.Cos(MathHelper.ToRadians((i + 1) * angleStep))),
                        position.Y + (this.radius * (float)Math.Sin(MathHelper.ToRadians((i + 1) * angleStep))),
                        position.Z),
                        color, color);
            }
        }

        /// <summary>update the position and radius w/out allocating new memory</summary>
        public void UpdateVertices()
        {
            float angleStep = 360 / this.linesPerAxis;
            for (short i = 0; i < linesPerAxis; i++)
            {
                lineList[i].UpdateWorldStartEnd(
                    new Vector3(WorldMatrix.X + (this.radius * (float)Math.Cos((double)MathHelper.ToRadians(i * angleStep))),
                        WorldMatrix.Y,
                        WorldMatrix.Z + (this.radius * (float)Math.Sin(MathHelper.ToRadians(i * angleStep)))),
                    new Vector3(WorldMatrix.X + (this.radius * (float)Math.Cos(MathHelper.ToRadians((i + 1) * angleStep))),
                        WorldMatrix.Y,
                        WorldMatrix.Z + (this.radius * (float)Math.Sin(MathHelper.ToRadians((i + 1) * angleStep)))));

                lineList[i + linesPerAxis].UpdateWorldStartEnd(
                    new Vector3(WorldMatrix.X,
                        WorldMatrix.Y + (this.radius * (float)Math.Cos(MathHelper.ToRadians(i * angleStep))),
                        WorldMatrix.Z + (this.radius * (float)Math.Sin(MathHelper.ToRadians(i * angleStep)))),
                    new Vector3(WorldMatrix.X,
                        WorldMatrix.Y + (this.radius * (float)Math.Cos(MathHelper.ToRadians((i + 1) * angleStep))),
                        WorldMatrix.Z + (this.radius * (float)Math.Sin(MathHelper.ToRadians((i + 1) * angleStep)))));

                lineList[i + (linesPerAxis * 2)].UpdateWorldStartEnd(
                    new Vector3(WorldMatrix.X + (this.radius * (float)Math.Cos(MathHelper.ToRadians(i * angleStep))),
                        WorldMatrix.Y + (this.radius * (float)Math.Sin(MathHelper.ToRadians(i * angleStep))),
                        WorldMatrix.Z),
                    new Vector3(WorldMatrix.X + (this.radius * (float)Math.Cos(MathHelper.ToRadians((i + 1) * angleStep))),
                        WorldMatrix.Y + (this.radius * (float)Math.Sin(MathHelper.ToRadians((i + 1) * angleStep))),
                        WorldMatrix.Z));
            }
        }

        /// <summary>clear the list of lines in this drawable</summary>
        public void Release()
        {
            lineList = null;
        }

        /// <summary>All 3D objects must update theirselves</summary>
        public override void Update(ref Camera camera, GameTime time)
        {
            boundingSphere.Center.X = this.WorldMatrix.X;
            boundingSphere.Center.Y = this.WorldMatrix.Y;
            boundingSphere.Center.X = this.WorldMatrix.X;
            boundingSphere.Radius = this.radius;
            CullObject(ref camera);

            if (inFrustum == false)
                return;

            foreach (Line3D line in lineList)
                line.Update(ref camera, time);
        }

        /// <summary>All 3D objects draw by queuing themselves within renderer's material batchs</summary>
        public override void Render()
        {
            if(base.inFrustum)
                foreach (Line3D line in lineList)
                    line.Render();
        }

        /// <summary>generate the OBB of this sphere</summary>
        public override void GenerateBoundingBox() 
        {
            OBB = new OrientedBoundingBox(new Vector3(-radius), new Vector3(radius));
        }
        /// <summary>update the OBB's matrix</summary>
        public override void UpdateBoundingBox() 
        {
            if (OBB != null)
                OBB.Update(WorldMatrix.GetWorldMatrix());
        }

        /// <summary>Unused</summary>
        public override void RenderImplicit(Effect effect) { }
        /// <summary>Unused</summary>
        public override void RenderDebug() { }
        /// <summary>Unused</summary>
        public override XMLMedium GetXML() { return null; }
        /// <summary>Unused</summary>
        public override void CreateFromXML(ContentManager content, XMLMedium inputXml) { }
        /// <summary>Unused</summary>
        public override Object3D GetCopy(ContentManager content) { return null; }
    }
}
