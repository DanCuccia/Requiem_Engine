using Engine.Game_Objects;
using Engine.Managers;

namespace Requiem.Scenes.Level1
{
    public sealed class Level1a : LevelScene
    {
        /// <summary>Default CTOR</summary>
        public Level1a(Game g, string name, SceneManager sm)
            : base(g, name, sm) { }

        public override void Initialize()
        {
            base.SceneInitialize("content//data//Level1//Level1a.xml");
            isInitialized = true;
        }

        public override sealed void AssignTrigger(ref Trigger trigger)
        {
            switch (trigger.ID)
            {
                case 955: trigger.triggerCallback = PlayerDies;
                    break;

                case 1868: trigger.triggerCallback = cb_gotoDev;
                    break;
            }
        }

        private void cb_gotoDev()
        {
            sceneManager.SetCurrentScene("dev");
        }
    }
}
