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
            
            //6 mellemrum
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
            //Press ENTER to play new battle
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
    }
}
