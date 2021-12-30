using System;
using Microsoft.Xna.Framework;

namespace Engine.Math_Physics
{
    /// <summary>Matrix3 is used for computing OBB's</summary>
    public sealed class Matrix3
    {
        /// <summary>initialize all elements</summary>
        /// <param name="f">initialize value</param>
        public Matrix3(float f)
        {
            this.Elements = new float[3, 3];
        }

        /// <summary>create this matrix from another</summary>
        /// <param name="m">original reference matrix</param>
        public Matrix3(Matrix m)
        {
            this.Elements = new float[3, 3];

            this.Elements[0, 0] = m.M11;
            this.Elements[0, 1] = m.M12;
            this.Elements[0, 2] = m.M13;

            this.Elements[1, 0] = m.M21;
            this.Elements[1, 1] = m.M22;
            this.Elements[1, 2] = m.M23;

            this.Elements[2, 0] = m.M31;
            this.Elements[2, 1] = m.M32;
            this.Elements[2, 2] = m.M33;
        }

        /// <summary>create this matrix from another</summary>
        /// <param name="m">original reference matrix</param>
        public Matrix3(Matrix3 m)
        {
            this.Elements = new float[3, 3];

            this.Elements[0, 0] = m[0, 0];
            this.Elements[0, 1] = m[0, 1];
            this.Elements[0, 2] = m[0, 2];

            this.Elements[1, 0] = m[1, 0];
            this.Elements[1, 1] = m[1, 1];
            this.Elements[1, 2] = m[1, 2];

            this.Elements[2, 0] = m[2, 0];
            this.Elements[2, 1] = m[2, 1];
            this.Elements[2, 2] = m[2, 2];
        }

        /// <summary>direct index into matrix</summary>
        /// <param name="row">row index</param>
        /// <param name="column">col index</param>
        /// <returns>matrix value</returns>
        public float this[int row, int column]
        {
            get
            {
                if (row > 2 || column > 2)
                    throw new IndexOutOfRangeException();

                return this.Elements[row, column];
            }
            set
            {
                if (row > 2 || column > 2)
                    throw new IndexOutOfRangeException();

                this.Elements[row, column] = value;
            }
        }

        /// <summary>get row vector</summary>
        /// <param name="row">index of row</param>
        /// <returns>vector3 value of the row</returns>
        public Vector3 Row(int row)
        {
            return new Vector3(this.Elements[row, 0],
                               this.Elements[row, 1],
                               this.Elements[row, 2]);
        }

        /// <summary>get column vector</summary>
        /// <param name="column">index of column</param>
        /// <returns>vector3 value of the column</returns>
        public Vector3 Column(int column)
        {
            return new Vector3(this.Elements[0, column],
                               this.Elements[1, column],
                               this.Elements[2, column]);
        }

        float[,] Elements;
    }
}
