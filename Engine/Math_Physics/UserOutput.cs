using System;
using StillDesign.PhysX;

namespace Engine.Math_Physics
{
    /// <summary>Used when creating a PhysX Core Object</summary>
    public class UserOutput : UserOutputStream
    {
        /// <summary>print physX message</summary>
        public override void Print(string message)
        {
            Console.WriteLine("PhysX: " + message);
        }
        /// <summary>print physX message</summary>
        public override AssertResponse ReportAssertionViolation(string message, string file, int lineNumber)
        {
            Console.WriteLine("PhysX: " + message);

            return AssertResponse.Continue;
        }
        /// <summary>print physX message</summary>
        public override void ReportError(ErrorCode errorCode, string message, string file, int lineNumber)
        {
            Console.WriteLine("PhysX: " + message);
        }
    }
}
