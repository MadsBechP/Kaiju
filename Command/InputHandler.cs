using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Kaiju.Command
{
    public class InputHandler
    {
        private static InputHandler instance;

        public static InputHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InputHandler();
                }
                return instance;
            }
        }
        private InputHandler()
        {

        }

        private Dictionary<Keys, ICommand> keybindsUpdate = new Dictionary<Keys, ICommand>();
        private Dictionary<Keys, ICommand> keybindsButtonDown = new Dictionary<Keys, ICommand>();
        private KeyboardState previousKeyState;
        public void AddUpdateCommand(Keys inputKey, ICommand command)
        {
            keybindsUpdate.Add(inputKey, command);
        }
        public void AddButtonDownCommand(Keys inputKey, ICommand command)
        {
            keybindsButtonDown.Add(inputKey, command);
        }
        public void Execute()
        {
            KeyboardState keyState = Keyboard.GetState();

            foreach (var pressedKey in keyState.GetPressedKeys())
            {
                if (keybindsUpdate.TryGetValue(pressedKey, out ICommand cmd))
                {
                    cmd.Execute();
                }
                if (!previousKeyState.IsKeyDown(pressedKey) && keyState.IsKeyDown(pressedKey))
                {
                    if (keybindsButtonDown.TryGetValue(pressedKey, out ICommand cmdBd))
                    {
                        cmdBd.Execute();
                    }
                }
            }
            previousKeyState = keyState;

        }
    }
}
