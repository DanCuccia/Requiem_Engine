#region using

using System.Collections.Generic;
using Engine.Managers.Camera;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Requiem.Entities;
using Requiem.Entities.EntitySupport;
using Requiem.Spells.SpellEffects;
using Requiem.Spells.RenderEffects;
using Engine.Drawing_Objects;

#endregion using

namespace Requiem.Spells.Base
{
    #region targeting types

    /// <summary>
    /// An enumeration definning the way a spell is targeted
    /// </summary>
    public enum TargetingTypes
    {
        PROJECTILE,
        TOUCH,
        SELF,
        BURST, 
        UNSET
    };

    #endregion targeting types

    /// <summary></summary>
    /// <author> Gabrial Dubois </author>
    public abstract class Spell
    {
        #region variables

        protected List<SpellEffect> effects = null;
        protected Livable owner = null;
        protected SpellManager manager = null;
        protected TargetingTypes targetingType = TargetingTypes.UNSET;
        protected Timer coolDown = null;
        protected Sprite icon = null;
        protected bool active = false;
        protected int numOfExpired = 0;

        public ISpellRenderer SpellRenderer { set; get; }

        #endregion variables

        public Spell() { }

        #region interface

        public abstract void Initialize();
        public abstract void Cast();
        public abstract void Update(Camera camera, GameTime time);
        public abstract void Draw();
        public abstract void DrawDebug();
        public abstract bool CheckCollisions(Livable target);
        public abstract bool CheckCollisionsNoEffect(OrientedBoundingBox obb);

        #endregion interface

        #region accessors

        /// <summary>
        /// Get a reference to the effect list
        /// </summary>
        public List<SpellEffect> Effects
        {
            get { return effects; }
        }

        /// <summary>
        /// Get a reference to the spell manager
        /// </summary>
        public SpellManager Manager
        {
            get { return manager; }
        }

        /// <summary>
        /// Get a reference to the cool down timer
        /// </summary>
        public Timer CoolDown
        {
            get { return coolDown; }
        }

        /// <summary>
        /// Get or set the owner 
        /// </summary>
        public Livable Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        /// <summary>
        /// Get or set the targeting type
        /// </summary>
        public TargetingTypes TargetingType
        {
            get { return targetingType; }
            set { targetingType = value; }
        }

        /// <summary>
        /// Get or set active
        /// </summary>
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        /// <summary>
        /// Get or set the spell icon
        /// </summary>
        public Sprite Icon
        {
            get { return icon; }
            set { icon = value; }
        }

        #endregion accessors
    }
}
