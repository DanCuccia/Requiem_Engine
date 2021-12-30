using Microsoft.Xna.Framework.Content;

namespace Engine.Drawing_Objects.ParticleSystems
{
    /// <summary>specific particle for opening fire emitter</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class MegaFireParticle : MegaParticle
    {
        /// <summary>the maximum scale this particle was assigned</summary>
        public float MaxScale = 1f;

        /// <summary>ctor</summary>
        /// <param name="content">xna content manater</param>
        /// <param name="instructionCount">how many instructions</param>
        public MegaFireParticle(ContentManager content, int instructionCount)
            : base(content, instructionCount) { }
    }
}
