using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Engine.Game_Objects.PlatformBehaviors;
using Engine.Game_Objects;

namespace Engine.Managers.Factories
{
    /// <summary>This un-constructable object holds static methods to create platform behaviors from xml</summary>
    public abstract class PlatformBehaviorFactory
    {
        /// <summary>Unconstructable</summary>
        private PlatformBehaviorFactory() { }

        /// <summary>the factory method that takes a platformBehaviorXml, depicts what it is, and recreates it</summary>
        /// <param name="platformXml">IPlatformBehaviorXML</param>
        /// <param name="platformObj">the actual platform object</param>
        /// <returns>IPlatofrm Behavior</returns>
        public static IPlatform GetPlatformBehavior(PlatformBehaviorXML platformXml, Platform platformObj)
        {
            IPlatform output = null;

            if (platformXml == null)
                return null;
            if(platformObj == null)
                return null;

            if (platformXml.GetType() == typeof(StationaryBehaviorXml))
            {
                output = new IStationaryPlatform(platformObj);
            }
            if (platformXml.GetType() == typeof(HorizontalBehaviorXml))
            {
                output = new IHorizontalPlatform(platformObj);
            }
            else if (platformXml.GetType() == typeof(VerticalBehaviorXml))
            {
                output = new IVerticalPlatform(platformObj);
            }
            else if (platformXml.GetType() == typeof(RotatingBehaviorXml))
            {
                output = new IRotatingPlatform(platformObj);
            }

            if (output != null)
                output.FromXml(platformXml);

            return output;
        }
    }
}
