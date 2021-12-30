using System.Collections.Generic;
using Engine.Game_Objects;
using Engine.Managers.Camera;
using Microsoft.Xna.Framework;
using Requiem.Scenes;
using Engine;

namespace Requiem.Entities.Enemy
{
    /// <summary>enemy manager uses this, a wrapped EnemySpawnLocation</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class SpawnLocation
    {
        #region MemberVars
        EnemyManager enemyManager = EnemyManager.Instance;

        /// <summary>the Object3D spawn location</summary>
        public EnemySpawnLocation location;

        /// <summary>this count is in milliseconds</summary>
        float timeSinceLast = 0f;

        /// <summary>spawning behavior</summary>
        public Engine.EnemyEnums.EnemySpawnCondition SpawnConditionType = 
            Engine.EnemyEnums.EnemySpawnCondition.SPAWN_MANUAL;

        /// <summary>what type of enemies come from this location</summary>
        public Engine.EnemyEnums.EnemySpawnLocationType SpawnEnemyType = 
            Engine.EnemyEnums.EnemySpawnLocationType.LOC_PLAYER_LOCAL_STANDARD;

        /// <summary>your good ol' general use "I can spawn" bool</summary>
        bool canSpawn = false;

        /// <summary>this is a list of enemies this spawner may have</summary>
        List<Enemy> myEnemyList = new List<Enemy>();

        /// <summary>Gets the list of enemies this location has spawned</summary>
        public List<Enemy> EnemyList
        { get { return myEnemyList; } }

        /// <summary>how long this location waits until spawning the next monster (if this is a timed spawner)</summary>
        public float WaitTime { set; get; }
        /// <summary>how many enemies this spawner can own at one time</summary>
        public int MaxEnemies { set; get; }
        #endregion

        #region API
        /// <summary>default ctor</summary>
        /// <remarks>unless otherwise changed, waitTime and maxEnemies are default values</remarks>
        /// <param name="loc">the spawn location recreated from xml</param>
        public SpawnLocation(EnemySpawnLocation loc)
        {
            this.MaxEnemies = loc.MaxEnemies;
            this.WaitTime = loc.MaxWaitMillies;
            this.location = loc;
            this.SpawnConditionType = loc.SpawnCondition;
            this.SpawnEnemyType = loc.SpawnType;
        }

        /// <summary>update the spawn location (so we know this is off camera or not)</summary>
        /// <param name="camera">camera reference</param>
        /// <param name="time">current game time</param>
        public void Update(ref Camera camera, GameTime time)
        {
            location.Update(ref camera, time);

            timeSinceLast += time.ElapsedGameTime.Milliseconds;

            decideToSpawn();

            if (canSpawn == true)
            {
                this.spawn();
                canSpawn = false;
                timeSinceLast = 0;
            }
        }

        /// <summary>wraps the decision logic for spawning enemies, canSpawn variable will be flipped</summary>
        private void decideToSpawn()
        {
            switch (this.SpawnConditionType)
            {
                //SPAWN PER TIMER
                case Engine.EnemyEnums.EnemySpawnCondition.SPAWN_TIMED:
                    if (timeSinceLast >= this.WaitTime)
                    {
                        if (this.myEnemyList.Count < this.MaxEnemies)
                        {
                            canSpawn = true;
                        }
                    }
                    break;

                //SPAWN WHEN OFF-SCREEN
                case Engine.EnemyEnums.EnemySpawnCondition.SPAWN_OFF_CAMERA:
                    if (timeSinceLast >= this.WaitTime && this.location.InsideFrustum == false)
                    {
                        if (this.myEnemyList.Count < this.MaxEnemies)
                        {
                            canSpawn = true;
                        }
                    }
                    break;

                case EnemyEnums.EnemySpawnCondition.SPAWN_MANUAL: canSpawn = false; break;
            }
        }

        /// <summary>spawn a new enemy and pass it to the manager</summary>
        private void spawn()
        {
            Enemy e = null;

            switch (this.SpawnEnemyType)
            {
                case EnemyEnums.EnemySpawnLocationType.LOC_DISTANCE_SHOOTING_STANDARD:
                    e = EnemyFactory.GetDistanceShooter(enemyManager.sceneManager.GetCurrentScene() as LevelScene, enemyManager.content, "models//andyAnims");
                    break;

                case EnemyEnums.EnemySpawnLocationType.LOC_PLAYER_LOCAL_STANDARD:
                    e = EnemyFactory.GetLocalStandardEnemy(enemyManager.sceneManager.GetCurrentScene() as LevelScene, enemyManager.content, "models//andyAnims");
                    break;

                case EnemyEnums.EnemySpawnLocationType.LOC_PLAYER_LOCAL_BOSS:
                    e = EnemyFactory.GetBossEnemy(enemyManager.sceneManager.GetCurrentScene() as LevelScene, enemyManager.content, "models//bossRoom//model//BroodMother");
                    break;
            }

            if (e != null)
            {
                e.WorldMatrix.Position = this.location.WorldMatrix.Position;
                e.WorldMatrix.Y += 50f;
                e.Initialize(enemyManager.content);
                e.Ai.BeginAutomation();
                this.myEnemyList.Add(e);
                enemyManager.ActiveEnemyList.Add(e);
            }
        }

        /// <summary>kills an enemy if found, and resets the timer</summary>
        /// <param name="e">the enemy you want to kill, its ok if it doesn't exist in this location</param>
        public void KillEnemy(Enemy e)
        {
            if (myEnemyList.Remove(e))
            {
                this.timeSinceLast = 0f;
            }
        }

        /// <summary>render the spawn location to screen</summary>
        public void RenderDebug()
        {
            location.Render();
            location.RenderDebug();
        }

        #endregion
    }
}
