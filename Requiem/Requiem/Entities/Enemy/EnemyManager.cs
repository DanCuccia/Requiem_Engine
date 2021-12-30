using System.Collections.Generic;
using Engine.Drawing_Objects;
using Engine.Game_Objects;
using Engine.Managers.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Engine.Managers;
using Requiem.Scenes;

namespace Requiem.Entities.Enemy
{
    /// <summary>Enemy Manager is the singleton object who takes care of all enemies, and spawn points</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class EnemyManager
    {
        #region Singleton
        private static EnemyManager myInstance;
        private EnemyManager() { }
        public static EnemyManager Instance
        {
            get
            {
                if (myInstance == null)
                    myInstance = new EnemyManager();
                return myInstance;
            }
        }
        #endregion

        #region MemberVars

        public ContentManager content;

        List<Enemy> activeEnemies = new List<Enemy>();
        public List<Enemy> ActiveEnemyList { get { return activeEnemies; } }

        List<SpawnLocation> spawnLocations = new List<SpawnLocation>();
        public SceneManager sceneManager = null;

        #endregion

        #region Init
        /// <summary>default init called in game to get xna's contentManager</summary>
        /// <param name="content">xna content management</param>
        public void Initialize(ContentManager content, SceneManager sceneManager)
        {
            this.content = content;
            this.sceneManager = sceneManager;
        }

        /// <summary>load and prepare this manager from the level object,r eady for gameplay</summary>
        /// <param name="level">fully loaded level object</param>
        public void LoadFromLevel(Level level)
        {
            foreach (Object3D obj in level.enemySpawnPointList)
            {
                SpawnLocation loc = new SpawnLocation(obj as EnemySpawnLocation);
                this.spawnLocations.Add(loc);
            }
        }

        /// <summary>dispose of all enemies, and locations</summary>
        public void Release()
        {
            activeEnemies.Clear();
            spawnLocations.Clear();
        }
        #endregion

        #region API
        /// <summary>update all objects</summary>
        /// <param name="camera">camera reference used for culling</param>
        /// <param name="time">current game time</param>
        public void Update(ref Camera camera, GameTime time)
        {
            if (sceneManager != null)
            {
                if (sceneManager.GetCurrentScene().GetType() == typeof(Requiem.Scenes.Level1.Level1d))
                {
                    if (sceneManager.GetCurrentScene().animateOut != null)
                        return;
                }
            }
            foreach (SpawnLocation loc in spawnLocations)
            {
                loc.Update(ref camera, time);
            }
            foreach (Enemy e in activeEnemies)
            {
                e.Movement.Update(time);
                e.Update(ref camera, time);
                e.UpdateBoundingBox();
            }

            for (int ctr = 0; ctr < activeEnemies.Count; ctr++)
            {
                if (!activeEnemies[ctr].Alive)
                {
                    KillEnemy(activeEnemies[ctr]);
                    ctr--;
                }
            }
        }

        /// <summary>main draw call for all enemies</summary>
        public void Render3D()
        {
            foreach (Enemy e in activeEnemies)
                e.Render();
        }

        /// <summary>render debugging information to the screen</summary>
        public void RenderDebug3D()
        {
            foreach (Enemy e in activeEnemies)
                e.RenderDebug();
            foreach (SpawnLocation loc in spawnLocations)
                loc.RenderDebug();
        }

        /// <summary>each location has their own list, as well as the manager's master list, 
        /// enemies must be removed from both lists</summary>
        /// <param name="enemy">the object you wish to remove</param>
        public void KillEnemy(Enemy enemy)
        {
            if(enemy == null) return;

            if(enemy.Actor != null)
            {
                enemy.Actor.Dispose();
            }
            foreach (SpawnLocation loc in this.spawnLocations)
            {
                loc.KillEnemy(enemy);
            }
            activeEnemies.Remove(enemy);
        }
        #endregion
    }
}
