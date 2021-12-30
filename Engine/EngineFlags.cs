using System;
using Engine.Managers;
using Engine.Managers.Camera;
using Engine.Managers.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Engine
{
    /// <summary>Major Gameplay Flags</summary>
    /// <author>Daniel Cuccia</author>
    public abstract class EngineFlags
    {
        /// <summary>the main game random seed</summary>
        public static Random            random              = new Random();

        /// <summary>draw any objects the game needs to help development</summary>
        public static bool              drawDevelopment     = false;

        /// <summary>draw all debugging information</summary>
        public static bool              drawDebug           = false;

        /// <summary>lower the screen resolution</summary>
        public static bool              runLowDef           = false;

        /// <summary>toggle all collision on/off</summary>
        public static bool              noCollision         = false;

        /// <summary>make level editor force draw pointlight bulbs</summary>
        public static bool              forceBulbs          = false;

        /// <summary>sort billboards from furthest to closest</summary>
        public static bool              sortBillboards      = false;

        /// <summary>Un-Creatable object</summary>
        private EngineFlags() { }
    }
}
