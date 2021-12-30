using Engine.Managers;
using Microsoft.Xna.Framework;
using Requiem.Entities.Enemy;
using Engine.Scenes.IScene_Animations;
using Requiem.Spells.Base;
using System;

namespace Requiem.Scenes.Level1
{
    public sealed class Level1d : LevelScene
    {
        float currentCount = 0f;
        float maxCount = 15000f;
        float playerToBossDist = 700f;

        BossEnemy boss;

        /// <summary>Default CTOR</summary>
        public Level1d(Game g, string name, SceneManager sm)
            : base(g, name, sm) { }

        public override void Initialize()
        {
            base.SceneInitialize("content//data//Level1//Level1d.xml");
            renderer.BloomSettings.BaseSaturation = 0.65f;

            AudioManager a = AudioManager.Instance;
            a.StopAll();
            a.ClearAll();
            a.LoadSound("audio//Pompeii", "boss");
            a.Play2DSound("boss");
            
            currentCount = 0f;

            isInitialized = true;
        }

        public override void Close()
        {
            renderer.BloomSettings = BloomSettings.PresetSettings[(int)BloomPresetIndices.BLOOM_SUBTLE];
            currentCount = 0f;
            base.Close();
        }

        public override void Update(GameTime time)
        {
            base.Update(time);

            if(boss == null)
            {
                foreach(Enemy e in enemyManager.ActiveEnemyList)
                {
                    if(e.GetType() == typeof(BossEnemy))
                    {
                        boss = e as BossEnemy;
                        break;
                    }
                }
            }

            if (boss != null)
            {
                if (Math.Abs(Vector3.Distance(player.WorldMatrix.Position, boss.WorldMatrix.Position)) <= playerToBossDist)
                {
                    currentCount += time.ElapsedGameTime.Milliseconds;
                    if (currentCount >= maxCount)
                    {
                        if (animateOut == null)
                        {
                            boss.Drawable.BeginAnimation("dead", false, quitLevel);
                            boss.Drawable.AnimationSpeed = .5f;
                            animateOut = new BossSceneAnimOut(this, quitLevel);
                        }
                    }
                }
            }
        }

        private void quitLevel()
        {
            sceneManager.SetCurrentScene("main");
            AudioManager.Instance.StopAll();
            AudioManager.Instance.ClearAll();
            AudioManager.Instance.LoadSound("audio//AshielfPi", "music");
            AudioManager.Instance.Play2DSound("music");
            animateOut = null;
        }
    }
}
