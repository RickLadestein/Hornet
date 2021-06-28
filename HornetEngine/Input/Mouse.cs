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
        /// <summary>
        /// The maximum amount of buttons which can be pressed
        /// </summary>
        public readonly int MAX_PRESSED_BUTTONS = 5;

        private MouseButtons[] pressed_buttons;
        private Vector2 position = new Vector2(0, 0);
        private MouseMode mode;

        private unsafe WindowHandle* parent_window;

        /// <summary>
        /// The mouse press function
        /// </summary>
        /// <param name="button">The mouse button which has been pressed</param>
        public delegate void MousePressFunc(MouseButtons button);

        /// <summary>
        /// The mouse release function
        /// </summary>
        /// <param name="button">The mouse button which has been released</param>
        public delegate void MouseReleaseFunc(MouseButtons button);

        /// <summary>
        /// The mouse scroll function
        /// </summary>
        /// <param name="xoffset">The x offset</param>
        /// <param name="yoffset">The y offset</param>
        public delegate void MouseScrollFunc(double xoffset, double yoffset);

        /// <summary>
        /// The mouse move function
        /// </summary>
        /// <param name="xpos">The new x pos</param>
        /// <param name="ypos">The new y pos</param>
        /// <param name="deltaX">The difference between the x positions</param>
        /// <param name="deltaY">The difference between the y positions</param>
        public delegate void MouseMoveFunc(double xpos, double ypos, double deltaX, double deltaY);

        /// <summary>
        /// The mouse press event
        /// </summary>
        public event MousePressFunc MousePress;

        /// <summary>
        /// The mouse release event
        /// </summary>
        public event MouseReleaseFunc MouseRelease;

        /// <summary>
        /// The mouse scroll event
        /// </summary>
        public event MouseScrollFunc MouseScroll;

        /// <summary>
        /// The mouse move event
        /// </summary>
        public event MouseMoveFunc MouseMove;


        /// <summary>
        /// The constructor of the Mouse class
        /// </summary>
        /// <param name="w_handle">The handle of the window</param>
        public unsafe Mouse(WindowHandle* w_handle)
        {
            // Initialize the currently pressed buttons array
            pressed_buttons = new MouseButtons[MAX_PRESSED_BUTTONS];
            for (int i = 0; i < MAX_PRESSED_BUTTONS; i++)
            {
                pressed_buttons[i] = MouseButtons.Unknown;
            }

            this.parent_window = w_handle;
            // Set the default mouse mode
            NativeWindow.GLFW.SetInputMode(w_handle, CursorStateAttribute.Cursor, CursorModeValue.CursorNormal);
            mode = MouseMode.VISIBLE;

            // Initialize the callbacks
            NativeWindow.GLFW.SetCursorPosCallback(w_handle, OnMouseMoved);
            NativeWindow.GLFW.SetMouseButtonCallback(w_handle, OnMousePressed);
            NativeWindow.GLFW.SetScrollCallback(w_handle, OnMouseScroll);
        }

        /// <summary>
        /// Checks if the specified mouse button is down
        /// </summary>
        /// <param name="button">The button to be checked against</param>
        /// <returns>true if mousebutton was down, false if not</returns>
        public bool IsButtonDown(MouseButtons button)
        {
            for(int i = 0; i < MAX_PRESSED_BUTTONS; i++)
            {
                if(pressed_buttons[i] == button)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets all the currently pressed mouse buttons
        /// </summary>
        /// <returns>Array with all currently pressed mouse buttons</returns>
        public MouseButtons[] GetPressedButtons()
        {
            MouseButtons[] output = new MouseButtons[MAX_PRESSED_BUTTONS];
            pressed_buttons.CopyTo(output, 0);
            return output;
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
        /// A function which can be used to switch the mouse's mode
        /// </summary>
        /// <param name="mode">The button which has been pressed</param>
        public unsafe void SetMode(MouseMode mode)
        {
            this.mode = mode;
            NativeWindow.GLFW.SetInputMode(parent_window, CursorStateAttribute.Cursor, (CursorModeValue)mode);
        }

        /// <summary>
        /// Suggests to the window to use the raw mouse motion or not
        /// </summary>
        /// <param name="state">true == raw mouse motion, false == windows normalised mouse motion</param>
        public unsafe void SetRawInputMode(bool state)
        {
            NativeWindow.GLFW.SetInputMode(parent_window, CursorStateAttribute.RawMouseMotion, state);
        }

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
                    AddMouseButton((MouseButtons)button);
                    MousePress?.Invoke((MouseButtons)button);
                    break;
                case InputAction.Release:
                    // Deregister the button as pressed
                    RemoveMouseButton((MouseButtons)button);
                    MouseRelease?.Invoke((MouseButtons)button);
                    break;
            }
        }

        /// <summary>
        /// A function which can be used to register a pressed button
        /// </summary>
        /// <param name="button">The button which has been pressed</param>
        private void AddMouseButton(MouseButtons button)
        {
            for (int i = 0; i < MAX_PRESSED_BUTTONS; i++)
            {
                if (pressed_buttons[i] == MouseButtons.Unknown)
                {
                    pressed_buttons[i] = button;
                    break;
                }
                else if (i == MAX_PRESSED_BUTTONS - 1)
                {
                    // Overwrite the first button entry
                    pressed_buttons[0] = button;
                }
            }
        }

        /// <summary>
        /// A function which can be used to deregister a button
        /// </summary>
        /// <param name="button">The button which should be deregistered</param>
        private void RemoveMouseButton(MouseButtons button)
        {
            for (int i = 0; i < MAX_PRESSED_BUTTONS; i++)
            {
                if (pressed_buttons[i] == button)
                {
                    // Overwrite the value with -1
                    pressed_buttons[i] = MouseButtons.Unknown;
                    break;
                }
            }
        }
    }
}
