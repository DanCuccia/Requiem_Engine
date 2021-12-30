using Engine.Managers;

namespace Requiem.Scenes.Level1
{
    public sealed class Level1e : LevelScene
    {
        /// <summary>Default CTOR</summary>
        public Level1e(Game g, string name, SceneManager sm)
            : base(g, name, sm) { }

        public override void Initialize()
        {
            base.SceneInitialize("content//data//Level1//Level1e.xml");

            AudioManager.Instance.StopAll();
            AudioManager.Instance.ClearAll();
            AudioManager.Instance.LoadSound("audio//Harappa_edited", "level1_bgm");
            AudioManager.Instance.Play2DSound("level1_bgm");

            isInitialized = true;
        }

        public override void AssignTrigger(ref Engine.Game_Objects.Trigger trigger)
        {
            switch (trigger.ID)
            {
                case 1612: trigger.triggerCallback = PlayerDies; break;
                case 75: trigger.triggerCallback = finishLevel; break;
            }
        }

        private void finishLevel()
        {
            sceneManager.SetCurrentScene("level1d");
        }
    }
}
