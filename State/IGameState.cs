using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kaiju.State
{
    /// <summary>
    /// Defines the core behavior in all game state.
    /// Each state must implement logic for updating, drawing, and cleaning up resources (if needed),
    /// as well as defining ad default background color.
    /// Made by Emilie
    /// </summary>
    public interface IGameState
    {
        Color DefaultBackgroundColor { get; }

        /// <summary>
        /// Updates the game logic for current state.
        /// </summary>
        /// <param name="gameTime"></param>
        void Update(GameTime gameTime);

        /// <summary>
        /// Draws visual elements used in the current state.
        /// </summary>
        /// <param name="spriteBatch">used to draw</param>
        void Draw(SpriteBatch spriteBatch);

        /// <summary>
        /// Cleans up the state before transitioning to another game state.
        /// </summary>
        void Exit();
    }
}
