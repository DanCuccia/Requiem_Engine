using System;
using Microsoft.Xna.Framework;

namespace Engine.Math_Physics
{
    /// <summary>this object will compute a curve in 3D space</summary>
    public sealed class Curve3D
    {
        /// <summary>X-component computation</summary>
        public Curve curveX = new Curve();
        /// <summary>Y-component computation</summary>
        public Curve curveY = new Curve();
        /// <summary>Z-component computation</summary>
        public Curve curveZ = new Curve();

        /// <summary>default ctor, must call AddPoint()</summary>
        public Curve3D()
        {
            curveX.PostLoop = CurveLoopType.Oscillate;
            curveY.PostLoop = CurveLoopType.Oscillate;
            curveZ.PostLoop = CurveLoopType.Oscillate;

            curveX.PreLoop = CurveLoopType.Oscillate;
            curveY.PreLoop = CurveLoopType.Oscillate;
            curveZ.PreLoop = CurveLoopType.Oscillate;

        }

        /// <summary>calculate the curve's tangents</summary>
        public void SetTangents()
        {
            CurveKey prev;
            CurveKey current;
            CurveKey next;
            int prevIndex;
            int nextIndex;
            for (int i = 0; i < curveX.Keys.Count; i++)
            {
                prevIndex = i - 1;
                if (prevIndex < 0) prevIndex = i;

                nextIndex = i + 1;
                if (nextIndex == curveX.Keys.Count) nextIndex = i;

                prev = curveX.Keys[prevIndex];
                next = curveX.Keys[nextIndex];
                current = curveX.Keys[i];
                SetCurveKeyTangent(ref prev, ref current, ref next);
                curveX.Keys[i] = current;
                prev = curveY.Keys[prevIndex];
                next = curveY.Keys[nextIndex];
                current = curveY.Keys[i];
                SetCurveKeyTangent(ref prev, ref current, ref next);
                curveY.Keys[i] = current;

                prev = curveZ.Keys[prevIndex];
                next = curveZ.Keys[nextIndex];
                current = curveZ.Keys[i];
                SetCurveKeyTangent(ref prev, ref current, ref next);
                curveZ.Keys[i] = current;
            }
        }

        /// <summary>Add a point to the curve</summary>
        /// <param name="point">position</param>
        /// <param name="time">time in millies</param>
        public void AddPoint(Vector3 point, float time)
        {
            curveX.Keys.Add(new CurveKey(time, point.X));
            curveY.Keys.Add(new CurveKey(time, point.Y));
            curveZ.Keys.Add(new CurveKey(time, point.Z));
        }

        /// <summary>get the position of this curve according to a specific time</summary>
        /// <param name="time">at what time of the curve</param>
        /// <returns>vector3 world location</returns>
        public Vector3 GetPointOnCurve(float time)
        {
            Vector3 point = new Vector3();
            point.X = curveX.Evaluate(time);
            point.Y = curveY.Evaluate(time);
            point.Z = curveZ.Evaluate(time);
            return point;
        }

        static void SetCurveKeyTangent(ref CurveKey prev, ref CurveKey cur, ref CurveKey next)
        {
            float dt = next.Position - prev.Position;
            float dv = next.Value - prev.Value;
            if (Math.Abs(dv) < float.Epsilon)
            {
                cur.TangentIn = 0;
                cur.TangentOut = 0;
            }
            else
            {
                // The in and out tangents should be equal to the slope between the adjacent keys.
                cur.TangentIn = dv * (cur.Position - prev.Position) / dt;
                cur.TangentOut = dv * (next.Position - cur.Position) / dt;
            }
        }

    }
}
