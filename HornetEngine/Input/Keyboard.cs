using System;
using Silk.NET.GLFW;
using HornetEngine.Graphics;

namespace HornetEngine.Input
{
    public class Keyboard
    {
        // The variables for the currently pressed buttons
        public static readonly int MAX_PRESSED_BUTTONS = 5;
        private Silk.NET.GLFW.Keys[] pressed_buttons;

        // The keyboard mode
        private KeyboardMode mode;

        /// <summary>
        /// The key press function
        /// </summary>
        /// <param name="key">The keys which were pressed</param>
        public delegate void KeyPressFunc(Keys key);

        /// <summary>
        /// The key release function
        /// </summary>
        /// <param name="key">The keys which were released</param>
        public delegate void KeyReleaseFunc(Keys key);

        /// <summary>
        /// The key repeat function
        /// </summary>
        /// <param name="key">The keys which were held down</param>
        public delegate void KeyRepeatFunc(Keys key);

        /// <summary>
        /// The key type function
        /// </summary>
        /// <param name="identifier">The identifier of the key</param>
        public delegate void KeyTypeFunc(uint identifier);

        /// <summary>
        /// The key press event
        /// </summary>
        public event KeyPressFunc KeyPress;

        /// <summary>
        /// The key release event
        /// </summary>
        public event KeyReleaseFunc KeyRelease;

        /// <summary>
        /// The key repeat event
        /// </summary>
        public event KeyRepeatFunc KeyRepeat;

        /// <summary>
        /// The key type event
        /// </summary>
        public event KeyTypeFunc KeyType;

        /// <summary>
        /// The constructor of the Keyboard class
        /// </summary>
        /// <param name="w_handle">The window to which the keyboard class should be bound.</param>
        public unsafe Keyboard(WindowHandle* w_handle) 
        {
            // Initialize the pressed keys array
            pressed_buttons = new Silk.NET.GLFW.Keys[MAX_PRESSED_BUTTONS];
            for (int i = 0; i < MAX_PRESSED_BUTTONS; i++)
            {
                pressed_buttons[i] = Silk.NET.GLFW.Keys.Unknown;
            }

            // Initialize the default keyboard mode
            mode = KeyboardMode.ACTION;

            // Set the callback for the key actions
            NativeWindow.GLFW.SetKeyCallback(w_handle, OnKeyAction);
            NativeWindow.GLFW.SetCharCallback(w_handle, OnKeyChar);
        }

        /// <summary>
        /// Gets all the pressed buttons
        /// </summary>
        /// <returns>Array filled with the currently pressed buttons</returns>
        public Silk.NET.GLFW.Keys[] GetPressedButtons()
        {
            Silk.NET.GLFW.Keys[] output = new Silk.NET.GLFW.Keys[MAX_PRESSED_BUTTONS];
            pressed_buttons.CopyTo(output, 0);
            return output;
        }

        /// <summary>
        /// Checks if the requested key is currently pressed
        /// </summary>
        /// <param name="key">The key that needs to be checked</param>
        /// <returns>true if the specified key was down, false if not</returns>
        public bool IsKeyDown(Silk.NET.GLFW.Keys key)
        {
            for(int i = 0; i < MAX_PRESSED_BUTTONS; i++)
            {
                if (pressed_buttons[i] == key)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// A function which can be used to change the keyboard's mode.
        /// </summary>
        /// <param name="newMode">The new mode which should be used.</param>
        public void ChangeMode(KeyboardMode newMode)
        {
            this.mode = newMode;
        }

        /// <summary>
        /// A function which will be called when a key has been pressed/released.
        /// </summary>
        /// <param name="window">The window in which the key has been pressed/released.</param>
        /// <param name="key">The key which has been used.</param>
        /// <param name="scanCode">The physical code which the keyboard passes to Windows.</param>
        /// <param name="action">A variable which holds whether the key has been pressed or released.</param>
        /// <param name="mods">Modifiers for the keys, such as shift/control etc.</param>
        private unsafe void OnKeyAction(WindowHandle* window, Keys key, int scanCode, InputAction action, KeyModifiers mods)
        {
            if(mode == KeyboardMode.ACTION)
            {
                switch(action)
                {
                    case InputAction.Press:
                        HandlePressedKey(key);
                        break;
                    case InputAction.Release:
                        HandleReleasedKey(key);
                        break;
                    case InputAction.Repeat:
                        HandleRepeatKey(key);
                        break;
                }
            }
        }

        /// <summary>
        /// A function which will be called when a key has been typed.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="codepoint"></param>
        private unsafe void OnKeyChar(WindowHandle* window, uint codepoint)
        {
            if(mode == KeyboardMode.TYPING)
            {
                KeyType?.Invoke(codepoint);
            }
        }

        /// <summary>
        /// A function which will handle a released key.
        /// </summary>
        /// <param name="releasedKey">The key which has been released.</param>
        private void HandleReleasedKey(Keys releasedKey)
        {
            RemoveKey(releasedKey);
            KeyRelease?.Invoke(releasedKey);
        }

        /// <summary>
        /// A function which will handle a pressed key.
        /// </summary>
        /// <param name="pressedKey">The key which has been pressed.</param>
        private void HandlePressedKey(Keys pressedKey)
        {
            AddKey(pressedKey);
            KeyPress?.Invoke(pressedKey);
        }

        /// <summary>
        /// A function which will handle a pressed key.
        /// </summary>
        /// <param name="pressedKey">The key which has been pressed.</param>
        private void HandleRepeatKey(Keys pressedKey)
        {
            KeyRepeat?.Invoke(pressedKey);
        }

        /// <summary>
        /// A function which can be used to add a key to the pressed keys arrays
        /// </summary>
        /// <param name="key"></param>
        private void AddKey(Keys key)
        {
            Console.Clear();
            for (int i = 0; i < MAX_PRESSED_BUTTONS; i++)
            {
                // Check which place in the array has not been filled yet
                if (pressed_buttons[i] == Silk.NET.GLFW.Keys.Unknown)
                {
                    // Register the button within the array
                    pressed_buttons[i] = key;
                    break;
                }
                // If all the buttons have been registered
                else if (i == MAX_PRESSED_BUTTONS - 1)
                {
                    // Overwrite the first button entry
                    pressed_buttons[0] = key;
                }
            }
        }

        /// <summary>
        /// A function which can be used to remove a key from the pressed keys array
        /// </summary>
        /// <param name="key">The key which should be removed.</param>
        private void RemoveKey(Keys key)
        {
            Console.Clear();
            // Loop through the pressed buttons to find the specific key
            for (int i = 0; i < MAX_PRESSED_BUTTONS; i++)
            {
                if (pressed_buttons[i] == key)
                {
                    // Overwrite the value with an empty char
                    pressed_buttons[i] = Silk.NET.GLFW.Keys.Unknown;
                    break;
                }
            }
        }
    }
}
