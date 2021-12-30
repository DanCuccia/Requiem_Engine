using Engine.Drawing_Objects;
using Microsoft.Xna.Framework;

namespace Engine.Scenes.HUD
{
    /// <summary>A grid made of lines around 3D origin, used to help perspective during development</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class Grid
    {
        private const float stepDist = 100;
        private const float axisDist = 200000;
        Line3D[] rows = new Line3D[20];
        Line3D[] cols = new Line3D[20];
        Line3D[] origin = new Line3D[3];

        /// <summary>Defalt CTOR</summary>
        public Grid()
        {
            origin[0] = new Line3D();
            origin[0].Initialize();
            origin[0].SetWorldStartEnd(new Vector3(-axisDist, 0, 0), new Vector3(axisDist, 0, 0),
                Color.Gray, Color.Gray);
            origin[1] = new Line3D();
            origin[1].Initialize();
            origin[1].SetWorldStartEnd(new Vector3(0, -axisDist, 0), new Vector3(0, axisDist, 0),
                Color.Gray, Color.Gray);
            origin[2] = new Line3D();
            origin[2].Initialize();
            origin[2].SetWorldStartEnd(new Vector3(0, 0, -axisDist), new Vector3(0, 0, axisDist),
                Color.Gray, Color.Gray);

            for (int i = 0; i < 3; i++)
            {
                origin[i].GenerateBoundingBox();
                origin[i].UpdateBoundingBox();
            }

            for (int i = 0; i < 20; i++)
            {
                float x = -stepDist * 20 * .5f;
                float z = x;
                //index 10 is the same as origin, skip
                if (i == 10)
                    continue;
                rows[i] = new Line3D();
                rows[i].Initialize();
                rows[i].SetWorldStartEnd(new Vector3(x, 0, z + (i * stepDist)),
                    new Vector3(-x, 0, z + (i * stepDist)), Color.Black, Color.Black);
                rows[i].GenerateBoundingBox();
                rows[i].UpdateBoundingBox();
                cols[i] = new Line3D();
                cols[i].Initialize();
                cols[i].SetWorldStartEnd(new Vector3(x + (i * stepDist), 0, z),
                    new Vector3(x + (i * stepDist), 0, -z), Color.Black, Color.Black);
                cols[i].GenerateBoundingBox();
                cols[i].UpdateBoundingBox();
            }

            rows[10] = new Line3D();
            rows[10].Initialize();
            rows[10].SetWorldStartEnd(
                new Vector3(stepDist * 20 * .5f, 0, stepDist * 20 * .5f),
                new Vector3(-stepDist * 20 * .5f, 0, stepDist * 20 * .5f),
                Color.Black, Color.Black);
            rows[10].GenerateBoundingBox();
            rows[10].UpdateBoundingBox();

            cols[10] = new Line3D();
            cols[10].Initialize();
            cols[10].SetWorldStartEnd(
                new Vector3(stepDist * 20 * .5f, 0, stepDist * 20 * .5f),
                new Vector3(stepDist * 20 * .5f, 0, -stepDist * 20 * .5f),
                Color.Black, Color.Black);
            cols[10].GenerateBoundingBox();
            cols[10].UpdateBoundingBox();
        }

        /// <summary>Draw the grid to screen, centered on 3D origin</summary>
        public void Render()
        {
            foreach (Line3D line in origin)
                line.Render();
            for (int i = 0; i < 20; i++)
            {
                rows[i].Render();
                cols[i].Render();
            }
        }
    }
}
