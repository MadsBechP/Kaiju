using Kaiju.Command;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.State
{
    /// <summary>
    /// This is the game state where a player can selects and/or creates a profile before starting a battle.
    /// The class handles profile creation logic and profile selection for both Player 1 and Player 2.
    /// Made by Emilie
    /// </summary>
    public class ProfileState : IGameState, ISelectable
    {
        public Color DefaultBackgroundColor => Color.Gray;

        private GameWorld game;
        private bool isPlayer1;
        private SpriteFont textFont;
        private List<string> profiles;
        private int selectedIndex = 0;
        private PlayerStats selectedStats;

        private bool player1Confirmed = false;
        private bool player2Confirmed = false;

        private bool isCreatingProfile = false;
        private string newProfileName;
        private string errorMessage;
        private KeyboardState previousKeyState;

        /// <summary>
        /// Constructor
        /// Makes a new instance of the ProfileState class, whit binding input controls and loading profiles.
        /// </summary>
        /// <param name="game">The current GameWorld instance</param>
        /// <param name="isPlayer1">Checks which player it is. True = P1, False = P2</param>
        public ProfileState(GameWorld game, bool isPlayer1)
        {
            this.game = game;
            this.isPlayer1 = isPlayer1;
            textFont = game.Content.Load<SpriteFont>("promptFont");

            profiles = DatabaseManager.Instance.ListAllPlayerNames();

            selectedStats = DatabaseManager.Instance.PrintPlayerStats(profiles[selectedIndex]);
            profiles.Add("Create new profile");
        }

        /// <summary>
        /// Updates the current state.
        /// keeps checking for if P1 or P2 have confirmed their choice of profile or is creating a new one.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (isCreatingProfile)
            {
                CreatingProfile();
                previousKeyState = Keyboard.GetState();
                return;
            }

            if (player1Confirmed && isPlayer1)
            {
                game.SelectedPlayerProfileP1 = profiles[selectedIndex];
                game.ChangeGameState(new MenuState(game));
            }
            if (player2Confirmed && !isPlayer1)
            {
                game.SelectedPlayerProfileP2 = profiles[selectedIndex];
                game.ChangeGameState(new MenuState(game));
            }
        }

        /// <summary>
        /// Draws profile selection and/or profile creation. 
        /// This include the list of profiles and the current profiles statistics.
        /// </summary>
        /// <param name="spriteBatch">Used for drawing</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            var w = game.GraphicsDevice.Viewport.Width;
            var h = game.GraphicsDevice.Viewport.Height;

            //title
            string title = isPlayer1 ? "Player 1 : Selecting Profile" : "Player 2 : Selecting Profile";
            spriteBatch.DrawString(textFont, title, new Vector2((w * 0.5f) - textFont.MeasureString(title).X / 2, h * 0.10f), Color.White);

            //Draw creating profile
            if (isCreatingProfile)
            {
                spriteBatch.DrawString(textFont, "Enter new profile name", new Vector2(w * 0.30f, h * 0.25f), Color.White);
                spriteBatch.DrawString(textFont, newProfileName + "|", new Vector2(w * 0.32f, h * 0.35f), Color.Yellow);

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    spriteBatch.DrawString(textFont, errorMessage, new Vector2(w * 0.35f, h * 0.45f), Color.Red);
                }

                return;
            }

            //Profile name            
            for (int i = 0; i < profiles.Count; i++)
            {
                string profileName = profiles[i];
                float spacing = 100f;
                Vector2 position = new Vector2(w * 0.25f, h * 0.30f + i * spacing);
                Color color = (i == selectedIndex) ? Color.Yellow : Color.White;

                spriteBatch.DrawString(textFont, profileName, position, color);
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                spriteBatch.DrawString(textFont, errorMessage, new Vector2(w * 0.40f, h * 0.20f), Color.Red);
            }

            //Profile stats
            if (selectedStats != null)
            {
                spriteBatch.DrawString(textFont, $"Games Played: {selectedStats.Wins}", new Vector2(w * 0.50f, h * 0.35f), Color.White);
                spriteBatch.DrawString(textFont, $"Wins: {selectedStats.Wins}", new Vector2(w * 0.50f, h * 0.40f), Color.White);
                spriteBatch.DrawString(textFont, $"Losses: {selectedStats.Losses}", new Vector2(w * 0.50f, h * 0.45f), Color.White);
                spriteBatch.DrawString(textFont, $"Draws: {selectedStats.Draws}", new Vector2(w * 0.50f, h * 0.50f), Color.White);
                spriteBatch.DrawString(textFont, $"Win/Loss Ratio: {selectedStats.WinLossRatio}", new Vector2(w * 0.50f, h * 0.55f), Color.White);
                spriteBatch.DrawString(textFont, $"KOs: {selectedStats.KOs}", new Vector2(w * 0.50f, h * 0.60f), Color.White);
                spriteBatch.DrawString(textFont, $"KOd: {selectedStats.KOd}", new Vector2(w * 0.50f, h * 0.65f), Color.White);
                spriteBatch.DrawString(textFont, $"Favorite Kaiju: {selectedStats.FavoriteCharacter}", new Vector2(w * 0.50f, h * 0.70f), Color.White);
            }
            else
            {
                spriteBatch.DrawString(textFont, "No states available", new Vector2(w * 0.75f, h * 0.50f), Color.White);
            }
        }

        /// <summary>
        /// Called when exiting the ProfileState. 
        /// Unused.
        /// </summary>
        public void Exit()
        {

        }

        /// <summary>
        /// Changes between the different profiles.
        /// The stats will change to fit the selected profile.
        /// When reaching past one end of the list, it loop back around to the other end.
        /// </summary>
        /// <param name="direction"> -1 is for up, 1 is for down </param>
        /// <param name="isPlayer1">A bool that checks if the player is P1. If not then it must be P2</param>
        public void ChangeSelection(int direction, bool isPlayer1)
        {
            selectedIndex += direction;
            if (selectedIndex < 0) { selectedIndex = profiles.Count - 1; }
            if (selectedIndex >= profiles.Count) { selectedIndex = 0; }

            selectedStats = DatabaseManager.Instance.PrintPlayerStats(profiles[selectedIndex]);
        }

        /// <summary>
        /// Player confirms their currently selected profile.
        /// This method checks which player confirmed, which then activates Update.
        /// If "Creating new profile" is selected, it will switch to profile creation mode.
        /// Prevent both player from selecting the same profile.
        /// </summary>
        /// <param name="isPlayer1">A bool that tells which player is confirming</param>
        public void ConfirmSelection(bool isPlayer1)
        {
            string selectedProfile = profiles[selectedIndex];

            if (selectedProfile == "Create new profile")
            {
                isCreatingProfile = true;
                newProfileName = "";
                errorMessage = "";
                selectedIndex = profiles.Count - 1; // makes sure that it is looking at "Creating new profile", not previous chosen name

                return;
            }

            if (isPlayer1)
            {
                if(game.SelectedPlayerProfileP2 == selectedProfile)
                {
                    errorMessage = "Player 2 is using this profile";
                    return;
                }
                player1Confirmed = true;
            }
            else
            {
                if (game.SelectedPlayerProfileP1 == selectedProfile)
                {
                    errorMessage = "Player 1 is using this profile";
                    return;
                }
                player2Confirmed = true;
            }
        }

        /// <summary>
        /// Creates a new profile.
        /// Uses the keyboard to type a profile name.
        /// Allows characters (letters, numbers and space), uses backspace to delete a character,
        /// and confirms the new profile name with Enter.
        /// Validate profile name. Does not allow use of unique characters or an empty name.
        /// </summary>
        public void CreatingProfile()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            foreach (Keys key in keyboardState.GetPressedKeys())
            {
                if(key == Keys.Enter)
                {
                    //error management
                    if (string.IsNullOrWhiteSpace(newProfileName)) { errorMessage = "Profile name cannot be empty"; return; }
                    if (DatabaseManager.Instance.ListAllPlayerNames().Contains(newProfileName)) { errorMessage = "This profile name already exist"; return; }

                    // Create profile
                    DatabaseManager.Instance.AddNewProfile(newProfileName);

                    // reload and reset 
                    profiles = DatabaseManager.Instance.ListAllPlayerNames();
                    profiles.Add("Create new profile");
                    selectedIndex = profiles.IndexOf(newProfileName);
                    selectedStats = DatabaseManager.Instance.PrintPlayerStats(newProfileName);

                    isCreatingProfile = false;
                    newProfileName = "";
                    errorMessage = "";
                    return;
                }

                char c = KeyToChar(key, keyboardState);
                if (c != '\0' && !previousKeyState.IsKeyDown(key))
                {
                    newProfileName += c;
                }

                if (key == Keys.Back && !previousKeyState.IsKeyDown(key))
                {
                    if(newProfileName.Length > 0)
                    {
                        newProfileName = newProfileName.Substring(0, newProfileName.Length - 1);
                    }
                }
            }
        }

        /// <summary>
        /// Converts a keyboard key to its characters representation.
        /// Can use uppercasing (A-Z), normal letters (a-z), numbers (0-9) and space.
        /// Ignores unsupported keys.
        /// </summary>
        /// <param name="key">The pressed key</param>
        /// <param name="keyboardState">Current state of the keyboard. Used to check for Shift</param>
        /// <returns>The corresponding character, or '\0' if the key is not valid.</returns>
        private char KeyToChar(Keys key, KeyboardState keyboardState)
        {
            bool shift = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);

            if(key >= Keys.A && key <= Keys.Z)
            {
                return (char)(shift ? key : key + 32);
            }
            if(key >= Keys.D0 && key <= Keys.D9)
            {
                return (char)(key - Keys.D0 + '0');
            }
            if(key == Keys.Space)
            {
                return ' ';
            }
            return '\0';
        }

        /// <summary>
        /// Changes between controller and keyboard controls when called
        /// </summary>
        /// <param name="p1Connected">If player 1 has a controller connected</param>
        /// <param name="p2Connected">If player 2 has a controller connected</param>
        public void OnControllerConnectionChanged(bool p1Connected, bool p2Connected)
        {
            InputHandler.Instance.ClearBindings();

            if (p1Connected)
            {
                InputHandler.Instance.AddButtonDownCommand(PlayerIndex.One, Buttons.LeftThumbstickUp, new ChangeSelectionCommand(-1, this, true));
                InputHandler.Instance.AddButtonDownCommand(PlayerIndex.One, Buttons.LeftThumbstickDown, new ChangeSelectionCommand(1, this, true));
                InputHandler.Instance.AddButtonDownCommand(PlayerIndex.One, Buttons.A, new ConfirmSelectionCommand(this, true));
            }
            else
            {
                InputHandler.Instance.AddButtonDownCommand(Keys.W, new ChangeSelectionCommand(-1, this, true));
                InputHandler.Instance.AddButtonDownCommand(Keys.S, new ChangeSelectionCommand(1, this, true));
                InputHandler.Instance.AddButtonDownCommand(Keys.LeftControl, new ConfirmSelectionCommand(this, true));
            }

            if (p2Connected)
            {
                InputHandler.Instance.AddButtonDownCommand(PlayerIndex.Two, Buttons.LeftThumbstickUp, new ChangeSelectionCommand(-1, this, false));
                InputHandler.Instance.AddButtonDownCommand(PlayerIndex.Two, Buttons.LeftThumbstickDown, new ChangeSelectionCommand(1, this, false));
                InputHandler.Instance.AddButtonDownCommand(PlayerIndex.Two, Buttons.A, new ConfirmSelectionCommand(this, false));
            }
            else
            {
                InputHandler.Instance.AddButtonDownCommand(Keys.Up, new ChangeSelectionCommand(-1, this, false));
                InputHandler.Instance.AddButtonDownCommand(Keys.Down, new ChangeSelectionCommand(1, this, false));
                InputHandler.Instance.AddButtonDownCommand(Keys.RightShift, new ConfirmSelectionCommand(this, false));
            }
        }

        /// <summary>
        /// Changes to a different state when called
        /// </summary>
        /// <param name="isPlayer1">Checks whether the selection is for player 1 (true) or player 2 (false)</param>
        public void ChangeToProfileState(bool isPlayer1)
        {
            
        }
    }
}
