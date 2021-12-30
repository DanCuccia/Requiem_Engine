using Engine.Math_Physics;
using Requiem.Spells.Base;
using Requiem.Entities;

namespace Requiem.Spells.RenderEffects
{
    /// <summary>This class is non-creatable and only has 1 static function to return the asked for projectile renderer</summary>
    /// <author>Daniel Cuccia</author>
    public abstract class SpellRendererFactory
    {
        /// <summary>Get the projectile renderer depending on enum arg</summary>
        /// <param name="p">projectile attached to renderer</param>
        /// <param name="t">type of renderer asked for</param>
        /// <returns>IProjectileRenderer object of enum type, or null type</returns>
        public static ISpellRenderer GetProjectileRenderer(Projectile p, SpellRendererType t)
        {
            switch (t)
            {
                case SpellRendererType.PROJ_LIGHTNINGTRAIL:
                    return new LightningProjectileRenderer(p);

                case SpellRendererType.PROJ_FIREBALL:
                    return new FireballRenderer(p);

                case SpellRendererType.PROJ_STRIKE:
                    return new StrikeRenderer(p);
            }
            return new NullProjectileRenderer(p);
        }

        /// <summary>Get the aoe burst type renderers</summary>
        /// <param name="world">pointer to the world this spawns at</param>
        /// <param name="t">type of renderer you want</param>
        /// <returns>burst renderer loaded and ready to go</returns>
        public static ISpellRenderer GetBurstRenderer(WorldMatrix world, SpellRendererType t)
        {
            switch (t)
            {
                case SpellRendererType.PROJ_WEB:
                    return new WebTrapRenderer(ref world);

                case SpellRendererType.PROJ_VAMPIRE:
                    return new VampireRenderer(ref world);
            }
            return new NullSpellRenderer(ref world);
        }

        /// <summary>Get the spell renderer depending on enum arg</summary>
        /// <param name="world">the world this spell renderer will be representing</param>
        /// <param name="t">the drawing type requested</param>
        /// <returns>constructed ready to go spell renderer</returns>
        public static ISpellRenderer GetSelfRenderer(Livable owner, SpellRendererType t)
        {
            WorldMatrix w = owner.WorldMatrix;
            switch (t)
            {
                case SpellRendererType.SELF_HEALING:
                    return new RegenerateRenderer(ref w, 70f);
            }
            return new NullSpellRenderer(ref w);
        }
    }
}
