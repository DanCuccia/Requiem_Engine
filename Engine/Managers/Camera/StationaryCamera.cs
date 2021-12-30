using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Managers.Camera
{
    /// <summary>stationary camera behavior</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class StationaryCamera : ICamera
    {
        /// <summary>default ctor</summary>
        public StationaryCamera()
        { }
        /// <summary>unused</summary>
        public void Update(GameTime time)
        { }         
        /// <summary>unused</summary>
        public void Input(KeyboardState kb, MouseState ms)
        { }
    }
}
