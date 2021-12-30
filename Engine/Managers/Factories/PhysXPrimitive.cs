using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StillDesign.PhysX;
using Engine.Math_Physics;
using Engine.Drawing_Objects;
using Microsoft.Xna.Framework;

using StillDesign.PhysX.MathPrimitives;

namespace Engine.Managers.Factories
{
    /// <summary>This is a static factory to build the base primitive objects of PhysX</summary>
    /// <author>Daniel Cuccia</author>
    public abstract class PhysXPrimitive
    {
        /// <summary>un-creatable object</summary>
        private PhysXPrimitive(){}

        /// <summary>Create a new PhysX Cube Actor</summary>
        /// <param name="dimensions">size of the cube</param>
        /// <param name="position">beginning location of the cube</param>
        /// <returns>reference to the actor (it has already been added to the engine's scene)</returns>
        public static Actor GetBoxActor(Microsoft.Xna.Framework.Vector3 dimensions,
            Microsoft.Xna.Framework.Vector3 position)
        {
            PhysXEngine engine = PhysXEngine.Instance;

            BoxShapeDescription boxDesc = new BoxShapeDescription();
            ActorDescription actorDesc = new ActorDescription();
            BodyDescription bodyDesc = new BodyDescription();

            boxDesc.Dimensions = new StillDesign.PhysX.MathPrimitives.Vector3(dimensions.X, dimensions.Y, dimensions.Z);
            boxDesc.LocalPosition = new StillDesign.PhysX.MathPrimitives.Vector3(position.X, position.Y, position.Z);
            actorDesc.GlobalPose = StillDesign.PhysX.MathPrimitives.Matrix.Translation(position.X, position.Y, position.Z);
            actorDesc.Shapes.Add(boxDesc);
            actorDesc.BodyDescription = bodyDesc;
            actorDesc.Density = 3f;

            if (!actorDesc.IsValid())
                throw new Exception("PhysXPrimitive::GetBoxActor - Invalid Data Found");

            return engine.scene.CreateActor(actorDesc);
        }

        public static Actor GetFrozenBoxActor(Microsoft.Xna.Framework.Vector3 dimensions,
            Microsoft.Xna.Framework.Vector3 position)
        {
            PhysXEngine engine = PhysXEngine.Instance;

            BoxShapeDescription boxDesc = new BoxShapeDescription();
            ActorDescription actorDesc = new ActorDescription();
            BodyDescription bodyDesc = new BodyDescription();

            bodyDesc.BodyFlags = BodyFlag.FrozenPosition | BodyFlag.FrozenRotation;
            actorDesc.BodyDescription = bodyDesc;

            boxDesc.Dimensions = new StillDesign.PhysX.MathPrimitives.Vector3(dimensions.X, dimensions.Y, dimensions.Z);
            boxDesc.LocalPosition = new StillDesign.PhysX.MathPrimitives.Vector3(position.X, position.Y, position.Z);

            actorDesc.GlobalPose = StillDesign.PhysX.MathPrimitives.Matrix.Translation(position.X, position.Y, position.Z);
            actorDesc.Shapes.Add(boxDesc);
            actorDesc.Density = 150f;

            if (!actorDesc.IsValid())
                throw new Exception("PhysXPrimitive::GetBoxActor - Invalid Data Found");

            return engine.scene.CreateActor(actorDesc);
        }
        /// <summary>Get a capsule PhysX actor to represent a player</summary>
        /// <param name="dimensions">dimensions of the box</param>
        /// <param name="position">beginning position of the box</param>
        /// <returns>reference to the actor (it has already been added to the engine's scene</returns>
        public static Actor GetPlayerActor(Microsoft.Xna.Framework.Vector3 dimensions,
            Microsoft.Xna.Framework.Vector3 position)
        {
            PhysXEngine engine = PhysXEngine.Instance;

            if (engine == null)
                return null;
            CapsuleShapeDescription capsule = new CapsuleShapeDescription();
            capsule.Radius = (dimensions.X + dimensions.Z) * .5f;
            capsule.Height = dimensions.Y;
            capsule.Material = engine.scene.Materials[0];

            BodyDescription bodyDesc = new BodyDescription();
            bodyDesc.BodyFlags = BodyFlag.FrozenRotation;

            ActorDescription actorDesc = new ActorDescription();
            actorDesc.BodyDescription = bodyDesc;
            actorDesc.Density = 5f;
            actorDesc.GlobalPose = StillDesign.PhysX.MathPrimitives.Matrix.Translation(position.X, position.Y + (capsule.Height * 0.5f), position.Z);

            actorDesc.Shapes.Add(capsule);

            if (!actorDesc.IsValid())
                throw new Exception("PhysXPrimitive::GetPlayerActor - Invalid Data Found");

            return engine.scene.CreateActor(actorDesc);
        }

