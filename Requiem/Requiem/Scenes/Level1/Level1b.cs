using Engine.Managers;

namespace Requiem.Scenes.Level1
{
    public sealed class Level1b : LevelScene
    {
        /// <summary>Default CTOR</summary>
        public Level1b(Game g, string name, SceneManager sm)
            : base(g, name, sm) { }

        public override void Initialize()
        {
            base.SceneInitialize("content//data//Level1//Level1b.xml");
            isInitialized = true;
        }
    }
}
