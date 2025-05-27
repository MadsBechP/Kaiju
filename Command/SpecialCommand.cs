using Kaiju.ComponentPattern;

namespace Kaiju.Command
{
    /// <summary>
    /// Represents a command that triggers an special action for a specific player
    /// Implements the ICommand interface as part of the Command Design Pattern
    /// Made by: Julius
    /// </summary>
    public class SpecialCommand : ICommand
    {
        private Player player;
        private int specialNumber;

        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="player">The player who will perform the special</param>
        /// <param name="atkNumber">The identifier for the specific special to perform</param>
        public SpecialCommand(Player player, int specialNumber)
        {
            this.player = player;
            this.specialNumber = specialNumber;
        }

        /// <summary>
        /// Executes the command, instructing the player to perform the specified special based on specialNumber
        /// </summary>
        public void Execute()
        {
            player.Special(specialNumber);
        }
    }
}