        /// <summary>Create the default ground plane actor (at 0,0,0)</summary>
        /// <returns>reference to the actor (it has already been added to the engine's scene)</returns>
        public static Actor GetGroundPlaneActor()
        {
            PhysXEngine engine = PhysXEngine.Instance;

            PlaneShapeDescription planeDesc = new PlaneShapeDescription();
            ActorDescription actorDesc = new ActorDescription();
            planeDesc.Material = engine.scene.Materials[0];
            actorDesc.Shapes.Add(planeDesc);
            return engine.scene.CreateActor(actorDesc);
        }

        /// <summary>Get a box actor using data from an obb and positioned from a world matrix</summary>
        /// <param name="obb">oriented bounding box</param>
        /// <param name="world">world matrix object</param>
        /// <returns>the physx actor</returns>
        public static Actor GetActor(OrientedBoundingBox obb, WorldMatrix world)
        {
            PhysXEngine engine = PhysXEngine.Instance;

            if (engine == null) return null;
            if (world == null) return null;
            if (obb == null) return null;

            ActorDescription actorDesc = new ActorDescription();
            BodyDescription bodyDesc = new BodyDescription();
            BoxShapeDescription boxDesc = new BoxShapeDescription();

            bodyDesc.BodyFlags = BodyFlag.FrozenPosition | BodyFlag.FrozenRotation;
            actorDesc.BodyDescription = bodyDesc;
            actorDesc.Density = 150f;

            Microsoft.Xna.Framework.Matrix trans = Microsoft.Xna.Framework.Matrix.Identity *
                Microsoft.Xna.Framework.Matrix.CreateRotationX(MathHelper.ToRadians(world.rX)) *
                Microsoft.Xna.Framework.Matrix.CreateRotationY(MathHelper.ToRadians(world.rY)) *
                Microsoft.Xna.Framework.Matrix.CreateRotationZ(MathHelper.ToRadians(world.rZ));

            boxDesc.Dimensions = new StillDesign.PhysX.MathPrimitives.Vector3(
                (obb.Max.X - obb.Min.X) * world.sX * .5f, 
                (obb.Max.Y - obb.Min.Y) * world.sY * .5f, 
                (obb.Max.Z - obb.Min.Z) * world.sZ * .5f);

            Microsoft.Xna.Framework.Vector3 distOffset = ((obb.Max - obb.Min) * world.Scale) * 0.5f;
            distOffset.X = distOffset.Z = 0;
            
            Microsoft.Xna.Framework.Vector3 final = Microsoft.Xna.Framework.Vector3.Transform(distOffset, trans);
            
            boxDesc.LocalPose = StillDesign.PhysX.MathPrimitives.Matrix.Identity *
                StillDesign.PhysX.MathPrimitives.Matrix.RotationX(MathHelper.ToRadians(world.rX)) *
                StillDesign.PhysX.MathPrimitives.Matrix.RotationY(MathHelper.ToRadians(world.rY)) *
                StillDesign.PhysX.MathPrimitives.Matrix.RotationZ(MathHelper.ToRadians(world.rZ)) *
                StillDesign.PhysX.MathPrimitives.Matrix.Translation(world.X + final.X, world.Y + final.Y, world.Z + final.Z);

            actorDesc.Shapes.Add(boxDesc);
            if (!actorDesc.IsValid())
                throw new Exception("PhysXPrimitive::GetPlatformActor - Invalid Data Found");

            return engine.scene.CreateActor(actorDesc);
        }
    }
}
