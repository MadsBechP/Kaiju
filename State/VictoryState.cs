using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Kaiju.State
{
    /// <summary>
    /// VictoryState is the ending scene where
    /// you can see who won and start a new fight.
    /// This class stands for the creation of the victory scene.
    /// </summary>
    public class VictoryState : IGameState
    {
        private GameWorld game;
        private SpriteFont winFont;
        private SpriteFont promptFont;
        private string winText;
        private string nameText;
        private string promptText;
        private bool isDraw;

        public Color BackgoundColor => Color.DarkSlateGray;

        public VictoryState(GameWorld game, string winnerName, bool isDraw)
        {
            this.game = game;

            winFont = game.Content.Load<SpriteFont>("VictoryFont");
            promptFont = game.Content.Load<SpriteFont>("promptFont");
            promptText = "press ENTER to start new battle";


            if (isDraw)
            {
                winText = "You Are Too Weak\n           To Be King";
            }
            else
            {
                winText = $"A New King Has Risen!\n       Kneel Before\n           {winnerName}";
            }

        }

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                game.ChangeGameState(new BattleState(game));
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            var width = GameWorld.Instance.GraphicsDevice.Viewport.Width;
            var height = GameWorld.Instance.GraphicsDevice.Viewport.Height;

            spriteBatch.DrawString(winFont, winText, new Vector2(width * 0.50f, height * 0.40f), Color.White);

            spriteBatch.DrawString(promptFont, promptText, new Vector2(width * 0.50f, height * 0.65f), Color.White);
        }

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
