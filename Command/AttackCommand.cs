using Kaiju.ComponentPattern;

namespace Kaiju.Command
{
    /// <summary>
    /// Represents a command that triggers an attack action for a specific player
    /// Implements the ICommand interface as part of the Command Design Pattern
    /// Made by: Mads
    /// </summary>
    public class AttackCommand : ICommand
    {
        private Player player;
        private int atkNumber;

        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="player">The player who will perform the attack</param>
        /// <param name="atkNumber">The identifier for the specific attack to perform</param>
        public AttackCommand(Player player, int atkNumber)
        {
            this.player = player;
            this.atkNumber = atkNumber;
        }

        /// <summary>
        /// Executes the command, instructing the player to perform the specified attack based on atkNumber
        /// </summary>
        public void Execute()
        {
            player.Attack(atkNumber);
        }
    }
}
