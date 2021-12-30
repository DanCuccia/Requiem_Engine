using System;
using Engine.Drawing_Objects;
using Engine.Drawing_Objects.Materials;
using Engine.Managers;
using Engine.Managers.Camera;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#pragma warning disable 1591

namespace Engine.Game_Objects
{
    /// <summary>Enemy Location serializable</summary>
    [Serializable]
    public class EnemySpawnLocationXml : XMLMedium
    {
        /// <summary>world space data</summary>
        public Vector3 position;
        public int spawnType;
        public int spawnCondition;
        public int maxWait;
        public int maxEnemy;
    }

    /// <summary>Level editor uses this, and is translated into game code into the "SpawnLocation" object</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class EnemySpawnLocation : Object3D
    {
        #region MemberVars
        Quad3D drawable;
        public Engine.EnemyEnums.EnemySpawnLocationType SpawnType { set; get; }
        public Engine.EnemyEnums.EnemySpawnCondition SpawnCondition { set; get; }

        int maxWait = EnemySpawnLocation.MinWait;
        public int MaxWaitMillies
        {
            set { maxWait = value; }
            get { return maxWait; }
        }
        int maxEnemy = EnemySpawnLocation.MinEnemy;
        public int MaxEnemies
        {
            set { maxEnemy = value; }
            get { return maxEnemy; }
        }

        public static int MinWait = 2000;
        public static int MaxWait = 10000;
        public static int MinEnemy = 1;
        public static int MaxEnemy = 5;
        #endregion

        #region Init
        /// <summary>ctor</summary>
        public EnemySpawnLocation() 
        {
            drawable = new Quad3D(Quad3DOrigin.BOTTOM_CENTER);
            SpawnType = EnemyEnums.EnemySpawnLocationType.LOC_PLAYER_LOCAL_STANDARD;
            SpawnCondition = EnemyEnums.EnemySpawnCondition.SPAWN_MANUAL;
        }

        /// <summary>load assets</summary>
        /// <param name="content">xna's content manager</param>
        public void Initialize(ContentManager content)
        {
            drawable.Initialize(TextureManager.getInstance().GetTexture("sprites//editor//LE_enemySpawnPoint"));
        }

        /// <summary>hardcoded obb to a 50f box</summary>
        public override void GenerateBoundingBox()
        {
            this.OBB = new OrientedBoundingBox(new Vector3(-25, -25, -25), new Vector3(25, 25, 25));
        }

        #endregion

        #region API
        /// <summary>update stuff</summary>
        /// <param name="camera">camera reference for frustum culling</param>
        /// <param name="time">current game time</param>
        public override void Update(ref Camera camera, GameTime time)
        {
            if (camera == null)
                return;

            if (drawable != null)
            {
                drawable.WorldMatrix.rY = 180 + MathHelper.ToDegrees((float)Math.Atan2(drawable.WorldMatrix.X - camera.Position.X,
                                drawable.WorldMatrix.Z - camera.Position.Z));
                drawable.WorldMatrix.Position = this.WorldMatrix.Position;
                drawable.Update(ref camera, time);
            }
        }

        /// <summary>update the OBB</summary>
        public override void UpdateBoundingBox()
        {
            if(OBB != null)
                this.OBB.Update(this.WorldMatrix.GetWorldMatrix());
        }

        /// <summary>renders all drawables</summary>
        public override void Render()
        {
            if (drawable != null)
                drawable.Render();
            if (OBB != null)
                OBB.Render();
        }

        /// <summary>unused - obb draws by default</summary>
        public override void RenderDebug() { }

        /// <summary>unused, this object contains drawables</summary>
        /// <param name="effect">current fx</param>
        public override void RenderImplicit(Effect effect) { }
        #endregion

        #region Serialization
        /// <summary>get the xml values needed for gameside</summary>
        /// <returns>serialization class</returns>
        public override XMLMedium GetXML()
        {
            EnemySpawnLocationXml output = new EnemySpawnLocationXml();
            output.position = this.WorldMatrix.Position;
            output.spawnType = (int)this.SpawnType;
            output.spawnCondition = (int)this.SpawnCondition;
            output.maxWait = this.MaxWaitMillies;
            output.maxEnemy = this.MaxEnemies;
            return output;
        }

        /// <summary>Get a copy of this object, with correct references</summary>
        /// <param name="content">xna's content manager</param>
        /// <returns>copy of this object</returns>
        public override Object3D GetCopy(ContentManager content)
        {
            EnemySpawnLocation output = new EnemySpawnLocation();
            output.Initialize(content);
            output.GenerateBoundingBox();
            output.SpawnType = this.SpawnType;
            output.SpawnCondition = this.SpawnCondition;
            output.MaxEnemies = this.MaxEnemies;
            output.MaxWaitMillies = this.MaxWaitMillies;
            return output;
        }

        /// <summary>re-create this object from xml</summary>
        /// <param name="content">xna's content manager</param>
        /// <param name="inputXml">de-serialized object from xml</param>
        public override void CreateFromXML(ContentManager content, XMLMedium inputXml)
        {
            EnemySpawnLocationXml input = inputXml as EnemySpawnLocationXml;
            this.Initialize(content);
            this.WorldMatrix.Position = input.position;
            this.SpawnType = (Engine.EnemyEnums.EnemySpawnLocationType)input.spawnType;
            this.SpawnCondition = (Engine.EnemyEnums.EnemySpawnCondition)input.spawnCondition;
            this.MaxWaitMillies = input.maxWait;
            this.maxEnemy = input.maxEnemy;
        }
        #endregion

        public override Material Material
        {
            get { return drawable.Material; }
            set { drawable.Material = value; }
        }
    }
}
