using System;
using System.Collections.Generic;
using Engine.Drawing_Objects;
using Engine.Drawing_Objects.ParticleSystems;
using Engine.Managers;
using Engine.Managers.Camera;
using Engine.Managers.Factories;
using Engine.Game_Objects.PreFabs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Engine.Math_Physics;
using Microsoft.Xna.Framework.Graphics;
using StillDesign.PhysX;
using System.IO;
using System.Xml.Serialization;

namespace Engine.Game_Objects
{
    /// <summary>A serializable class containing all values needed to save and load this level</summary>
    [Serializable]
    public class LevelXml
    {
        /// <summary>name of level</summary>
        public string levelName = "";
        /// <summary>list of player collidable objects</summary>
        public List<XMLMedium> collidables = new List<XMLMedium>();
        /// <summary>list of doodad objects</summary>
        public List<XMLMedium> doodads = new List<XMLMedium>();
        /// <summary>list of triggers</summary>
        public List<XMLMedium> triggers = new List<XMLMedium>();
        /// <summary>list of lights</summary>
        public List<XMLMedium> lights = new List<XMLMedium>();
        /// <summary>list of all enemy spawning locations</summary>
        public List<XMLMedium> enemySpawnPoints = new List<XMLMedium>();
        /// <summary>beginning position</summary>
        public Vector3 startPosition;
    }

