using Engine.Managers;

namespace Requiem.Scenes.Level1
{
    public sealed class Level1c : LevelScene
    {
        /// <summary>Default CTOR</summary>
        public Level1c(Game g, string name, SceneManager sm)
            : base(g, name, sm) { }

        public override void Initialize()
        {
            base.SceneInitialize("content//data//Level1//Level1c.xml");
            isInitialized = true;
        }
    }
}
