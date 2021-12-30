using System;
using Microsoft.Xna.Framework;
using Engine.Drawing_Objects;

namespace Engine.Math_Physics
{
    /// <summary>3D Accurate Collision, this Oriented Bounding Box (OBB) is not Axis-Aligned,
    /// meaning this can rotate along with the model it represents</summary>
    /// <author>Edited and modified by Daniel Cuccia</author>
    public sealed class OrientedBoundingBox
    {
        #region Member Variables

        private Vector3         min;
        private Vector3         max;
        private Vector3         center;
        private Vector3         extents;
        private Matrix          world       = Matrix.Identity;

        private Line3D[]        drawingLines;

        #endregion Member Variables

        #region Initialization

        /// <summary>Default CTOR - you must set World to the object this is representing</summary>
        /// <param name="min">min distance from center</param>
        /// <param name="max">max distance from center</param>
        public OrientedBoundingBox(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;

            updateFromMinMax();
        }

        /// <summary>Update the center and extends, privately called</summary>
        private void updateFromMinMax()
        {
            this.center = (min + max) * .5f;
            this.extents = (max - min) * .5f;
        }

        #endregion Initialization

        #region API

        /// <summary>Default Collision Test against another OBB</summary>
        /// <param name="other">other OBB to test against</param>
        /// <returns>true or false for collision</returns>
        public bool Intersects(OrientedBoundingBox other)
        {
            // Matrix to transform other OBB into my reference to allow me to be treated as an AABB
            Matrix toMe = other.World * Matrix.Invert(world);

            Vector3 centerOther = MyMath.Multiply(other.Center, toMe);
            Vector3 extentsOther = other.Extents;
            Vector3 separation = centerOther - center;

            Matrix3 rotations = new Matrix3(toMe);
            Matrix3 absRotations = MyMath.Abs(rotations);

            float r, r0, r1, r01;

            //--- Test case 1 - X axis
            r = Math.Abs(separation.X);
            r1 = Vector3.Dot(extentsOther, absRotations.Column(0));
            r01 = extents.X + r1;
            if (r > r01) return false;

            //--- Test case 1 - Y axis
            r = Math.Abs(separation.Y);
            r1 = Vector3.Dot(extentsOther, absRotations.Column(1));
            r01 = extents.Y + r1;
            if (r > r01) return false;

            //--- Test case 1 - Z axis
            r = Math.Abs(separation.Z);
            r1 = Vector3.Dot(extentsOther, absRotations.Column(2));
            r01 = extents.Z + r1;
            if (r > r01) return false;

            //--- Test case 2 - X axis
            r = Math.Abs(Vector3.Dot(rotations.Row(0), separation));
            r0 = Vector3.Dot(extents, absRotations.Row(0));
            r01 = r0 + extentsOther.X;
            if (r > r01) return false;

            //--- Test case 2 - Y axis
            r = Math.Abs(Vector3.Dot(rotations.Row(1), separation));
            r0 = Vector3.Dot(extents, absRotations.Row(1));
            r01 = r0 + extentsOther.Y;
            if (r > r01) return false;

            //--- Test case 2 - Z axis
            r = Math.Abs(Vector3.Dot(rotations.Row(2), separation));
            r0 = Vector3.Dot(extents, absRotations.Row(2));
            r01 = r0 + extentsOther.Z;
            if (r > r01) return false;

            //--- Test case 3 # 1
            r = Math.Abs(separation.Z * rotations[0, 1] - separation.Y * rotations[0, 2]);
            r0 = extents.Y * absRotations[0, 2] + extents.Z * absRotations[0, 1];
            r1 = extentsOther.Y * absRotations[2, 0] + extentsOther.Z * absRotations[1, 0];
            r01 = r0 + r1;
            if (r > r01) return false;

            //--- Test case 3 # 2
            r = Math.Abs(separation.Z * rotations[1, 1] - separation.Y * rotations[1, 2]);
            r0 = extents.Y * absRotations[1, 2] + extents.Z * absRotations[1, 1];
            r1 = extentsOther.X * absRotations[2, 0] + extentsOther.Z * absRotations[0, 0];
            r01 = r0 + r1;
            if (r > r01) return false;

            //--- Test case 3 # 3
            r = Math.Abs(separation.Z * rotations[2, 1] - separation.Y * rotations[2, 2]);
            r0 = extents.Y * absRotations[2, 2] + extents.Z * absRotations[2, 1];
            r1 = extentsOther.X * absRotations[1, 0] + extentsOther.Y * absRotations[0, 0];
            r01 = r0 + r1;
            if (r > r01) return false;

            //--- Test case 3 # 4
            r = Math.Abs(separation.X * rotations[0, 2] - separation.Z * rotations[0, 0]);
            r0 = extents.X * absRotations[0, 2] + extents.Z * absRotations[0, 0];
            r1 = extentsOther.Y * absRotations[2, 1] + extentsOther.Z * absRotations[1, 1];
            r01 = r0 + r1;
            if (r > r01) return false;

            //--- Test case 3 # 5
            r = Math.Abs(separation.X * rotations[1, 2] - separation.Z * rotations[1, 0]);
            r0 = extents.X * absRotations[1, 2] + extents.Z * absRotations[1, 0];
            r1 = extentsOther.X * absRotations[2, 1] + extentsOther.Z * absRotations[0, 1];
            r01 = r0 + r1;
            if (r > r01) return false;

            //--- Test case 3 # 6
            r = Math.Abs(separation.X * rotations[2, 2] - separation.Z * rotations[2, 0]);
            r0 = extents.X * absRotations[2, 2] + extents.Z * absRotations[2, 0];
            r1 = extentsOther.X * absRotations[1, 1] + extentsOther.Y * absRotations[0, 1];
            r01 = r0 + r1;
            if (r > r01) return false;

            //--- Test case 3 # 7
            r = Math.Abs(separation.Y * rotations[0, 0] - separation.X * rotations[0, 1]);
            r0 = extents.X * absRotations[0, 1] + extents.Y * absRotations[0, 0];
            r1 = extentsOther.Y * absRotations[2, 2] + extentsOther.Z * absRotations[1, 2];
            r01 = r0 + r1;
            if (r > r01) return false;

            //--- Test case 3 # 8
            r = Math.Abs(separation.Y * rotations[1, 0] - separation.X * rotations[1, 1]);
            r0 = extents.X * absRotations[1, 1] + extents.Y * absRotations[1, 0];
            r1 = extentsOther.X * absRotations[2, 2] + extentsOther.Z * absRotations[0, 2];
            r01 = r0 + r1;
            if (r > r01) return false;

            //--- Test case 3 # 9
            r = Math.Abs(separation.Y * rotations[2, 0] - separation.X * rotations[2, 1]);
            r0 = extents.X * absRotations[2, 1] + extents.Y * absRotations[2, 0];
            r1 = extentsOther.X * absRotations[1, 2] + extentsOther.Y * absRotations[0, 2];
            r01 = r0 + r1;
            if (r > r01) return false;

            return true;  // No separating axis, we have intersection
        }

