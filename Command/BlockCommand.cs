using Kaiju.ComponentPattern;

namespace Kaiju.Command
{
    /// <summary>
    /// Represents a command that triggers a block action for a specific player
    /// Implements the ICommand interface as part of the Command Design Pattern
    /// Made by: Mads
    /// </summary>
    public class BlockCommand : ICommand
    {
        private Player player;

        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="player">The player who will perform the command</param>
        public BlockCommand(Player player)
        {
            this.player = player;
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        public void Execute()
        {
            player.Block();
        }
    }
}
