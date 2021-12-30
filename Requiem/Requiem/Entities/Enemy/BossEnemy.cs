#region Using
using Microsoft.Xna.Framework;
using Engine.Managers.Camera;
using Microsoft.Xna.Framework.Content;
#endregion Using

namespace Requiem.Entities.Enemy
{
    /// <summary></summary>
    /// <author>Gabrial Dubois</author>
    public class BossEnemy : Enemy
    {
        public BossEnemy(string filename)
            :base(filename)
        {
            health = new EntitySupport.Health(100);
        }
    }
}
