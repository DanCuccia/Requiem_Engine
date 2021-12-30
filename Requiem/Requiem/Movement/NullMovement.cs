using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StillDesign.PhysX;

namespace Requiem.Movement
{
    /// <summary>Default Null movement logic</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class NullMovement : CMovement
    {
        public NullMovement(ref WorldMatrix world, ref Actor playerActor)
            :base(ref world, ref playerActor)
        { }
        public override void Input(KeyboardState kb, MouseState ms, GamePadState gp) { }
        public override void Update(GameTime time) { }
        public override void ForceReset() { }
        public override void AddImpulse(Vector3 direction, float amount) { }
        public override void Input(Keys key) { }
    }
}
