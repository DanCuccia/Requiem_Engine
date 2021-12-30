using System;
using Engine.Managers;
using Engine.Managers.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StillDesign.PhysX;

#pragma warning disable 0168

namespace Engine.Math_Physics
{
    /// <summary>this engine uses the StillDesign .NET PhysX wrapper</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class PhysXEngine
    {
        #region MemberVariables

        /// <summary>main stillDesign.physX scene object</summary>
        public StillDesign.PhysX.Scene scene;
        /// <summary>stillDesign.physx scene description</summary>
        public SceneDescription sceneDesc;
        /// <summary>Main PhysX core object</summary>
        public Core core;

        Renderer renderer;
        Effect fx;
        /// <summary>The main camera used to draw the engine data</summary>
        public Camera Camera { set; get; }
        GraphicsDevice device;

        Model box;
        Model capsule;
        Model plane;
        Model sphere;

        #endregion

        #region Init

        /// <summary>private singleton CTOR</summary>
        private PhysXEngine() { }

        private static PhysXEngine myInstance;
        /// <summary>Main Instance Accessor, you must (initialize the object, and set camera) separately</summary>
        public static PhysXEngine Instance
        {
            get
            {
                if (myInstance == null)
                    myInstance = new PhysXEngine();
                return myInstance;
            }
        }

        /// <summary>Default CTOR</summary>
        /// <param name="device">device used to render primitives</param>
        /// <param name="content">xna content manager</param>
        public void Initialize(GraphicsDevice device, ContentManager content)
        {
            CoreDescription coreDesc = new CoreDescription();
            UserOutput output = new UserOutput();
            this.core = new Core(coreDesc, output);
            fx = EffectManager.getInstance().GetEffect("shaders//PhysXVisual");
            renderer = Renderer.getInstance();
            this.device = device;

            if (EngineFlags.drawDebug)
            {
                try
                {
                    box = content.Load<Model>("models//physx//cube");
                    capsule = content.Load<Model>("models//physx//capsule");
                    plane = content.Load<Model>("models//physx//plane");
                    sphere = content.Load<Model>("models//physx//sphere");
                }
                catch (Exception e)
                {}
            }

            core.SetParameter(PhysicsParameter.VisualizationScale, 2.0f);
            core.SetParameter(PhysicsParameter.VisualizeCollisionShapes, true);
            core.SetParameter(PhysicsParameter.VisualizeClothMesh, true);
            core.SetParameter(PhysicsParameter.VisualizeJointLocalAxes, true);
            core.SetParameter(PhysicsParameter.VisualizeJointLimits, true);
            core.SetParameter(PhysicsParameter.VisualizeFluidPosition, false);
            core.SetParameter(PhysicsParameter.VisualizeFluidEmitters, false);
            core.SetParameter(PhysicsParameter.VisualizeForceFields, true);
            core.SetParameter(PhysicsParameter.VisualizeSoftBodyMesh, true);
            core.SetParameter(PhysicsParameter.DefaultSleepLinearVelocitySquared, 2.0f * 2.0f);
            core.SetParameter(PhysicsParameter.DefaultSleepAngularVelocitySquared, 2.0f * 2.0f);

            sceneDesc = new SceneDescription()
            {
                //SimulationType = SimulationType.Hardware,
                Gravity = new StillDesign.PhysX.MathPrimitives.Vector3(0, -85.81f, 0),
                GroundPlaneEnabled = true
            };

            this.scene = core.CreateScene(sceneDesc);

            core.Foundation.RemoteDebugger.Connect("localhost");
        }

        /// <summary>clear out all actors, leave all physX core objects in-tact</summary>
        public void Release()
        {
            scene.FetchResults(SimulationStatus.RigidBodyFinished, true);
            while (scene.Actors.Count > 0)
            {
                scene.Actors[scene.Actors.Count-1].Dispose();
            }
        }

        /// <summary>do a complete shutdown (do not call this more than once,
        /// once you shutdown, this must be re-initialized</summary>
        public void Shutdown()
        {
            scene.FetchResults(SimulationStatus.RigidBodyFinished, true);
            core.Dispose();
            core = null;
            scene = null;
            sceneDesc = null;
        }

        #endregion

        #region API

        /// <summary>Run simulation step</summary>
        /// <param name="time">current time values</param>
        public void Update(GameTime time)
        {
            scene.Simulate((float)time.ElapsedGameTime.TotalMilliseconds / 500f);
            scene.FlushStream();
            scene.FetchResults(SimulationStatus.RigidBodyFinished, true);
        }

        /// <summary>This is for debugging purposes only</summary>
        public void Render3D()
        {
            if (EngineFlags.drawDebug == false)
                return;

            using (DebugRenderable data = this.scene.GetDebugRenderable())
            {
                DrawDebug(data);
            }
        }

        #endregion

        #region Primitive Drawing

        /// <summary>privately used to draw the data</summary>
        /// <param name="data">debugRenderable data found from simulation</param>
        private void DrawDebug(DebugRenderable data)
        {
            EffectPass pass = fx.Techniques[0].Passes[0];

            fx.Parameters["World"].SetValue(Matrix.Identity);
            fx.Parameters["View"].SetValue(renderer.Camera.ViewMatrix);
            fx.Parameters["Projection"].SetValue(renderer.Camera.ProjectionMatrix);

            pass.Apply();

            if (data.PointCount > 0)
            {
                var points = data.GetDebugPoints();

                var vertices = new VertexPositionColor[points.Length];
                for (int i = 0; i < data.LineCount; i++)
                {
                    DebugPoint point = points[i];

                    vertices[i * 2 + 0] = new VertexPositionColor(point.Point.As<Vector3>(), Color.White);
                }

                DrawVertices(vertices);
            }

            if (data.LineCount > 0)
            {
                var lines = data.GetDebugLines();

                var vertices = new VertexPositionColor[data.LineCount * 2];
                for (int x = 0; x < data.LineCount; x++)
                {
                    DebugLine line = lines[x];

                    vertices[x * 2 + 0] = new VertexPositionColor(line.Point0.As<Vector3>(), Color.White);
                    vertices[x * 2 + 1] = new VertexPositionColor(line.Point1.As<Vector3>(), Color.White);
                }

                DrawVertices(vertices);
            }

            if (data.TriangleCount > 0)
            {
                var triangles = data.GetDebugTriangles();

                var vertices = new VertexPositionColor[data.TriangleCount * 3];
                for (int x = 0; x < data.TriangleCount; x++)
                {
                    DebugTriangle triangle = triangles[x];

                    vertices[x * 3 + 0] = new VertexPositionColor(triangle.Point0.As<Vector3>(), Color.White);
                    vertices[x * 3 + 1] = new VertexPositionColor(triangle.Point1.As<Vector3>(), Color.White);
                    vertices[x * 3 + 2] = new VertexPositionColor(triangle.Point2.As<Vector3>(), Color.White);
                }

                DrawVertices(vertices);
            }

            // World axis
            {
                var vertices = new[] 
				{
					// X
					new VertexPositionColor(new Vector3(0,0,0), new Color(10, 0, 0)),
					new VertexPositionColor(new Vector3(5,0,0), new Color(10, 0, 0)),

					// Y
					new VertexPositionColor(new Vector3(0,0,0), new Color(0, 10, 0)),
					new VertexPositionColor(new Vector3(0,5,0), new Color(0, 10, 0)),

					// Z
					new VertexPositionColor(new Vector3(0,0,0), new Color(0, 0, 10)),
					new VertexPositionColor(new Vector3(0,0,5), new Color(0, 0, 10)),
				};

                DrawVertices(vertices);
            }
        }

        /// <summary>draws a list of vertices to screen</summary>
        private void DrawVertices(VertexPositionColor[] vertices)
        {
            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, vertices.Length / 3);
        }

