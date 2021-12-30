using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Requiem.Movement
{
    /// <summary>Base interface for different movement logics</summary>
    /// <author>Daniel Cuccia</author>
    public interface IMovement
    {
        void Input(KeyboardState kb, MouseState ms, GamePadState gp);
        void Input(Keys key);
        void Update(GameTime time);
        void ForceReset();
        void ModifySpeed(float scalar);
        void AddImpulse(Vector3 direction, float amount);
        Vector3 Direction();
    }
}
