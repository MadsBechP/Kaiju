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
        private string promptText;

        public Color BackgoundColor => Color.DarkSlateGray;

        public Color DefaultBackgroundColor => Color.DarkSlateGray;

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
        public void Draw(SpriteBatch spriteBatch)
        {
           
            var width = GameWorld.Instance.GraphicsDevice.Viewport.Width;
            var height = GameWorld.Instance.GraphicsDevice.Viewport.Height;

            Vector2 winOrigin = winFont.MeasureString(winText) / 2;
            spriteBatch.DrawString(winFont, winText, new Vector2(width * 0.50f, height * 0.40f), Color.White, 0, winOrigin, 1, SpriteEffects.None, 1);

            Vector2 promOrigin = promptFont.MeasureString(promptText) / 2;
            spriteBatch.DrawString(promptFont, promptText, new Vector2(width * 0.50f, height * 0.65f), Color.White, 0, promOrigin, 1, SpriteEffects.None, 1);
        }

        public void Exit()
        {
        }
    }
}
