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
    public class ProfileState : IGameState, ISelectable
    {
        public Color DefaultBackgroundColor => Color.Beige;

        private GameWorld game;
        private bool isPlayer1;
        private SpriteFont textFont;
        private List<string> profiles;
        private int selectedIndex = 0;
        private PlayerStats selectedStats;

        private bool player1Confirmed = false;
        private bool player2Confirmed = false;

        public ProfileState(GameWorld game, bool isPlayer1)
        {
            this.game = game;
            this.isPlayer1 = isPlayer1;
            textFont = game.Content.Load<SpriteFont>("promptFont");

            profiles = DatabaseManager.Instance.ListAllPlayerNames();

            InputHandler.Instance.ClearBindings();
            if (isPlayer1)
            {
                InputHandler.Instance.AddButtonDownCommand(Keys.W, new ChangeSelectionCommand(-1, this, true));
                InputHandler.Instance.AddButtonDownCommand(Keys.S, new ChangeSelectionCommand(1, this, true));
                InputHandler.Instance.AddButtonDownCommand(Keys.LeftControl, new ConfirmSelectionCommand(this, true));

            }
            else
            {
                InputHandler.Instance.AddButtonDownCommand(Keys.Up, new ChangeSelectionCommand(-1, this, false));
                InputHandler.Instance.AddButtonDownCommand(Keys.Down, new ChangeSelectionCommand(1, this, false));
                InputHandler.Instance.AddButtonDownCommand(Keys.Enter, new ConfirmSelectionCommand(this, false));
            }

            selectedStats = DatabaseManager.Instance.PrintPlayerStats(profiles[selectedIndex]);
        }

        /// <summary>
        /// Keeps checking if the player currently changing their profile have decided on a profile.
        /// When P1 or P2 confirms their choice, the choice will be saved in GameWorld and the player
        /// will go back to MenuState
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
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

        public void Draw(SpriteBatch spriteBatch)
        {
            var w = game.GraphicsDevice.Viewport.Width;
            var h = game.GraphicsDevice.Viewport.Height;

            //title
            string title = isPlayer1 ? "Player 1 : Selecting Profile" : "Player 2 : Selecting Profile";
            spriteBatch.DrawString(textFont, title, new Vector2((w * 0.5f) - textFont.MeasureString(title).X / 2, h * 0.10f), Color.White);

            //Profile name            
            for (int i = 0; i < profiles.Count; i++)
            {
                string profileName = profiles[i];
                float spacing = 100f;
                Vector2 position = new Vector2(w * 0.25f, h * 0.30f + i * spacing);
                Color color = (i == selectedIndex) ? Color.Yellow : Color.White;

                spriteBatch.DrawString(textFont, profileName, position, color);
            }

            //Profile stats
            if (selectedStats != null)
            {
                spriteBatch.DrawString(textFont, $"Games Played: {selectedStats.Wins}", new Vector2(w * 0.50f, h * 0.35f), Color.White);
                spriteBatch.DrawString(textFont, $"Wins: {selectedStats.Wins}", new Vector2(w * 0.50f, h * 0.40f), Color.White);
                spriteBatch.DrawString(textFont, $"Losses: {selectedStats.Losses}", new Vector2(w * 0.50f, h * 0.45f), Color.White);
                spriteBatch.DrawString(textFont, $"Draws: {selectedStats.Draws}", new Vector2(w * 0.50f, h * 0.50f), Color.White);
                spriteBatch.DrawString(textFont, $"Win/Loss Ratio: {selectedStats.WinLossRatio}", new Vector2(w * 0.50f, h * 0.55f), Color.White);
                spriteBatch.DrawString(textFont, $"Favorite Kaiju: {selectedStats.FavoriteCharacter}", new Vector2(w * 0.50f, h * 0.60f), Color.White);
            }
            else
            {
                spriteBatch.DrawString(textFont, "No states available", new Vector2(w * 0.75f, h * 0.50f), Color.White);
            }
        }

        public void Exit()
        {

        }

        /// <summary>
        /// Scroll between the different profiles.
        /// the stats will change to fit the selected profile.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="isPlayer1">A bool that checks if the player is P1. If not then it must be P2</param>
        public void ChangeSelection(int direction, bool isPlayer1)
        {
            selectedIndex += direction;
            if (selectedIndex < 0) { selectedIndex = profiles.Count - 1; }
            if (selectedIndex >= profiles.Count) { selectedIndex = 0; }

            selectedStats = DatabaseManager.Instance.PrintPlayerStats(profiles[selectedIndex]);
        }

        /// <summary>
        /// Player confirms their choice of profile.
        /// This method checks which player confirmed, which then activates Update.
        /// </summary>
        /// <param name="isPlayer1">A bool that tells which player is confirming</param>
        public void ConfirmSelection(bool isPlayer1)
        {
            if (isPlayer1)
            {
                player1Confirmed = true;
            }
            else
            {
                player2Confirmed = true;
            }
        }
    }
}
