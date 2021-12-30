using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Engine.Math_Physics;
using Engine.Drawing_Objects.Materials;
using Engine.Managers;

namespace Engine.Drawing_Objects.ParticleSystems
{
    /// <summary>This is a 3D sphere model used for mega-particle techniques</summary>
    public class MegaParticle : Particle
    {
        private StaticObject3D sphere = new StaticObject3D();

        /// <summary>default CTOR</summary>
        /// <param name="content">this contains a staticObject3D, which needs contentmanager to load the filepath</param>
        /// <param name="instructionCount">how many instructions this particle is assigned to</param>
        public MegaParticle(ContentManager content, int instructionCount)
            : base(instructionCount)
        {
            sphere.Initialize(content, "particle//megaSphere");
            sphere.Material.ID = GameIDList.Shader_MegaParticle;
        }

        /// <summary>draws the mega particle sphere</summary>
        public override void Render()
        {
            sphere.Render();
        }

        /// <summary>Main Particle Drawable accessor</summary>
        public virtual StaticObject3D MegaSphere
        {
            get { return this.sphere; }
        }
        /// <summary>Overriden to the drawable's worldMatrix</summary>
        public override WorldMatrix WorldMatrix
        {
            get {  return sphere.WorldMatrix; }
            set { sphere.WorldMatrix = value; }
        }
        /// <summary>Overriden to the drawable's material</summary>
        public override Color Color
        {
            get { return Color.FromNonPremultiplied((sphere.Material as NullMaterial).color); }
            set { (sphere.Material as NullMaterial).color = value.ToVector4(); }
        }
        /// <summary>alpha of the particle</summary>
        public override float Alpha
        {
            get { return (float)(Color.A * 255); }
            set { (sphere.Material as NullMaterial).color.W = value; }
        }
    }
}
