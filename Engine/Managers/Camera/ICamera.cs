using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Managers.Camera
{
    /// <summary>Camera behavior base interface</summary>
    /// <author>Daniel Cuccia</author>
    public interface ICamera
    {
        /// <summary>Update Logic is under stategy pattern</summary>
        /// <param name="time">current timing values</param>
        void Update(GameTime time);
        /// <summary>Input Logic is under stategy pattern</summary>
        /// <param name="kb">current keyboard state</param>
        /// <param name="ms">current mouse state</param>
        void Input(KeyboardState kb, MouseState ms);
    }
}
