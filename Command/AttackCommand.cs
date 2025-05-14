using Kaiju.ComponentPattern;

namespace Kaiju.Command
{
    public class AttackCommand : ICommand
    {
        private Player player;
        private int atkNumber;

        public AttackCommand(Player player, int atkNumber)
        {
            this.player = player;
            this.atkNumber = atkNumber;
        }

        public void Execute()
        {
            player.Attack(atkNumber);
        }
    }
}
