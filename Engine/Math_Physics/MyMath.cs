using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Math_Physics
{
    /// <summary>Filled with lovely static functions for your everyday Mathematical ease</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class MyMath
    {
        /// <summary>Get the greatest component of a vector3</summary>
        /// <param name="val">vector3 to test</param>
        /// <returns>the greatest dimension</returns>
        public static float GetGreatestDimension(Vector3 val)
        {
            float output = 0f;
            output = val.X > output ? val.X : output;
            output = val.Y > output ? val.Y : output;
            output = val.Z > output ? val.Z : output;
            output = val.Y > output ? val.Y : output;
            return output;
        }

        /// <summary>Get the greatest component of a vector3</summary>
        /// <param name="val">vector3 to test</param>
        /// <param name="scale">scaling</param>
        /// <returns>the greatest dimension</returns>
        public static float GetGreatestDimension(Vector3 val, Vector3 scale)
        {
            float output = 0f;
            output = val.X * scale.X > output ? val.X * scale.X : output;
            output = val.Y * scale.Y > output ? val.Y * scale.Y : output;
            output = val.Z * scale.Z > output ? val.Z * scale.Z : output;
            output = val.Y * scale.Y > output ? val.Y * scale.Y : output;
            return output;
        }

        /// <summary>get the Y - rotation facing a 3D position</summary>
        /// <param name="cameraX">x-position of your 3D point in space</param>
        /// <param name="cameraZ">z-position of your 3D point in space</param>
        /// <param name="positionX">x-position of the object you want rotated</param>
        /// <param name="positionZ">z-position of the object you wanted rotated</param>
        /// <returns>Y - component</returns>
        public static float RotateYToCamera(float cameraX, float cameraZ, float positionX, float positionZ)
        {
            return 180f + MathHelper.ToDegrees((float)Math.Atan2(positionX - cameraX, positionZ - cameraZ));
        }

        /// <summary>rounds the given float to an integer, using bounds as well</summary>
        /// <param name="min">minumum int you wish to recieve</param>
        /// <param name="max">maximum int you wish to revieve</param>
        /// <param name="value">floating point value you want rounded</param>
        /// <returns>rounded float to an integer within the given bounds</returns>
        public static int RoundFloatToInt(int min, int max, float value)
        {
            if (max - min < 1)
                throw new InvalidOperationException("MyMath::RoundFloatToInt() - Invalid input arguments");

            int output = (int)value;
            if (value % 1 >= .5f)
                output++;

            if (output > max)
                output = max;
            if (output < min)
                output = min;
            
            return output;
        }

        /// <summary>using a 0-1 value, this gets the actual value between min and max</summary>
        /// <param name="min">minimum component</param>
        /// <param name="max">maximum component</param>
        /// <param name="value">0-1 value between min/max</param>
        /// <returns>actual value between min/max, found from value</returns>
        public static float GetValueBetween(float min, float max, float value)
        {
            return min + (value * (max - min));
        }

        /// <summary>This will give you the 0-1 scalar between min and max, this gets clamped to 0 and 1</summary>
        /// <param name="min">minimum component</param>
        /// <param name="max">maximum component</param>
        /// <param name="value">value you want to find the 0-1 value from</param>
        /// <returns>0-1 of where value is between min and max</returns>
        public static float GetScalarBetween(float min, float max, float value)
        {
            float val = (value - min) / (max - min);
            return (float.IsInfinity(val)) ? 0f : ((val > 1) ? 1f : val);
        }

        /// <summary>2D point within Rectangle test</summary>
        /// <param name="x">x component to test within rect</param>
        /// <param name="y">y component to test within rect</param>
        /// <param name="rect">destination rect to test x y </param>
        /// <returns>true or false, (x,y) is within the rect</returns>
        public static bool IsWithin(float x, float y, Rectangle rect)
        {
            if (x < rect.Left || x > rect.Right)
                return false;
            if (y < rect.Top || y > rect.Bottom)
                return false;
            return true;
        }
        
        /// <summary>get random float</summary>
        /// <param name="min">min value</param>
        /// <param name="max">max value</param>
        /// <returns>uniform random float</returns>
        public static float GetRandomFloat(float min, float max)
        {
            return min + ((max - min) * (float) EngineFlags.random.NextDouble());
        }

        /// <summary>get random float, if you want to keep a local random seed, use this one,
        /// sending in which ever random obj your using</summary>
        /// <param name="min">min value</param>
        /// <param name="max">max value</param>
        /// <param name="random">your local random generator, with it's own seed</param>
        /// <returns>uniform random float</returns>
        public static float GetRandomFloat(float min, float max, Random random)
        {
            return min + ((max - min) * (float)random.NextDouble());
        }

        /// <summary>Get the Angle between two vectors</summary>
        /// <param name="one">first component</param>
        /// <param name="two">second component</param>
        /// <returns>angle of theta of the dot product from the normalized magnitudes</returns>
        public static float AngleBetweenVectors(Vector3 one, Vector3 two)
        {
            double oneMag = Math.Sqrt( (one.X * one.X) + (one.Y * one.Y) + (one.Z * one.Z) );
            double twoMag = Math.Sqrt( (two.X * two.X) + (two.Y * two.Y) + (two.Z * two.Z) );
            double theta = Vector3.Dot(one, two) / (oneMag * twoMag);
            return (float)Math.Acos(theta);
        }

        /// <summary>Generate a Vector3 array of vertice positions from the list of Verts themselves</summary>
        /// <param name="vertices">vertex array, from the model buffer</param>
        /// <returns>array of vector3 vertex positions</returns>
        public static Vector3[] GenerateVertexPositionArray(VertexPositionNormalTexture[] vertices)
        {
            Vector3[] output = new Vector3[vertices.Length];

            int index = 0;
            foreach (VertexPositionNormalTexture vert in vertices)
            {
                output[index++] = vert.Position;
            }

            return output;
        }

        /// <summary>Generate a Vector3 array of vertice positions from the list of Verts themselves</summary>
        /// <param name="vertices">vertex array, from the model buffer</param>
        /// <returns>array of vector3 vertex positions</returns>
        public static Vector3[] GenerateVertexPositionArray(VertexPositionNormal[] vertices)
        {
            Vector3[] output = new Vector3[vertices.Length];

            int index = 0;
            foreach (VertexPositionNormal vert in vertices)
            {
                output[index++] = vert.Position;
            }

            return output;
        }

        /// <summary>Generate a Vector3 array of vertice positions from the list of Verts themselves</summary>
        /// <param name="vertices">vertex array, from the model buffer</param>
        /// <returns>array of vector3 vertex positions</returns>
        public static Vector3[] GenerateVertexPositionArray(VertexPositionNormalTextureTangentBinormal[] vertices)
        {
            Vector3[] output = new Vector3[vertices.Length];

            int index = 0;
            foreach (VertexPositionNormalTextureTangentBinormal vert in vertices)
            {
                output[index++] = vert.Position;
            }

            return output;
        }

        /// <summary>Generate a Vector3 array of vertice positions from the list of Verts themselves</summary>
        /// <param name="vertices">vertex array, from the model buffer</param>
        /// <returns>array of vector3 vertex positions</returns>
        public static Vector3[] GenerateVertexPositionArray(VertexNormalMappedSkeleton[] vertices)
        {
            Vector3[] output = new Vector3[vertices.Length];

            int index = 0;
            foreach (VertexNormalMappedSkeleton vert in vertices)
            {
                output[index++] = vert.Position;
            }

            return output;
        }

        /// <summary>Generate a Vector3 array of vertice positions from the list of Verts themselves</summary>
        /// <param name="vertices">vertex array, from the model buffer</param>
        /// <returns>array of vector3 vertex positions</returns>
        public static Vector3[] GenerateVertexPositionArray(VertexSkeleton[] vertices)
        {
            Vector3[] output = new Vector3[vertices.Length];

            int index = 0;
            foreach (VertexSkeleton vert in vertices)
            {
                output[index++] = vert.Position;
            }

            return output;
        }

        /// <summary>Multiply a Vector to a complete world matrix</summary>
        /// <param name="vector">the vector to transform</param>
        /// <param name="mat">the matrix that translates the vector</param>
        /// <returns>a translated vector according to matrix</returns>
        public static Vector3 Multiply(Vector3 vector, Matrix mat)
        {
            return new Vector3(vector.X * mat.M11 + vector.Y * mat.M21 + vector.Z * mat.M31 + mat.M41,
                            vector.X * mat.M12 + vector.Y * mat.M22 + vector.Z * mat.M32 + mat.M42,
                            vector.X * mat.M13 + vector.Y * mat.M23 + vector.Z * mat.M33 + mat.M43);
        }

        /// <summary>Absolute values of the custom Matrix3</summary>
        /// <param name="m3">input matrix3 to find abs values of</param>
        /// <returns>abs Matrix3</returns>
        public static Matrix3 Abs(Matrix3 m3)
        {
            Matrix3 absMatrix = new Matrix3(0);

            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    absMatrix[r, c] = Math.Abs(m3[r, c]);
                
            return absMatrix;
        }

        /// <summary> In a 2D grid, returns the angle to a specified point from the +X axis</summary>
        /// <param name="X">x component</param>
        /// <param name="Y">y component</param>
        /// <returns>0-360 angle to input, from 0,0</returns>
        public static float ArcTanAngle(float X, float Y)
        {
            if (X == 0)
            {
                if (Y == 1)
                    return (float)MathHelper.PiOver2;
                else
                    return (float)-MathHelper.PiOver2;
            }
            else if (X > 0)
                return (float)Math.Atan(Y / X);
            else if (X < 0)
            {
                if (Y > 0)
                    return (float)Math.Atan(Y / X) + MathHelper.Pi;
                else
                    return (float)Math.Atan(Y / X) - MathHelper.Pi;
            }
            else
                return 0;
        }

        /// <summary>returns Euler angles that point from one point to another</summary>
        /// <param name="from">beginning location</param>
        /// <param name="location">ending location</param>
        /// <returns>euler values from to location</returns>
        public static Vector3 AngleTo(Vector3 from, Vector3 location)
        {
            Vector3 angle = new Vector3();
            Vector3 v3 = Vector3.Normalize(location - from);
            angle.X = (float)Math.Asin(v3.Y);
            angle.Y = ArcTanAngle(-v3.Z, -v3.X);
            return angle;
        }

        /// <summary>converts a Quaternion to Euler angles (X = pitch, Y = yaw, Z = roll)</summary>
        /// <param name="rotation">quaternion orientation values</param>
        /// <returns>Vector3 Euler angle equivelant values</returns>
        public static Vector3 QuaternionToEuler(Quaternion rotation)
        {
            Vector3 rotationaxes = new Vector3();

            Vector3 forward = Vector3.Transform(Vector3.Forward, rotation);
            Vector3 up = Vector3.Transform(Vector3.Up, rotation);
            rotationaxes = AngleTo(new Vector3(), forward);
            if (rotationaxes.X == MathHelper.PiOver2)
            {
                rotationaxes.Y = ArcTanAngle(up.Z, up.X);
                rotationaxes.Z = 0;
            }
            else if (rotationaxes.X == -MathHelper.PiOver2)
            {
                rotationaxes.Y = ArcTanAngle(-up.Z, -up.X);
                rotationaxes.Z = 0;
            }
            else
            {
                up = Vector3.Transform(up, Matrix.CreateRotationY(-rotationaxes.Y));
                up = Vector3.Transform(up, Matrix.CreateRotationX(-rotationaxes.X));
                rotationaxes.Z = ArcTanAngle(up.Y, -up.X);
            }
            return rotationaxes;
        }

        /// <summary>line intersection dirivative function, to use this in 3D space, throw out the dimension you are 
        /// not testing (eg. to find X intersection, use a top-down perspective with X and Z</summary>
        /// <param name="x1">line 1 x1</param>
        /// <param name="y1">line 1 y1</param>
        /// <param name="x2">line 1 x2</param>
        /// <param name="y2">line 1 y2</param>
        /// <param name="x3">line 2 x1</param>
        /// <param name="y3">line 2 y1</param>
        /// <param name="x4">line 2 x2</param>
        /// <param name="y4">line 2 y2</param>
        /// <returns>intersection point on concurrent place, Vector2(0) if lines are parallel</returns>
        public static Vector2 lineIntersection(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            float bx = x2 - x1;
            float by = y2 - y1;
            float dx = x4 - x3;
            float dy = y4 - y3;
            float b_dot_d_perp = bx * dy - by * dx;
            if (b_dot_d_perp == 0)
            {
                return Vector2.Zero;
            }
            float cx = x3 - x1;
            float cy = y3 - y1;
            float t = (cx * dy - cy * dx) / b_dot_d_perp;

            return new Vector2(x1 + t * bx, y1 + t * by);
        }
    }
}
