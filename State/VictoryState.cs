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
        private SpriteFont victoryFont;
        private string winText;
        private string promptText;
        private bool isDraw;
        
        public Color BackgoundColor => Color.DarkSlateGray;

        public VictoryState(GameWorld game, string winnerName, bool isDraw)
        {
            this.game = game;
            
            victoryFont = game.Content.Load<SpriteFont>("VictoryFont");
            promptText = "press ENTER to start new battle";
            

            if (isDraw)
            {
                winText = "You Are Too Weak To Be King";
            }
            else
            {
                winText = $"A New King Has Risen !\n  Kneel Before {winnerName}";
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
            var width = GameWorld.Instance.Graphics.PreferredBackBufferWidth;
            var height = GameWorld.Instance.Graphics.PreferredBackBufferHeight;
                        

            spriteBatch.DrawString(victoryFont, winText, new Vector2((width / 2), (height / 2)-100), Color.White);;

            spriteBatch.DrawString(victoryFont, promptText, new Vector2((width / 2), (height / 2)+50), Color.White);
        }

        public void Exit()
        {            
        }
    }
}
