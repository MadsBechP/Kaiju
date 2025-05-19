using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public VictoryState(GameWorld game)
        {
            this.game = game;
        }
        
        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

    }
}
