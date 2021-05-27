using System;
using System.Collections.Generic;
using System.Text;
using Silk.NET.GLFW;
using HornetEngine.Graphics;
using System.Numerics;

namespace HornetEngine.Input
{
    public class Mouse
    {
        // The variables for the currently pressed buttons
        public readonly int MAX_PRESSED_BUTTONS = 5;
        private int[] pressed_buttons;

        // The current mouse position
        private Vector2 position = new Vector2(0, 0);
        // The current mode of the mouse {FPS, Invible or Visible}
        private MouseMode mode;

        public delegate void MousePressFunc(MouseButton button);
        public delegate void MouseReleaseFunc(MouseButton button);
        public delegate void MouseScrollFunc(double xoffset, double yoffset);
        public delegate void MouseMoveFunc(double xpos, double ypos, double deltaX, double deltaY);

        public event MousePressFunc MousePress;
        public event MouseReleaseFunc MouseRelease;
        public event MouseScrollFunc MouseScroll;
        public event MouseMoveFunc MouseMove;

        /// <summary>
        /// A function which will be called when the user scrolls
        /// </summary>
        /// <param name="w_handle">The handle of the window</param>
        /// <param name="xoffset">The amount scrolled on the x-axis</param>
        /// <param name="yoffset">The amount scrolled on the y-axis</param>
        private unsafe void OnMouseScroll(WindowHandle* w_handle, double xoffset, double yoffset)
        {
            // Call the mouseScroll event
            MouseScroll?.Invoke(xoffset, yoffset);
        }

        /// <summary>
        /// A function which will be called when the mouse moves.
        /// </summary>
        /// <param name="w_handle">The handle of the window</param>
        /// <param name="xpos">The new position on the x-axis</param>
        /// <param name="ypos">The new position on the y-axis</param>
        private unsafe void OnMouseMoved(WindowHandle* w_handle, double xpos, double ypos)
        {
            // Initialize the old values
            float oldX = position.X;
            float oldY = position.Y;

            // Calculate the delta
            double deltaX = xpos - oldX;
            double deltaY = oldY - ypos;

            // Set the new values
            position.X = (float)xpos;
            position.Y = (float)ypos;

            // Call the mouseMoved event
            MouseMove?.Invoke(xpos, ypos, deltaX, deltaY);
        }

        /// <summary>
        /// A function which will be called when a mousebutton has been pressed/released
        /// </summary>
        /// <param name="w_handle">The handle of the window</param>
        /// <param name="button">The button which has used/param>
        /// <param name="action">The action of the button (pressed/released)</param>
        /// <param name="mods">The modifiers of the button (for ex: Ctrl, Shift etc.)</param>
        private unsafe void OnMousePressed(WindowHandle* w_handle, MouseButton button, InputAction action, KeyModifiers mods)
        {
            switch(action)
            {
                case InputAction.Press:
                    // Register the button as pressed
                    AddMouseButton(button);
                    ChangeMode(w_handle, button);
                    MousePress?.Invoke(button);
                    break;
                case InputAction.Release:
                    // Deregister the button as pressed
                    RemoveMouseButton(button);
                    MouseRelease?.Invoke(button);
                    break;
            }
        }

        /// <summary>
        /// A function which can be used to register a pressed button
        /// </summary>
        /// <param name="button">The button which has been pressed</param>
        private void AddMouseButton(MouseButton button)
        {
            Console.Clear();
            for (int i = 0; i < MAX_PRESSED_BUTTONS; i++)
            {
                // Check which place in the array has not been filled yet
                if (pressed_buttons[i] == -1)
                {
                    // Register the button within the array
                    pressed_buttons[i] = (int)button;
                    break;
                }
                // If all the buttons have been registered
                else if (i == MAX_PRESSED_BUTTONS - 1)
                {
                    // Overwrite the first button entry
                    pressed_buttons[0] = (int)button;
                }
            }

            // Print the currently pressed buttons
            for (int i = 0; i < MAX_PRESSED_BUTTONS; i++)
            {
                Console.WriteLine(pressed_buttons[i]);
            }
        }

        /// <summary>
        /// A function which can be used to deregister a button
        /// </summary>
        /// <param name="button">The button which should be deregistered</param>
        private void RemoveMouseButton(MouseButton button)
        {
            Console.Clear();
            // Loop through the pressed buttons to find the specific button
            for (int i = 0; i < MAX_PRESSED_BUTTONS; i++)
            {
                if (pressed_buttons[i] == (int)button)
                {
                    // Overwrite the value with -1
                    pressed_buttons[i] = -1;
                    break;
                }
            }
            // Print the currently pressed buttons
            for (int i = 0; i < MAX_PRESSED_BUTTONS; i++)
            {
                Console.WriteLine(pressed_buttons[i]);
            }
        }

        /// <summary>
        /// A function which can be used to switch the mouse's mode
        /// </summary>
        /// <param name="w_handle">The handle of the window</param>
        /// <param name="button">The button which has been pressed</param>
        private unsafe void ChangeMode(WindowHandle* w_handle, MouseButton button)
        {
            // Check which button has been pressed and set the mode accordingly
            if(button == MouseButton.Left)
            {
                mode = MouseMode.VISIBLE;
            } else if (button == MouseButton.Right)
            {
                mode = MouseMode.INVISIBLE;
            } else if (button == MouseButton.Middle)
            {
                mode = MouseMode.FPS;
            }
            // Print the new mode to the console
            Console.WriteLine($"New mode: {mode}");

            // Update the input mode of the cursor
            NativeWindow.GLFW.SetInputMode(w_handle, CursorStateAttribute.Cursor, (CursorModeValue)mode);
        }

        /// <summary>
        /// A function which will return the current mode of the mouse
        /// </summary>
        /// <returns>A MouseMode, depending on the current mode.</returns>
        public MouseMode GetMode()
        {
            return this.mode;
        }

        /// <summary>
        /// The constructor of the Mouse class
        /// </summary>
        /// <param name="w_handle">The handle of the window</param>
        public unsafe Mouse(WindowHandle* w_handle)
        {
            // Initialize the currently pressed buttons array
            pressed_buttons = new int[MAX_PRESSED_BUTTONS];
            for(int i = 0; i < MAX_PRESSED_BUTTONS; i++)
            {
                pressed_buttons[i] = -1;
            }

            // Set the default mouse mode
            NativeWindow.GLFW.SetInputMode(w_handle, CursorStateAttribute.Cursor, CursorModeValue.CursorNormal);
            mode = MouseMode.VISIBLE;

            // Initialize the callbacks
            NativeWindow.GLFW.SetCursorPosCallback(w_handle, OnMouseMoved);
            NativeWindow.GLFW.SetMouseButtonCallback(w_handle, OnMousePressed);
            NativeWindow.GLFW.SetScrollCallback(w_handle, OnMouseScroll);
        }
    }
}
