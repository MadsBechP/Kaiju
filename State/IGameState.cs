using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.State
{
    public interface IGameState
    {
        Color BackgoundColor { get; }
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
        void Exit();
    }
}
