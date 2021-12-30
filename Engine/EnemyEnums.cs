#pragma warning disable 1591
namespace Engine
{
    /// <summary>enum stash for enemies, which need to be in both engine and game, ugly I know...</summary>
    /// <author>Daniel Cuccia</author>
    public abstract class EnemyEnums
    {
        private EnemyEnums() { }

        /// <summary>describes the different options enemy spawn locations behave</summary>
        /// <author>Daniel Cuccia</author>
        public enum EnemySpawnCondition
        {
            SPAWN_OFF_CAMERA,
            SPAWN_TIMED,
            SPAWN_MANUAL,
            COUNT
        }

        /// <summary>this describes what type of enemy AI will be spawned from 'this' location</summary>
        /// <author>Daniel Cuccia</author>
        public enum EnemySpawnLocationType
        {
            LOC_PLAYER_LOCAL_STANDARD,
            LOC_PLAYER_LOCAL_BOSS,
            LOC_DISTANCE_BOSS,
            LOC_DISTANCE_MOVING_STANDARD,
            LOC_DISTANCE_SHOOTING_STANDARD,
            COUNT
        }
    }
}
