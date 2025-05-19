using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
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
        public void AddUpdateCommand(Keys inputKey, ICommand command)
        {
            keybindsUpdate.Add(inputKey, command);
        }

        public void AddButtonDownCommand(Keys inputKey, ICommand command)
        {
            keybindsButtonDown.Add(inputKey, command);
        }

        //GamePad commands
        private Dictionary<PlayerIndex, Dictionary<Buttons, ICommand>> buttonbindsUpdate = new();
        private Dictionary<PlayerIndex, Dictionary<Buttons, ICommand>> buttonbindsButtonDown = new();
        private Dictionary<PlayerIndex, GamePadState> previousButtonStates = new();
        private PlayerIndex[] supportedPlayers = new[] { PlayerIndex.One, PlayerIndex.Two };

        public void AddUpdateCommand(PlayerIndex player, Buttons inputButton, ICommand command)
        {
            if (!buttonbindsUpdate.ContainsKey(player))
            {
                buttonbindsUpdate[player] = new();
            }
            buttonbindsUpdate[player][inputButton] = command;
        }

        public void AddButtonDownCommand(PlayerIndex player, Buttons inputButton, ICommand command)
        {
            if (!buttonbindsButtonDown.ContainsKey(player))
            {
                buttonbindsButtonDown[player] = new();
            }
            buttonbindsButtonDown[player][inputButton] = command;
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
            foreach (var player in supportedPlayers)
            {
                var currentState = GamePad.GetState(player);

                if (!previousButtonStates.ContainsKey(player))
                {
                    previousButtonStates[player] = currentState;
                }

                var previousButtonState = previousButtonStates[player];

                if (currentState.IsConnected)
                {
                    foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
                    {
                        if (currentState.IsButtonDown(button))
                        {
                            if (buttonbindsUpdate.TryGetValue(player, out var updateDict) && updateDict.TryGetValue(button, out var cmd))
                            {
                                cmd.Execute();
                            }

                            if (!previousButtonState.IsButtonDown(button) && buttonbindsButtonDown.TryGetValue(player, out var downDict) && downDict.TryGetValue(button, out var cmdBd))
                            {
                                cmdBd.Execute();
                            }
                        }
                    }
                    previousButtonStates[player] = currentState;
                }
            }
        }
    }
}