    /// <summary>This is a level, and contains everything saved from the level editor for run time</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class Level
    {
        #region MemberVariables

        /// <summary>name of level</summary>
        public string levelName = "New";
        /// <summary>list of player collidable objects</summary>
        public List<Object3D> collidableList = new List<Object3D>();
        /// <summary>list of doodad objects</summary>
        public List<Object3D> dooDadList = new List<Object3D>();
        /// <summary>list of triggers</summary>
        public List<Trigger> triggerList = new List<Trigger>();
        /// <summary>list of lights</summary>
        public List<PointLight> lightList = new List<PointLight>();
        /// <summary>list of all enemy spawning locations</summary>
        public List<Object3D> enemySpawnPointList = new List<Object3D>();
        /// <summary>beginning position</summary>
        public Vector3 currentSpawnLocation = Vector3.Zero;

        ContentManager content;
        Actor groundActor;

        /// <summary>The main running PhysX Engine</summary>
        public PhysXEngine physicsEngine;

        #endregion MemberVariables

        #region Init
        /// <summary>Initialize</summary>
        public void Initialize(ContentManager content, GraphicsDevice device, ref Camera camera)
        {
            this.content = content;
            physicsEngine = PhysXEngine.Instance;
            physicsEngine.Camera = camera;
        }

        /// <summary>Release all objects for garbage cleaning</summary>
        public void Release()
        {
            collidableList.Clear(); collidableList = null;
            dooDadList.Clear(); dooDadList = null;
            triggerList.Clear(); triggerList = null;
            Renderer.getInstance().UnRegisterLightList();
            lightList.Clear(); lightList = null;
            physicsEngine.Release();
        }

        #endregion Init

        #region API

        /// <summary>Major Render3D call</summary>
        public void Render3D()
        {
            foreach (Object3D obj in collidableList)
                obj.Render();
            foreach (Object3D obj in dooDadList)
                obj.Render();
            if(EngineFlags.drawDevelopment == true)
                foreach (Trigger trig in triggerList)
                    trig.Render();
            foreach (PointLight light in lightList)
                light.Render();
        }

        /// <summary>Calls RenderDebug on all items within level</summary>
        public void RenderDebug3D()
        {
            foreach (Object3D obj in collidableList)
                obj.RenderDebug();
            foreach (Object3D obj in dooDadList)
                obj.RenderDebug();
            if (EngineFlags.drawDevelopment == true)
                foreach (Trigger trig in triggerList)
                    trig.RenderDebug();
            foreach (PointLight light in lightList)
                light.RenderDebug();
        }

        /// <summary>Render the PhysX data to screen</summary>
        public void RenderDebugPhysX()
        {
            physicsEngine.Render3D();
        }

        /// <summary>Calls the Object3D base class update on all objects in level</summary>
        /// <param name="time">current game timing values</param>
        /// <param name="camera">camera reference used for culling</param>
        public void Update(ref Camera camera, GameTime time)
        {
            foreach (Object3D obj in collidableList)
            {
                obj.Update(ref camera, time);
                obj.UpdateBoundingBox();
            }
            foreach (Object3D obj in dooDadList)
            {
                obj.Update(ref camera, time);
                obj.UpdateBoundingBox();
            }
            foreach (Trigger trig in triggerList)
            {
                trig.Update(ref camera, time);
                trig.UpdateBoundingBox();
            }
            foreach (PointLight light in lightList)
            {
                light.Update(ref camera, time);
                light.UpdateBoundingBox();
            }
            physicsEngine.Update(time);
        }

        /// <summary>because the same algorithm is used to determine both OBB and boundingSphere,
        /// We generate and update the bounding information normally, and dump the unused OBB.
        /// This is in-efficient I know, but frustum culling wasn't implemented until late
        /// TODO!</summary>
        public void GenerateFrustumBounding()
        {
            foreach (Object3D obj in dooDadList)
            {
                obj.GenerateBoundingBox();
                obj.UpdateBoundingBox();
                obj.OBB = null;
                if (obj.Actor != null)
                {
                    obj.Actor.Dispose(); 
                    obj.Actor = null;
                }
            }
            foreach (Object3D obj in lightList)
            {
                obj.GenerateBoundingBox();
                obj.UpdateBoundingBox();
                obj.OBB = null;
                if (obj.Actor != null)
                {
                    obj.Actor.Dispose(); 
                    obj.Actor = null;
                }
            }
        }

        /// <summary>Test the given obb by (all - temp) triggers in the scene</summary>
        /// <returns>true if a collision was found</returns>
        /// <param name="testee">given obb</param>
        public bool TestTriggers(OrientedBoundingBox testee)
        {
            if (testee == null)
                return false;

            bool result = false;
            foreach (Trigger t in triggerList)
            {
                if (t.OBB.Intersects(testee))
                {
                    t.Execute();
                    result = true;
                    break;
                }
            }
            return result;
        }

        #endregion API

        #region Building

        /// <summary>Get the serializable class of this level, containing all information for all objects</summary>
        /// <returns>the full xml of our level</returns>
        public LevelXml GetLevelXml()
        {
            LevelXml output = new LevelXml();
            output.levelName = levelName;
            output.startPosition = currentSpawnLocation;

            foreach (Object3D obj in collidableList)
                output.collidables.Add(obj.GetXML());
            foreach (Object3D obj in dooDadList)
                output.doodads.Add(obj.GetXML());
            foreach (Trigger obj in triggerList)
                output.triggers.Add(obj.GetXML());
            foreach (PointLight light in lightList)
                output.lights.Add(light.GetXML());
            foreach (Object3D obj in enemySpawnPointList)
                output.enemySpawnPoints.Add(obj.GetXML());

            return output;
        }

        /// <summary>Load the level from file, all xml deserialization is done in here</summary>
        /// <param name="filepath">filepath to xml in content</param>
        public void LoadFromFile(string filepath)
        {
            LevelXml levelInput = new LevelXml();
            TextReader reader = new StreamReader(filepath);
            XmlSerializer xml = new XmlSerializer(typeof(LevelXml));
            levelInput = (LevelXml)xml.Deserialize(reader);
            reader.Close();
            this.LoadLevel(levelInput);
            this.GenerateFrustumBounding();
        }

        /// <summary>Load the level from a pre-deserialized object</summary>
        /// <param name="input">deserialized object containing all needed values to build a level</param>
        public void LoadLevel(LevelXml input)
        {
            this.triggerList.Clear();
            this.dooDadList.Clear();
            this.collidableList.Clear();
            this.lightList.Clear();
            this.enemySpawnPointList.Clear();

            buildCollidables(input.collidables);
            buildDoodads(input.doodads);
            buildTriggers(input.triggers);
            buildLights(input.lights);
            buildEnemySpawnPoints(input.enemySpawnPoints);
            Renderer.getInstance().RegisterLightList(ref this.lightList);
            this.currentSpawnLocation = input.startPosition;

            groundActor = PhysXPrimitive.GetGroundPlaneActor();
        }

        /// <summary>Take the list of the levelXML and build the collidables from it</summary>
        /// <param name="collidables">deserialized list of collidables</param>
        private void buildCollidables(List<XMLMedium> collidables)
        {
            Object3D obj = null;
            foreach (XMLMedium input in collidables)
            {
                if (input.GetType() == typeof(StaticObject3DXML))
                {
                    obj = new StaticObject3D();
                    obj.CreateFromXML(content, input);
                }
                else if (input.GetType() == typeof(AnimatedObject3DXML))
                {
                    obj = new AnimatedObject3D();
                    obj.CreateFromXML(content, input);
                }
                else if (input.GetType() == typeof(QuadXml))
                {
                    obj = new Quad3D((Quad3DOrigin)(input as QuadXml).origin);
                    obj.CreateFromXML(content, input);
                }
                else if (input.GetType() == typeof(PreFabricationXML))
                {
                    obj = PreFabFactory.GetPreFabrication(content, input);
                }
                else if (input.GetType() == typeof(PlatformXml))
                {
                    obj = new Platform();
                    obj.CreateFromXML(content, input);
                }
                if (obj != null)
                {
                    obj.GenerateBoundingBox();
                    obj.Actor = PhysXPrimitive.GetActor(obj.OBB, obj.WorldMatrix);
                    obj.Collidable = true;
                    collidableList.Add(obj);
                }
                
            }
        }

        /// <summary>Take the list of the levelXML and build the dooDad props from it</summary>
        /// <param name="props">deserialized list of doodads</param>
        private void buildDoodads(List<XMLMedium> props)
        {
            Object3D obj = null;
            foreach (XMLMedium input in props)
            {
                if (input.GetType() == typeof(ParticleSystemXML))
                {
                    obj = getParticleSystem(input as ParticleSystemXML);
                }
                if (input.GetType() == typeof(StaticObject3DXML))
                {
                    obj = new StaticObject3D();
                    obj.CreateFromXML(content, input);
                }
                else if (input.GetType() == typeof(AnimatedObject3DXML))
                {
                    obj = new AnimatedObject3D();
                    obj.CreateFromXML(content, input);
                }
                else if (input.GetType() == typeof(QuadXml))
                {
                    obj = new Quad3D((Quad3DOrigin)(input as QuadXml).origin);
                    obj.CreateFromXML(content, input);
                }
                else if (input.GetType() == typeof(PreFabricationXML))
                {
                    obj = PreFabFactory.GetPreFabrication(content, input);
                }
                if (obj != null)
                {
                    dooDadList.Add(obj);
                }
            }
        }

        /// <summary>take the deserialized list and create the objects from it</summary>
        /// <param name="enemySpawnList">list of enemy spawn points from xml</param>
        private void buildEnemySpawnPoints(List<XMLMedium> enemySpawnList)
        {
            if (enemySpawnList == null)
                return;

            foreach (XMLMedium input in enemySpawnList)
            {
                EnemySpawnLocation s = new EnemySpawnLocation();
                s.CreateFromXML(content, input);
                this.enemySpawnPointList.Add(s);
            }
        }

        /// <summary>particle systems need to construct differently</summary>
        /// <param name="inputXml">input particle system xml object</param>
        /// <returns>fully built particle system</returns>
        /// <remarks>may return null</remarks>
        private ParticleSystem getParticleSystem(ParticleSystemXML inputXml)
        {
            ParticleSystem output = null;

            switch (inputXml.id)
            {
                case GameIDList.PS_MegaParticle_Smoke:
                    output = new SmokeEmitter(content);
                    break;
                case GameIDList.PS_BillBoardParticle_Tornado:
                    output = new TornadoGlitterEmitter(content);
                    break;
                case GameIDList.PS_BillBoardParticle_Glitter:
                    output = new GlitterEmitter(content);
                    break;
            }

            if(output != null)
                output.CreateFromXML(content, inputXml);

            return output;
        }

        /// <summary>Take the list of the levelXML and build the Triggers from it</summary>
        /// <param name="triggers">deserialized list of triggers</param>
        private void buildTriggers(List<XMLMedium> triggers)
        {
            TriggerFactory factory = TriggerFactory.getInstance();
            foreach (TriggerXML input in triggers)
            {
                this.triggerList.Add(factory.GetTrigger(input));
            }
        }

        /// <summary>Build the list of lights from deserialized light xml structures</summary>
        /// <param name="lights">light list</param>
        private void buildLights(List<XMLMedium> lights)
        {
            if (lights == null)
                return;
            if (lights.Count == 0)
                return;

            foreach (XMLMedium light in lights)
            {
                PointLight input = new PointLight();
                input.CreateFromXML(content, (PointLightXML)light);
                lightList.Add(input);
            }
        }

        #endregion

        #region Level Editing

        /// <summary>Level editor adds objects through here</summary>
        /// <param name="obj">the object that is being added to the world</param>
        public void AddFromLevelEditor(Object3D obj)
        {
            if (obj == null)
                return;

            if (obj.GetType() == typeof(PFSpawnPoint) &&
                currentSpawnLocation == Vector3.Zero)
            {
                currentSpawnLocation = obj.WorldMatrix.Position;
            }

            if (obj.GetType() == typeof(EnemySpawnLocation))
            {
                foreach (EnemySpawnLocation loc in enemySpawnPointList)
                {
                    if (loc == obj)
                        return;
                }
                enemySpawnPointList.Add(obj);
            }

            if (obj.GetType() == typeof(PointLight))
            {
                foreach (PointLight light in lightList)
                {
                    if (light == obj)
                        return;
                }
                lightList.Add((PointLight)obj);
                return;
            }

            if (obj.GetType() == typeof(Trigger))
            {
                foreach (Trigger trigger in triggerList)
                {
                    if (trigger == obj)
                        return;
                }
                triggerList.Add((Trigger)obj);
                return;
            }

            if (obj.Collidable == true)
            {
                foreach (Object3D drawable in collidableList)
                {
                    if (drawable == obj)
                        return;
                }
                collidableList.Add(obj);
                return;
            }
            else
            {
                foreach (Object3D drawable in dooDadList)
                {
                    if (drawable == obj)
                        return;
                }
                dooDadList.Add(obj);
                return;
            }
        }

        /// <summary>level editor removes objects through here</summary>
        /// <param name="removal">the object that wants to be removed</param>
        public void DeleteFromEditor(Object3D removal)
        {
            if (removal.GetType() == typeof(PFSpawnPoint))
            {
                currentSpawnLocation = Vector3.Zero;
                foreach (Object3D obj in collidableList)
                {
                    if (obj == removal)
                    {
                        continue;
                    }
                    if (obj.GetType() == typeof(PFSpawnPoint))
                    {
                        currentSpawnLocation = obj.WorldMatrix.Position;
                        break;
                    }
                }
            }

            if (removal.GetType() == typeof(EnemySpawnLocation))
            {
                enemySpawnPointList.Remove(removal);
                return;
            }

            if (removal.GetType() == typeof(Trigger))
            {
                triggerList.Remove((Trigger)removal);
                return;
            }
            else if (removal.GetType() == typeof(PointLight))
            {
                lightList.Remove((PointLight)removal);
                return;
            }
            else
            {
                collidableList.Remove(removal);
                dooDadList.Remove(removal);
            }
        }

        /// <summary>Randomize a trigger ID checking for ID collision</summary>
        /// <returns>random trigger ID</returns>
        public int GetNextTriggerID()
        {
            int output = -1;
            while (output == -1)
            {
                int generatedID = EngineFlags.random.Next(0, 2048);
                bool collisionFound = false;
                foreach (Trigger trig in triggerList)
                {
                    if (trig.ID == generatedID)
                        collisionFound = true;
                }
                if (collisionFound == false)
                    output = generatedID;
            }
            return output;
        }

        /// <summary>Level editor needs everything to have bounding information for picking</summary>
        public void GenerateAllBounding()
        {
            foreach (Trigger trigger in triggerList)
            {
                trigger.GenerateBoundingBox();
                trigger.UpdateBoundingBox();
            }
            foreach (Object3D obj in collidableList)
            {
                obj.GenerateBoundingBox();
                obj.UpdateBoundingBox();
            }
            foreach (Object3D obj in dooDadList)
            {
                obj.GenerateBoundingBox();
                obj.UpdateBoundingBox();
            }
            foreach (Object3D obj in lightList)
            {
                obj.GenerateBoundingBox();
                obj.UpdateBoundingBox();
            }
            foreach (Object3D obj in enemySpawnPointList)
            {
                obj.GenerateBoundingBox();
                obj.UpdateBoundingBox();
            }
        }

        #endregion Level Editing
    }
}
