using Kaiju.ComponentPattern;
using Microsoft.Xna.Framework;

namespace Kaiju.Command
{
    /// <summary>
    /// Represents a command that triggers a Move action for a specific player
    /// Implements the ICommand interface as part of the Command Design Pattern
    /// Made by: Mads
    /// </summary>
    public class MoveCommand : ICommand
    {
        private Vector2 velocity;
        private Player player;

        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="player">The player who will perform the command</param>
        /// <param name="velocity">Specifies the direction of the movement</param>
        public MoveCommand(Player player, Vector2 velocity)
        {
            this.player = player;
            this.velocity = velocity;
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        public void Execute()
        {
            player.Move(velocity);
        }
    }
}
