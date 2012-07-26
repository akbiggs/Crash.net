using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace CrashNet.Engine
{
    /// <summary>
    /// A wrapper class for all game input.
    /// It needs to be updated in the main update method, but can be accessed statically.
    /// </summary>
    public static class Input
    {
        private static MouseState mouse = Mouse.GetState();
        private static MouseState mousePrev = Mouse.GetState();
        private static KeyboardState keyboard = Keyboard.GetState();
        private static KeyboardState keyboardPrev = Keyboard.GetState();

        /// <summary>
        /// Updates the input information.
        /// </summary>
        public static void Update()
        {
            mousePrev = mouse;
            keyboardPrev = keyboard;
            mouse = Mouse.GetState();
            keyboard = Keyboard.GetState();
        }

        public static KeyboardState GetKeyboardState()
        {
            return Input.keyboard;
        }

        public static KeyboardState GetPreviousKeyboardState()
        {
            return Input.keyboardPrev;
        }

        public static bool IsKeyDown(Keys key)
        {
            return keyboard.IsKeyDown(key);
        }

        public static bool IsKeyUp(Keys key)
        {
            return keyboard.IsKeyUp(key);
        }

        public static bool KeyboardTapped(Keys key)
        {
            return (keyboardPrev.IsKeyDown(key) && keyboard.IsKeyUp(key));
        }

        public static bool MouseScrollDown
        {
            get { return (mouse.ScrollWheelValue < mousePrev.ScrollWheelValue); }
        }

        public static bool MouseScrollUp
        {
            get { return (mouse.ScrollWheelValue > mousePrev.ScrollWheelValue); }
        }

        public static bool MouseLeftButtonPressed
        {
            get { return (mouse.LeftButton == ButtonState.Pressed && mousePrev.LeftButton == ButtonState.Released); }
        }

        public static bool MouseLeftButtonTapped
        {
            get { return (mouse.LeftButton == ButtonState.Pressed && mousePrev.LeftButton == ButtonState.Released); }
        }

        public static bool MouseRightButtonTapped
        {
            get { return (mouse.RightButton == ButtonState.Pressed && mousePrev.RightButton == ButtonState.Released); }
        }

        public static bool MouseMiddleButtonTapped
        {
            get { return (mouse.MiddleButton == ButtonState.Pressed && mousePrev.MiddleButton == ButtonState.Released); }
        }

        public static bool MouseLeftButtonDown
        {
            get { return mouse.LeftButton == ButtonState.Pressed; }
        }

        public static bool MouseRightButtonDown
        {
            get { return mouse.RightButton == ButtonState.Pressed; }
        }

        public static bool MouseMiddleButtonDown
        {
            get { return mouse.MiddleButton == ButtonState.Pressed; }
        }

        public static Vector2 MousePosition
        {
            get { return new Vector2(mouse.X, mouse.Y); }
        }

        public static Vector2 MousePrevPosition
        {
            get { return new Vector2(mousePrev.X, mousePrev.Y); }
        }
    }
}