        /// <summary>Intersection test for Ray Collision</summary>
        /// <param name="ray">the ray to test going through this OBB, position and direction</param>
        /// <param name="nearCollision">if collision this variable is the entry distance from ray origin</param>
        /// <param name="farCollision">if collision this variable is exit distance from ray origin</param>
        /// <returns>-1 for no collision, anything other is collision</returns>
        public int Intersects(Ray ray, out float nearCollision, out float farCollision)
        {
            Vector3 rayOrigin = ray.Position;
            Vector3 rayDirection = ray.Direction;
            Matrix inverseWorld;
            Matrix.Invert(ref world, out inverseWorld);

            Vector3.Transform(ref rayOrigin, ref inverseWorld, out rayOrigin);
            Vector3.TransformNormal(ref rayDirection, ref inverseWorld, out rayDirection);

            Vector3 min = this.min;
            Vector3 max = this.max;

            float t, t1, t2;

            nearCollision = float.MinValue;
            farCollision = float.MaxValue;

            int face, i = -1, j = -1;

            if (rayDirection.X > -0.00001f && rayDirection.X < -0.00001f)
            {
                if (rayOrigin.X < min.X || rayOrigin.X > max.X)
                    return -1;
            }
            else
            {
                t = 1.0f / rayDirection.X;
                t1 = (min.X - rayOrigin.X) * t;
                t2 = (max.X - rayOrigin.X) * t;

                if (t1 > t2)
                {
                    t = t1; t1 = t2; t2 = t;
                    face = 0;
                }
                else
                    face = 3;

                if (t1 > nearCollision)
                {
                    nearCollision = t1;
                    i = face;
                }
                if (t2 < farCollision)
                {
                    farCollision = t2;
                    if (face > 2)
                        j = face - 3;
                    else
                        j = face + 3;
                }

                if (nearCollision > farCollision || farCollision < 0.00001f)
                    return -1;
            }

            // intersect in Y 
            if (rayDirection.Y > -0.00001f && rayDirection.Y < -0.00001f)
            {
                if (rayOrigin.Y < min.Y || rayOrigin.Y > max.Y)
                    return -1;
            }
            else
            {
                t = 1.0f / rayDirection.Y;
                t1 = (min.Y - rayOrigin.Y) * t;
                t2 = (max.Y - rayOrigin.Y) * t;

                if (t1 > t2)
                {
                    t = t1; t1 = t2; t2 = t;
                    face = 1;
                }
                else
                    face = 4;

                if (t1 > nearCollision)
                {
                    nearCollision = t1;
                    i = face;
                }
                if (t2 < farCollision)
                {
                    farCollision = t2;
                    if (face > 2)
                        j = face - 3;
                    else
                        j = face + 3;
                }

                if (nearCollision > farCollision || farCollision < 0.00001f)
                    return -1;
            }

            // intersect in Z 
            if (rayDirection.Z > -0.00001f && rayDirection.Z < -0.00001f)
            {
                if (rayOrigin.Z < min.Z || rayOrigin.Z > max.Z)
                    return -1;
            }
            else
            {
                t = 1.0f / rayDirection.Z;
                t1 = (min.Z - rayOrigin.Z) * t;
                t2 = (max.Z - rayOrigin.Z) * t;

                if (t1 > t2)
                {
                    t = t1; t1 = t2; t2 = t;
                    face = 2;
                }
                else
                    face = 5;

                if (t1 > nearCollision)
                {
                    nearCollision = t1;
                    i = face;
                }
                if (t2 < farCollision)
                {
                    farCollision = t2;
                    if (face > 2)
                        j = face - 3;
                    else
                        j = face + 3;
                }
            }

            if (nearCollision > farCollision || farCollision < 0.00001f)
                return -1;

            if (nearCollision < 0.0f)
                return j;
            else
                return i;
        }

