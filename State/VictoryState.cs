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
            

            if (isDraw)
            {
                winText = "You Are Too Weak To Be King";
                nameText = "";
            }
            else
            {
                winText = $"A New King Has Risen !\n    Kneel Before {winnerName}";
                //nameText = $"Kneel Before {winnerName}";
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

            spriteBatch.DrawString(winFont, winText, new Vector2((width / 2) - 425, (height / 2) - 325), Color.White);
            //spriteBatch.DrawString(winFont, nameText, new Vector2((width / 2) - 410, (height / 2) - 285), Color.White);

            spriteBatch.DrawString(promptFont, promptText, new Vector2((width / 2) - 450, (height / 2) + 165), Color.White);
        }

        public void Exit()
        {            
        }
    }
}
