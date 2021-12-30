using Microsoft.Xna.Framework;
using Engine.Managers.Camera;
using Microsoft.Xna.Framework.Content;

namespace Requiem.Entities.Enemy
{
    /// <summary></summary>
    /// <author>Gabrial Dubois</author>
    public class StandardEnemy : Enemy
    {
        public StandardEnemy(string filename)
            :base(filename)
        {
            health = new EntitySupport.Health(50);
        }
    }
}