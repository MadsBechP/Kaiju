using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kaiju.State
{
    public interface IGameState
    {
        Color DefaultBackgroundColor { get; }
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
        void Exit();
        void OnControllerConnectionChanged(bool p1Connected, bool p2Connected);
    }
}
