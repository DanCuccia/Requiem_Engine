using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Engine.Math_Physics;
using Requiem.Entities;

namespace Requiem.Movement
{
    /// <summary>movement interface used for the "strike" spell</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class StrikeProjectileMovement : CManualMovement
    {
        float angle = 0f;
        Livable player;
        const float radius = 75f;
        const float rotationSpeed = 3.5f;

        /// <summary>Default ctor</summary>
        /// <param name="world">reference to the world this is relative to</param>
        public StrikeProjectileMovement(ref WorldMatrix world, Livable owner)
            : base(ref world)
        {
            this.player = owner;
        }

        /// <summary>Update position according to player's position</summary>
        /// <param name="time">game time</param>
        public override void Update(GameTime time)
        {
            angle += rotationSpeed;
            worldMatrix.X = player.WorldMatrix.X + ((float)Math.Sin(MathHelper.ToRadians(angle)) * radius);
            worldMatrix.Z = player.WorldMatrix.Z + ((float)Math.Cos(MathHelper.ToRadians(angle)) * radius);
            worldMatrix.Y = player.WorldMatrix.Y;
            worldMatrix.rY += rotationSpeed * 3f;
        }

        public override void Input(KeyboardState kb, MouseState ms, GamePadState gp) { }
        public override void Input(Keys key) { }
        public override void AddImpulse(Vector3 direction, float amount) { }
    }
}