        /// <summary>private visualization of an arrow</summary>
        private void drawArrow(StillDesign.PhysX.MathPrimitives.Vector3 posA, 
            StillDesign.PhysX.MathPrimitives.Vector3 posB, Color color)
        {

            VertexPositionColor[] vertices = new VertexPositionColor[2];
            vertices[0] = new VertexPositionColor(new Vector3(posA.X, posA.Y, posA.Z), color);
            vertices[1] = new VertexPositionColor(new Vector3(posB.X, posB.Y, posB.Z), color);

            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 1);
        }

        /// <summary>Private visualization of a sphere object</summary>
        private void drawSphere(SphereShape sphereShape, Vector3 color)
        {
            // Draw the Box
            {
                Matrix[] transforms = new Matrix[sphere.Bones.Count];
                sphere.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in sphere.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.DiffuseColor = color;
                        effect.EnableDefaultLighting();
                        effect.World = Matrix.CreateScale(sphereShape.Radius, sphereShape.Radius, sphereShape.Radius) * 
                            transforms[mesh.ParentBone.Index] * 
                            Matrix.CreateTranslation(sphereShape.GlobalPose.M41, sphereShape.GlobalPose.M42, sphereShape.GlobalPose.M42);
                        effect.View = Camera.ViewMatrix;
                        effect.Projection = Camera.ProjectionMatrix;
                    }

                    mesh.Draw();
                }
            }
        }

        /// <summary>private visualization of a cube object</summary>
        private void drawBox(BoxShape shape, Vector3 color)
        {
            // Draw the Box
            {
                Matrix[] transforms = new Matrix[box.Bones.Count];
                box.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in box.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.DiffuseColor = color;
                        effect.EnableDefaultLighting();

                        effect.World = Matrix.CreateScale(shape.Dimensions.X, shape.Dimensions.Y, shape.Dimensions.Z) * 
                            transforms[mesh.ParentBone.Index] *
                            Matrix.CreateTranslation(shape.GlobalPose.M41, shape.GlobalPose.M42, shape.GlobalPose.M42);
                        effect.View = Camera.ViewMatrix;
                        effect.Projection = Camera.ProjectionMatrix;
                    }

                    mesh.Draw();
                }
            }
        }

        /// <summary>private visuallization of a capsule object</summary>
        private void drawCapsule(CapsuleShape shape, Vector3 color)
        {
            {
                Matrix[] transforms = new Matrix[capsule.Bones.Count];
                capsule.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in capsule.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.DiffuseColor = color;
                        effect.EnableDefaultLighting();

                        effect.World = Matrix.CreateScale(shape.Radius, shape.Height, shape.Radius) * 
                            transforms[mesh.ParentBone.Index] *
                            Matrix.CreateTranslation(shape.GlobalPose.M41, shape.GlobalPose.M42, shape.GlobalPose.M42);
                        effect.View = Camera.ViewMatrix;
                        effect.Projection = Camera.ProjectionMatrix;
                    }

                    mesh.Draw();
                }
            }
        }

        /// <summary>private visualization of a plane</summary>
        private void drawPlane(PlaneShape shape)
        {
            {
                Matrix[] transforms = new Matrix[plane.Bones.Count];
                plane.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in plane.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();

                        effect.World = Matrix.CreateScale(10240.0f, 1.0f, 10240.0f) * 
                            transforms[mesh.ParentBone.Index] *
                            Matrix.CreateTranslation(shape.GlobalPose.M41, shape.GlobalPose.M42, shape.GlobalPose.M42);
                        effect.View = Camera.ViewMatrix;
                        effect.Projection = Camera.ProjectionMatrix;
                    }

                    mesh.Draw();
                }
            }

        }

        //Dont work correctly
        private void drawForce(Actor actor, StillDesign.PhysX.MathPrimitives.Vector3 forceVec, Color color)
        {
            float magnitude = forceVec.Length();

            if (magnitude < 0.1f)
                return;

            forceVec = 6 * forceVec / magnitude;
            StillDesign.PhysX.MathPrimitives.Vector3 pos = actor.CenterOfMassGlobalPosition;
            drawArrow(pos, pos + forceVec, color);
        }

        /// <summary>StillDesign to XNA conversion</summary>
        private Color Int32ToColor(int color)
        {
            byte a = (byte)((color & 0xFF000000) >> 32);
            byte r = (byte)((color & 0x00FF0000) >> 16);
            byte g = (byte)((color & 0x0000FF00) >> 8);
            byte b = (byte)((color & 0x000000FF) >> 0);

            return new Color(r, g, b, a);
        }

        #endregion Primitive Drawing
    }
}
