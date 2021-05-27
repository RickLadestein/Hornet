using System;
using Silk.NET.GLFW;
using HornetEngine.Graphics;

namespace HornetEngine.Input
{
    public class Keyboard
    {
        // The variables for the currently pressed buttons
        public readonly int MAX_PRESSED_BUTTONS = 5;
        private char[] pressed_buttons;

        // The keyboard mode
        private KeyboardMode mode;

        public delegate void KeyPressFunc(Keys key);
        public delegate void KeyReleaseFunc(Keys key);
        public delegate void KeyRepeatFunc(Keys key);

        public event KeyPressFunc keyPress;
        public event KeyReleaseFunc keyRelease;
        public event KeyRepeatFunc keyRepeat;

        /// <summary>
        /// The constructor of the Keyboard class
        /// </summary>
        /// <param name="w_handle">The window to which the keyboard class should be bound.</param>
        public unsafe Keyboard(WindowHandle* w_handle) 
        {
            // Initialize the pressed keys array
            pressed_buttons = new char[MAX_PRESSED_BUTTONS];
            for (int i = 0; i < MAX_PRESSED_BUTTONS; i++)
            {
                pressed_buttons[i] = ' ';
            }

            // Initialize the default keyboard mode
            mode = KeyboardMode.ACTION;

            // Set the callback for the key actions
            NativeWindow.GLFW.SetKeyCallback(w_handle, onKeyAction);
        }

        /// <summary>
        /// A function which can be used to change the keyboard's mode.
        /// </summary>
        /// <param name="newMode">The new mode which should be used.</param>
        public void changeMode(KeyboardMode newMode)
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
        private unsafe void onKeyAction(WindowHandle* window, Keys key, int scanCode, InputAction action, KeyModifiers mods)
        {
            // Check whether the current mode is TYPING
            if(mode == KeyboardMode.TYPING)
            {
                return;
            } 

            switch(action)
            {
                case InputAction.Press:
                    handlePressedKey(key);
                    break;
                case InputAction.Release:
                    handleReleasedKey(key);
                    break;
                case InputAction.Repeat:
                    handleRepeatKey(key);
                    break;
            }
        }

        /// <summary>
        /// A function which will handle a released key.
        /// </summary>
        /// <param name="releasedKey">The key which has been released.</param>
        private void handleReleasedKey(Keys releasedKey)
        {
            removeKey(releasedKey);
            keyRelease?.Invoke(releasedKey);
        }

        /// <summary>
        /// A function which will handle a pressed key.
        /// </summary>
        /// <param name="pressedKey">The key which has been pressed.</param>
        private void handlePressedKey(Keys pressedKey)
        {
            switch(pressedKey)
            {
                case Keys.E:
                    addKey(Keys.E);
                    handleKeyE();
                    keyPress?.Invoke(pressedKey);
                    break;
                default:
                    Console.WriteLine("This key has not been implemented yet.");
                    break;
            }
        }

        /// <summary>
        /// A function which will handle the repeated usage of a key.
        /// </summary>
        /// <param name="repeatKey">The key which has been repeated.</param>
        private void handleRepeatKey(Keys repeatKey)
        {
            switch(repeatKey)
            {
                case Keys.E:
                    handleKeyE();
                    keyRepeat?.Invoke(repeatKey);
                    break;
                default:
                    Console.WriteLine("An unknown key has been repeated.");
                    break;
            }
        }

        /// <summary>
        /// A function which will handle the specific 'e' key.
        /// </summary>
        private void handleKeyE()
        {
            Console.WriteLine("E has been pressed");
        }

        /// <summary>
        /// A function which can be used to add a key to the pressed keys arrays
        /// </summary>
        /// <param name="key"></param>
        private void addKey(Keys key)
        {
            Console.Clear();
            for (int i = 0; i < MAX_PRESSED_BUTTONS; i++)
            {
                // Check which place in the array has not been filled yet
                if (pressed_buttons[i] == ' ')
                {
                    // Register the button within the array
                    pressed_buttons[i] = (char)key;
                    break;
                }
                // If all the buttons have been registered
                else if (i == MAX_PRESSED_BUTTONS - 1)
                {
                    // Overwrite the first button entry
                    pressed_buttons[0] = (char)key;
                }
            }

            // Print the currently pressed buttons
            for (int i = 0; i < MAX_PRESSED_BUTTONS; i++)
            {
                Console.WriteLine(pressed_buttons[i]);
            }
        }

        /// <summary>
        /// A function which can be used to remove a key from the pressed keys array
        /// </summary>
        /// <param name="key">The key which should be removed.</param>
        private void removeKey(Keys key)
        {
            Console.Clear();
            // Loop through the pressed buttons to find the specific key
            for (int i = 0; i < MAX_PRESSED_BUTTONS; i++)
            {
                if (pressed_buttons[i] == (char)key)
                {
                    // Overwrite the value with an empty char
                    pressed_buttons[i] = ' ';
                    break;
                }
            }
            // Print the currently pressed buttons
            for (int i = 0; i < MAX_PRESSED_BUTTONS; i++)
            {
                Console.WriteLine(pressed_buttons[i]);
            }
        }
    }
}
