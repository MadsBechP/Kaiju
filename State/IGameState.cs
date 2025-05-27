using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
