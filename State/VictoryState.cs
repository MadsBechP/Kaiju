using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Kaiju.State
{
    /// <summary>
    /// VictoryState is the ending scene where
    /// player(s) can see who won/lost or if it was a draw.
    /// It also provides option to start a new battle with the same characters or return to the menu.
    /// Made by Emilie
    /// </summary>
    public class VictoryState : IGameState
    {
        private GameWorld game;
        private SpriteFont winFont;
        private SpriteFont promptFont;
        private string winText;
        private string promptText;

        public Color DefaultBackgroundColor => Color.DarkSlateGray;

        /// <summary>
        /// Constructor
        /// Initialize a new instance af the VictoryState
        /// </summary>
        /// <param name="game">The current GameWorld instance</param>
        /// <param name="winnerName">The name of the winner</param>
        /// <param name="isDraw">True if the match has ended in a draw</param>
        public VictoryState(GameWorld game, string winnerName, bool isDraw)
        {
            this.game = game;

            winFont = game.Content.Load<SpriteFont>("VictoryFont");
            promptFont = game.Content.Load<SpriteFont>("promptFont"); 
            promptText = "press ENTER to start new battle\nor SPACE to go back to the menu";


            if (isDraw)
            {
                winText = "You Are Too Weak\n      To Be King";
            }
            else
            {
                winText = $"A New King Has Risen!\n         Kneel Before\n            {winnerName}";
            }

        }

        /// <summary>
        /// Handles input and transition to BattleState or MenuState.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                game.ChangeGameState(new BattleState(game));
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                game.ChangeGameState(new MenuState(game));
            }
        }

        /// <summary>
        /// Draws the victory screen, including winner message and prompts.
        /// </summary>
        /// <param name="spriteBatch">used to draw</param>
        public void Draw(SpriteBatch spriteBatch)
        {
           
            var width = GameWorld.Instance.GraphicsDevice.Viewport.Width;
            var height = GameWorld.Instance.GraphicsDevice.Viewport.Height;

            Vector2 winOrigin = winFont.MeasureString(winText) / 2;
            spriteBatch.DrawString(winFont, winText, new Vector2(width * 0.50f, height * 0.40f), Color.White, 0, winOrigin, 1, SpriteEffects.None, 1);

            Vector2 promOrigin = promptFont.MeasureString(promptText) / 2;
            spriteBatch.DrawString(promptFont, promptText, new Vector2(width * 0.50f, height * 0.65f), Color.White, 0, promOrigin, 1, SpriteEffects.None, 1);
        }

        /// <summary>
        /// Cleans up the state when exiting. 
        /// Unused.
        /// </summary>
        public void Exit()
        {
        }

        /// <summary>
        /// Changes the controls of the players depeding on if a controller is connected or not
        /// </summary>
        /// <param name="p1Connected">Bool if player one has a controller connected</param>
        /// <param name="p2Connected">Bool if player two has a controller connected</param>
        public void OnControllerConnectionChanged(bool p1Connected, bool p2Connected)
        {
            
        }
    }
}
