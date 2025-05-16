using Microsoft.Xna.Framework;
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

        //Keyboard commands
        private Dictionary<Keys, ICommand> keybindsUpdate = new Dictionary<Keys, ICommand>();
        private Dictionary<Keys, ICommand> keybindsButtonDown = new Dictionary<Keys, ICommand>();
        private KeyboardState previousKeyState;

        //GamePad commands
        private Dictionary<Buttons, ICommand> buttonbindsUpdate = new();
        private Dictionary<Buttons, ICommand> buttonbindsButtonDown = new();
        private GamePadState previousButtonState;
        private PlayerIndex playerIndex = PlayerIndex.One;

        public void AddUpdateCommand(Keys inputKey, ICommand command)
        {
            keybindsUpdate.Add(inputKey, command);
        }

        public void AddButtonDownCommand(Keys inputKey, ICommand command)
        {
            keybindsButtonDown.Add(inputKey, command);
        }

        public void AddUpdateCommand(Buttons inputButton, ICommand command)
        {
            buttonbindsUpdate.Add(inputButton, command);
        }

        public void AddButtonDownCommand(Buttons inputButton, ICommand command)
        {
            buttonbindsButtonDown.Add(inputButton, command);
        }

        public void Execute()
        {
            //Keyboard input
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

            //Gamepad input
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            if (gamePadState.IsConnected)
            {
                foreach (Buttons button in System.Enum.GetValues(typeof(Buttons)))
                {
                    if (gamePadState.IsButtonDown(button))
                    {
                        if (buttonbindsUpdate.TryGetValue(button, out var cmd))
                        {
                            cmd.Execute();
                        }
                        if (!previousButtonState.IsButtonDown(button))
                        {
                            if (buttonbindsButtonDown.TryGetValue(button, out var cmdBd))
                            {
                                cmdBd.Execute();
                            }
                        }
                    }
                }
                previousButtonState = gamePadState;
            }
        }
    }
}
