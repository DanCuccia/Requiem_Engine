using System;
using System.Collections.Generic;
using Engine.Drawing_Objects;
using Engine.Managers.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Engine.Math_Physics
{
    /// <summary>For all of those little algorithms that can be used anywhere</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class MyUtility
    {
        /// <summary>Check every button on the gamepad to see if any are down</summary>
        /// <param name="gp">gamepad state to check</param>
        /// <returns>amount of buttons pressed down</returns>
        public static int GamePadPressedKeyCount(GamePadState gp)
        {
            int output = 0;

            if (gp.IsButtonDown(Buttons.A))output++;
            if (gp.IsButtonDown(Buttons.B))output++;
            if (gp.IsButtonDown(Buttons.Back))output++;
            if (gp.IsButtonDown(Buttons.BigButton))output++;
            if (gp.IsButtonDown(Buttons.DPadDown))output++;
            if (gp.IsButtonDown(Buttons.DPadLeft))output++;
            if (gp.IsButtonDown(Buttons.DPadRight))output++;
            if (gp.IsButtonDown(Buttons.DPadUp))output++;
            if (gp.IsButtonDown(Buttons.LeftShoulder))output++;
            if (gp.IsButtonDown(Buttons.LeftStick))output++;
            if (gp.IsButtonDown(Buttons.LeftThumbstickDown))output++;
            if (gp.IsButtonDown(Buttons.LeftThumbstickLeft))output++;
            if (gp.IsButtonDown(Buttons.LeftThumbstickRight))output++;
            if (gp.IsButtonDown(Buttons.LeftThumbstickUp))output++;
            if (gp.IsButtonDown(Buttons.LeftTrigger))output++;
            if (gp.IsButtonDown(Buttons.RightShoulder))output++;
            if (gp.IsButtonDown(Buttons.RightStick))output++;
            if (gp.IsButtonDown(Buttons.RightThumbstickDown))output++;
            if (gp.IsButtonDown(Buttons.RightThumbstickLeft))output++;
            if (gp.IsButtonDown(Buttons.RightThumbstickRight))output++;
            if (gp.IsButtonDown(Buttons.RightThumbstickUp))output++;
            if (gp.IsButtonDown(Buttons.RightTrigger))output++;
            if (gp.IsButtonDown(Buttons.Start))output++;
            if (gp.IsButtonDown(Buttons.X))output++;
            if (gp.IsButtonDown(Buttons.Y))output++;

            return output;
        }
        /// <summary>get the current mouse ray according to the elaborte game-gam</summary>
        /// <param name="viewport">the current viewport</param>
        /// <param name="camera">the game-cam</param>
        /// <returns>current mouse ray</returns>
        public static Ray GetMouseRay(Viewport viewport, Camera camera)
        {
            if (camera == null)
                return new Ray();

            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            Vector3 nearPoint = new Vector3(mousePosition, 0);
            Vector3 farPoint = new Vector3(mousePosition, 1);

            nearPoint = viewport.Unproject(nearPoint, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
            farPoint = viewport.Unproject(farPoint, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            return new Ray(nearPoint, direction);
        }

        /// <summary>Dump excetion to the console</summary>
        /// <param name="ex">exception to dump</param>
        public static void DumpException(Exception ex)
        {
            Console.WriteLine("--------- Outer Exception Data ---------");
            WriteExceptionInfo(ex);
            ex = ex.InnerException;
            if (null != ex)
            {
                Console.WriteLine("--------- Inner Exception Data ---------");
                WriteExceptionInfo(ex.InnerException);
                ex = ex.InnerException;
            }
        }

        /// <summary>output specific information to the console</summary>
        /// <param name="ex">exception to display</param>
        public static void WriteExceptionInfo(Exception ex)
        {
            Console.WriteLine("Message: {0}", ex.Message);
            Console.WriteLine("Exception Type: {0}", ex.GetType().FullName);
            Console.WriteLine("Source: {0}", ex.Source);
            Console.WriteLine("StrackTrace: {0}", ex.StackTrace);
            Console.WriteLine("TargetSite: {0}", ex.TargetSite);
        }

        /// <summary>Iterate and run all standard button logic</summary>
        /// <param name="ms">current mouse state</param>
        /// <param name="btnList">the button list you wish to process</param>
        public static void ProcessButtonList(MouseState ms, List<SpriteButton> btnList)
        {
            if (ms == null || btnList == null)
                return;

            switch (ms.LeftButton)
            {
                case ButtonState.Pressed:
                    foreach (SpriteButton btn in btnList)
                        if (MyMath.IsWithin(ms.X, ms.Y, btn.GetBoundingBox()))
                        {
                            btn.OnPress();
                            return;
                        }
                        else btn.ToDefault();
                    break;

                case ButtonState.Released:
                    foreach (SpriteButton btn in btnList)
                        if (MyMath.IsWithin(ms.X, ms.Y, btn.GetBoundingBox()))
                        {
                            btn.OnRelease();
                            return;
                        }
                        else btn.ToDefault();
                    break;
            }
        }

        /// <summary>Process all standard logic for a list of sliders,
        /// note: this will first find the current grabbed slider, therefore will not allow you to
        ///       by accidently grab multiple sliders</summary>
        /// <param name="ms">current mouse state</param>
        /// <param name="sliders">the list of sliders you wish to process</param>
        public static void processSliders(MouseState ms, List<Slider> sliders)
        {
            if (ms == null || sliders == null)
                return;

            short grabbedIndex = -1;
            for (short i = 0; i < sliders.Count; i++)
            {
                if (sliders[i].isGrabbed == true)
                {
                    grabbedIndex = i;
                    break;
                }
            }
            if (grabbedIndex != -1)
            {
                sliders[grabbedIndex].Input(ms);
            }
            else
            {
                foreach (Slider slider in sliders)
                    slider.Input(ms);
            }
        }

        /// <summary>Convert a Xna vector3 to StillDesign Vector3, allocates new memory</summary>
        /// <param name="val">Xna value</param>
        /// <returns>still design value</returns>
        public static StillDesign.PhysX.MathPrimitives.Vector3 GetStillDesign_Vector3(Microsoft.Xna.Framework.Vector3 val)
        {
            return new StillDesign.PhysX.MathPrimitives.Vector3(val.X, val.Y, val.Z);
        }

        /// <summary>allocate new memory for a still design vector3</summary>
        /// <param name="x">x component</param>
        /// <param name="y">y component</param>
        /// <param name="z">z component</param>
        /// <returns>StillDesign.PhysX.MathPrimitives.Vector3 value</returns>
        public static StillDesign.PhysX.MathPrimitives.Vector3 GetStillDesign_Vector3(float x, float y, float z)
        {
            return new StillDesign.PhysX.MathPrimitives.Vector3(x, y, z);
        }

        /// <summary>Set the input still design vector3 from the xna vector3, without allocation new memory</summary>
        /// <param name="sdv3">StillDesign.PhysX.MathPrimitives.Vector3 value</param>
        /// <param name="xnav3">Microsoft.Xna.Framework.Vector3 value</param>
        public static void SetStillDesignFromXna(ref StillDesign.PhysX.MathPrimitives.Vector3 sdv3, ref Microsoft.Xna.Framework.Vector3 xnav3)
        {
            sdv3.X = xnav3.X;
            sdv3.Y = xnav3.Y;
            sdv3.Z = xnav3.Z;
        }

        /// <summary>Set the input XNA vector3 from the Still Design vector3, without allocation new memory</summary>
        /// <param name="sdv3">StillDesign.PhysX.MathPrimitives.Vector3 value</param>
        /// <param name="xnav3">Microsoft.Xna.Framework.Vector3 value</param>
        public static void SetXnaFromStillDesign(ref Microsoft.Xna.Framework.Vector3 xnav3, ref StillDesign.PhysX.MathPrimitives.Vector3 sdv3)
        {
            xnav3.X = sdv3.X;
            xnav3.Y = sdv3.Y;
            xnav3.Z = sdv3.Z;
        }
    }
}
