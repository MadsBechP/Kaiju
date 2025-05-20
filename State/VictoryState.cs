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
    public class VictoryState : IGameState
    {
        private GameWorld game;
        private SpriteFont victoryFont;
        private string winText;
        private string text;

        public Color BackgoundColor => Color.DarkSlateGray;

        public VictoryState(GameWorld game, string winnerName)
        {
            this.game = game;
            winText = $"A New King Has Risen!\nKneel Before {winnerName}";
            victoryFont = game.Content.Load<SpriteFont>("VictoryFont");
            text = "press ENTER to start new battle";

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
            
            spriteBatch.DrawString(victoryFont, text, new Vector2(500,500), Color.White);
        }

        public void Exit()
        {            
        }
    }
}
