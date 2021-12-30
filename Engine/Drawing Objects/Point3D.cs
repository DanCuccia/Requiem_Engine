using Engine.Managers.Camera;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#pragma warning disable 1573
#pragma warning disable 1572
namespace Engine.Drawing_Objects
{
    /// <summary>Used to draw a point with cross-hairs in 3D space (NOT WORKING - DO NOT USE RIGHT NOW)</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class Point3D : Object3D
    {
        #region Member Varialbes

        float                   radius              = 30f;
        Line3D[]                lines               = new Line3D[6];
        OrientedBoundingBox[]   bounding;

        static Color            color               = Color.Red;
        const float             diameter            = 3f;

        static Vector3          grabbedDirection    = Vector3.Zero;
        static Vector3          grabOffset          = Vector3.Zero;

        #endregion Member Variables

        #region Initialization

        /// <summary>default CTOR</summary>
        public Point3D()
        {    }

        /// <summary>load lines</summary>
        /// <param name="position">worldspace position</param>
        /// <param name="radius">distance from center for each line</param>
        public void Initialize(Vector3 position, float radius = 50f)
        {
            this.radius = radius;
            WorldMatrix.Position = position;
            boundingSphere.Center = position;
            boundingSphere.Radius = radius;

            lines[0] = new Line3D();
            lines[0].Initialize();
            lines[0].SetWorldStartEnd(position, position + new Vector3(0, radius, 0), color, color);
            lines[1] = new Line3D();
            lines[1].Initialize();
            lines[1].SetWorldStartEnd(position, position + new Vector3(0, -radius, 0), color, color);

            lines[2] = new Line3D();
            lines[2].Initialize();
            lines[2].SetWorldStartEnd(position, position + new Vector3(radius, 0, 0), color, color);
            lines[3] = new Line3D();
            lines[3].Initialize();
            lines[3].SetWorldStartEnd(position, position + new Vector3(-radius, 0, 0), color, color);

            lines[4] = new Line3D();
            lines[4].Initialize();
            lines[4].SetWorldStartEnd(position, position + new Vector3(0, 0, radius), color, color);
            lines[5] = new Line3D();
            lines[5].Initialize();
            lines[5].SetWorldStartEnd(position, position + new Vector3(0, 0, -radius), color, color);
        }

        /// <summary>Generate the Oriented Bounding Box, OBB will be null until this is called</summary>
        public override void GenerateBoundingBox()
        {
            bounding = new OrientedBoundingBox[6];
            bounding[0] = new OrientedBoundingBox(new Vector3(-diameter, 0, -diameter), new Vector3(diameter, radius, diameter));
            bounding[1] = new OrientedBoundingBox(new Vector3(-diameter, 0, -diameter), new Vector3(diameter, -radius, diameter));
            bounding[2] = new OrientedBoundingBox(new Vector3(0, -diameter, -diameter), new Vector3(radius, diameter, diameter));
            bounding[3] = new OrientedBoundingBox(new Vector3(0, -diameter, -diameter), new Vector3(-radius, diameter, diameter));
            bounding[4] = new OrientedBoundingBox(new Vector3(-diameter, -diameter, 0), new Vector3(diameter, diameter, radius));
            bounding[5] = new OrientedBoundingBox(new Vector3(-diameter, -diameter, 0), new Vector3(diameter, diameter, -radius));
            foreach (OrientedBoundingBox box in bounding)
                box.World = worldMatrix.GetWorldMatrix();

            base.boundingSphere.Center.X = WorldMatrix.X;
            base.boundingSphere.Center.Y = WorldMatrix.Y;
            base.boundingSphere.Center.Z = WorldMatrix.Z;
            base.boundingSphere.Radius = this.radius;
        }

        /// <summary>Update the OBB's world matrix, to match this model's world matrix</summary>
        public override void UpdateBoundingBox()
        {
            if (bounding != null)
            {
                foreach (OrientedBoundingBox box in bounding)
                    box.Update(worldMatrix.GetWorldMatrix());
            }
        }

        #endregion Initialization

        #region API

        /// <summary>Update the point lines</summary>
        public override void Update(ref Camera camera, GameTime time)
        {
            boundingSphere.Center.X = this.WorldMatrix.X;
            boundingSphere.Center.Y = this.WorldMatrix.Y;
            boundingSphere.Center.X = this.WorldMatrix.X;
            boundingSphere.Radius = this.radius;
            base.CullObject(ref camera);
            if (inFrustum == false)
                return;

            lines[0].SetWorldStartEnd(worldMatrix.Position, worldMatrix.Position + new Vector3(0, radius, 0), lines[0].GetColor(), lines[0].GetColor());
            lines[1].SetWorldStartEnd(worldMatrix.Position, worldMatrix.Position + new Vector3(0, -radius, 0), lines[1].GetColor(), lines[1].GetColor());
            lines[2].SetWorldStartEnd(worldMatrix.Position, worldMatrix.Position + new Vector3(radius, 0, 0), lines[2].GetColor(), lines[2].GetColor());
            lines[3].SetWorldStartEnd(worldMatrix.Position, worldMatrix.Position + new Vector3(-radius, 0, 0), lines[3].GetColor(), lines[3].GetColor());
            lines[4].SetWorldStartEnd(worldMatrix.Position, worldMatrix.Position + new Vector3(0, 0, radius), lines[4].GetColor(), lines[4].GetColor());
            lines[5].SetWorldStartEnd(worldMatrix.Position, worldMatrix.Position + new Vector3(0, 0, -radius), lines[5].GetColor(), lines[5].GetColor());
        }

        /// <summary>Tell each line to render</summary>
        public override void Render()
        {
            foreach (Line3D line in lines)
            {
                line.Render();
            }
            
        }

        /// <summary>Unused - this drawing object uses other drawing objects to display</summary>
        public override void RenderImplicit(Effect effect)
        {        }

        /// <summary>Debuging information - bounding box rendering if available</summary>
        public override void RenderDebug()
        {
            if (bounding == null)
                return;
            foreach (OrientedBoundingBox box in bounding)
                box.Render();
        }

        /// <summary>Get the OrientedBoundingBox for whatever the current grabbed direction is</summary>
        /// <returns>the OBB of the grabbed direction, or null if not currently grabbed</returns>
        private OrientedBoundingBox getCurrentBoundingBox()
        {
            if (bounding == null)
                return null;
            if (grabbedDirection == Vector3.Zero)
                return null;

            if (grabbedDirection == Vector3.Up)
                return bounding[0];
            else if (grabbedDirection == Vector3.Down)
                return bounding[1];
            else if (grabbedDirection == Vector3.Right)
                return bounding[2];
            else if (grabbedDirection == Vector3.Left)
                return bounding[3];
            else if (grabbedDirection == Vector3.Forward)
                return bounding[4];
            else if (grabbedDirection == Vector3.Backward)
                return bounding[5];

            return null;
        }

        /// <summary>Check for a ray colliding with any of the lines of this Point</summary>
        /// <param name="ray">Ray to test against point3D</param>
        /// <returns>whichever line is selected, that value is returned as a normalized direction</returns>
        public Vector3 RayTest(Ray ray)
        {
            float input, output;

            foreach(Line3D line in lines)
                line.SetColor(Color.Red);

            if (bounding[0].Intersects(ray, out input, out output) != -1)
            {
                return Vector3.Up;
            }
            else if (bounding[1].Intersects(ray, out input, out output) != -1)
            {
                return Vector3.Down;
            }
            else if (bounding[2].Intersects(ray, out input, out output) != -1)
            {
                return Vector3.Right;
            }
            else if (bounding[3].Intersects(ray, out input, out output) != -1)
            {
                return Vector3.Left;
            }
            else if (bounding[4].Intersects(ray, out input, out output) != -1)
            {
                return Vector3.Backward;
            }
            else if (bounding[5].Intersects(ray, out input, out output) != -1)
            {
                return Vector3.Forward;
            }
            return Vector3.Zero;
        }

        /// <summary>clicking any of the lines of this point will move that axes with the mouse</summary>
        /// <param name="ray">mouse ray</param>
        /// <param name="ms">mouse state</param>
        /// <returns>true if position changed</returns>
        public bool MoveWithMouse(Ray ray, MouseState ms)
        {
            Vector3 direction = this.RayTest(ray);
            if(grabbedDirection == Vector3.Zero)
                editDirectionColor(direction, Color.Green);

            if (direction != Vector3.Zero && grabbedDirection == Vector3.Zero)
            {
                grabbedDirection = direction;
            }

            if (ms.LeftButton != ButtonState.Pressed)
            {
                grabbedDirection = grabOffset = Vector3.Zero;
                return false;
            }

            Vector2 intersection;
            //X-axis
            if (grabbedDirection == Vector3.Right || grabbedDirection == Vector3.Left)
            {
                intersection = MyMath.lineIntersection(worldMatrix.X, worldMatrix.Z,
                                                        worldMatrix.X + 1f, worldMatrix.Z,
                                                        ray.Position.X, ray.Position.Z,
                                                        ray.Position.X + ray.Direction.X, ray.Position.Z + ray.Direction.Z);
                this.setGrabOffset(intersection);
                worldMatrix.X = worldMatrix.X + (intersection.X - worldMatrix.X) + grabOffset.X;
                editDirectionColor(grabbedDirection, Color.Green);
                editDirectionColor(-grabbedDirection, Color.Green);
                return true;
            }

            //Z-axis
            else if (grabbedDirection == Vector3.Forward || grabbedDirection == Vector3.Backward)
            {
                intersection = MyMath.lineIntersection(worldMatrix.X, worldMatrix.Z,
                                                        worldMatrix.X, worldMatrix.Z + 1f,
                                                        ray.Position.X, ray.Position.Z,
                                                        ray.Position.X + ray.Direction.X, ray.Position.Z + ray.Direction.Z);
                this.setGrabOffset(intersection);
                worldMatrix.Z = worldMatrix.Z + (intersection.Y - worldMatrix.Z) + grabOffset.Z;
                editDirectionColor(grabbedDirection, Color.Green);
                editDirectionColor(-grabbedDirection, Color.Green);
                return true;
            }

            //Y-axis
            else if (grabbedDirection == Vector3.Up || grabbedDirection == Vector3.Down)
            {
                intersection = MyMath.lineIntersection(worldMatrix.Z, worldMatrix.Y,
                                                        worldMatrix.Z, worldMatrix.Y + 1f,
                                                        ray.Position.Z, ray.Position.Y,
                                                        ray.Position.Z + ray.Direction.Z, ray.Position.Y + ray.Direction.Y);
                this.setGrabOffset(intersection);
                worldMatrix.Y = worldMatrix.Y + (intersection.Y - worldMatrix.Y) + grabOffset.Y;
                editDirectionColor(grabbedDirection, Color.Green);
                editDirectionColor(-grabbedDirection, Color.Green);
                return true;
            }

            return false;
        }

        /// <summary>To stop the object from snapping exactly where the mouse is, we need to find an offset on the current
        /// grabbedAxis and apply it to the point's position, using intersect, position, and grabbed direction, we can find the 
        /// exact distance on the correct plane, in 3D space</summary>
        /// <param name="intersect">the intersection location</param>
        private void setGrabOffset(Vector2 intersect)
        {
            if (grabOffset != Vector3.Zero)
                return;

            float dist = 0;
            //X-axis
            if (grabbedDirection == Vector3.Right || grabbedDirection == Vector3.Left)
            {
                dist = Vector2.Distance(new Vector2(worldMatrix.X, worldMatrix.Z), intersect);
            }
            //Z-axis
            else if (grabbedDirection == Vector3.Forward || grabbedDirection == Vector3.Backward)
            {
                dist = Vector2.Distance(new Vector2(worldMatrix.X, worldMatrix.Z), intersect);
            }
            //Y-axis
            else if (grabbedDirection == Vector3.Up || grabbedDirection == Vector3.Down)
            {
                dist = Vector2.Distance(new Vector2(worldMatrix.Z, worldMatrix.Y), intersect);
            }
            grabOffset = grabbedDirection * -dist;
        }

        /// <summary>edit a line's color according vector3 direction</summary>
        /// <param name="direction">which line to change coloer</param>
        /// <param name="color">the color you want the line to be</param>
        private void editDirectionColor(Vector3 direction, Color color)
        {
            if (direction == new Vector3(0, 1, 0))
                lines[0].SetColor(color);
            else if (direction == new Vector3(0, -1, 0))
                lines[1].SetColor(color);
            else if (direction == new Vector3(1, 0, 0))
                lines[2].SetColor(color);
            else if (direction == new Vector3(-1, 0, 0))
                lines[3].SetColor(color);
            else if (direction == new Vector3(0, 0, 1))
                lines[4].SetColor(color);
            else if (direction == new Vector3(0, 0, -1))
                lines[5].SetColor(color);
        }

        /// <summary>Call this per cycle to dynamically change the size of the crosshairs according to
        /// how far it is from the camera - effectively making it always "grabbable"</summary>
        /// <param name="cameraPosition">worldspace position of the camera</param>
        public void EditRadiusPerCamera(Vector3 cameraPosition, float offset = 0f)
        {
            this.SetRadius(Vector3.Distance(this.worldMatrix.Position, cameraPosition) / 3.5f);
        }

        /// <summary>Change the distance from center for each axis</summary>
        /// <param name="radius">worldspace distance from center</param>
        public void SetRadius(float dist)
        {
            radius = dist;
            Vector3 position = worldMatrix.Position;
            lines[0].SetWorldStartEnd(position, position + new Vector3(0, dist, 0), lines[0].GetColor(), lines[0].GetColor());
            lines[1].SetWorldStartEnd(position, position + new Vector3(0, -dist, 0), lines[1].GetColor(), lines[1].GetColor());
            lines[2].SetWorldStartEnd(position, position + new Vector3(dist, 0, 0), lines[2].GetColor(), lines[2].GetColor());
            lines[3].SetWorldStartEnd(position, position + new Vector3(-dist, 0, 0), lines[3].GetColor(), lines[3].GetColor());
            lines[4].SetWorldStartEnd(position, position + new Vector3(0, 0, dist), lines[4].GetColor(), lines[4].GetColor());
            lines[5].SetWorldStartEnd(position, position + new Vector3(0, 0, -dist), lines[5].GetColor(), lines[5].GetColor());
            this.GenerateBoundingBox();
        }

        /// <summary>Get the XML structure containing all information needed save and load this object</summary>
        /// <returns>a serializable class containing all data needed for this object</returns>
        public override XMLMedium GetXML()
        { return null; }

        /// <summary>Load all member variables from the input parameters</summary>
        /// <param name="input">all information needed to load this object is found here</param>
        public override void CreateFromXML(ContentManager content, XMLMedium inputXml)
        { }

        /// <summary> Un-Used - Do not call </summary> 
        public override Object3D GetCopy(ContentManager content)
        { return null; }

        #endregion API

        /// <summary>Because we do not use the base class OBB, but an array of OBB's, 
        /// this is overriden to always return null - be careful of this</summary>
        public override OrientedBoundingBox OBB
        { get { return null; } }
    }
}