        /// <summary>Render this OBB to screen - requires 12 Line3D objects per OBB)</summary>
        public void Render()
        {
            if (drawingLines == null)
            {
                initDrawables();
                updateDrawables();
                for (int i = 0; i < 12; i++)
                    drawingLines[i].GenerateBoundingBox();
            }
            foreach (Line3D line in drawingLines)
            {
                line.Render();
            }
        }

        /// <summary>Create the Line3D array used to draw the OBB to screen,
        /// privately called whenever this OBB is fist called to Render</summary>
        private void initDrawables()
        {
            drawingLines = new Line3D[12];
            for (int i = 0; i < 12; i++)
            {
                drawingLines[i] = new Line3D();
                drawingLines[i].Initialize();
            }
        }

        /// <summary>Updates the Line3D array transformed by current world values</summary>
        private void updateDrawables()
        {
            if (drawingLines == null)
                throw new Exception("OBB::updateDrawables - should not have gotten here");

            Vector3[] vertices = new Vector3[8];

            //vertices[0] = new Vector3(Min.X, Min.Y, Min.Z);
            vertices[0].X = Min.X; vertices[0].Y = Min.Y; vertices[0].Z = Min.Z;
            //vertices[1] = new Vector3(Max.X, Min.Y, Min.Z);
            vertices[1].X = Max.X; vertices[1].Y = Min.Y; vertices[1].Z = Min.Z;
            //vertices[2] = new Vector3(Max.X, Max.Y, Min.Z);
            vertices[2].X = Max.X; vertices[2].Y = Max.Y; vertices[2].Z = Min.Z;
            //vertices[3] = new Vector3(Min.X, Max.Y, Min.Z);
            vertices[3].X = Min.X; vertices[3].Y = Max.Y; vertices[3].Z = Min.Z;
            //vertices[4] = new Vector3(Min.X, Min.Y, Max.Z);
            vertices[4].X = Min.X; vertices[4].Y = Min.Y; vertices[4].Z = Max.Z;
            //vertices[5] = new Vector3(Max.X, Min.Y, Max.Z);
            vertices[5].X = Max.X; vertices[5].Y = Min.Y; vertices[5].Z = Max.Z;
            //vertices[6] = new Vector3(Max.X, Max.Y, Max.Z);
            vertices[6].X = Max.X; vertices[6].Y = Max.Y; vertices[6].Z = Max.Z;
            //vertices[7] = new Vector3(Min.X, Max.Y, Max.Z);
            vertices[7].X = Min.X; vertices[7].Y = Max.Y; vertices[7].Z = Max.Z;

            Matrix w = this.world;
            Vector3.Transform(vertices, ref w, vertices);

            //top box
            drawingLines[0].SetWorldStartEnd(vertices[3], vertices[2], Color.Crimson, Color.Crimson);
            drawingLines[1].SetWorldStartEnd(vertices[2], vertices[6], Color.Crimson, Color.Crimson);
            drawingLines[2].SetWorldStartEnd(vertices[6], vertices[7], Color.Crimson, Color.Crimson);
            drawingLines[3].SetWorldStartEnd(vertices[7], vertices[3], Color.Crimson, Color.Crimson);

            //vertical edges
            drawingLines[4].SetWorldStartEnd(vertices[3], vertices[0], Color.Crimson, Color.Crimson);
            drawingLines[5].SetWorldStartEnd(vertices[2], vertices[1], Color.Crimson, Color.Crimson);
            drawingLines[6].SetWorldStartEnd(vertices[7], vertices[4], Color.Crimson, Color.Crimson);
            drawingLines[7].SetWorldStartEnd(vertices[6], vertices[5], Color.Crimson, Color.Crimson);

            //bottom box
            drawingLines[8].SetWorldStartEnd(vertices[0], vertices[1], Color.Crimson, Color.Crimson);
            drawingLines[9].SetWorldStartEnd(vertices[1], vertices[5], Color.Crimson, Color.Crimson);
            drawingLines[10].SetWorldStartEnd(vertices[5], vertices[4], Color.Crimson, Color.Crimson);
            drawingLines[11].SetWorldStartEnd(vertices[4], vertices[0], Color.Crimson, Color.Crimson);
        }

        /// <summary>Update the OBB world matrix, 
        /// will also update the drawingLines if they are available to</summary>
        /// <param name="world">new world orientation matrix</param>
        public void Update(Matrix world)
        {
            this.World = world;

            if(drawingLines != null)
                this.updateDrawables();
        }

        #endregion API

        #region Mutators
        /// <summary>Vector3 dimensions (max - min)</summary>
        public Vector3 Dimensions
        {
            get { return this.Max - this.Min; }
        }
        /// <summary>max position</summary>
        public Vector3 Max
        {
            get { return this.max; }
            set { this.max = value; this.updateFromMinMax(); }
        }
        /// <summary>min position</summary>
        public Vector3 Min
        {
            get { return this.min; }
            set { this.min = value; this.updateFromMinMax(); }
        }
        /// <summary>center position (world space)</summary>
        public Vector3 Center
        {
            get { return this.center; }
        }
        /// <summary>extentes from center</summary>
        public Vector3 Extents
        {
            get { return this.extents; }
        }
        /// <summary>world orientation</summary>
        public Matrix World
        {
            get { return this.world; }
            set { this.world = value; }
        }
        #endregion Mutators
    }
}
