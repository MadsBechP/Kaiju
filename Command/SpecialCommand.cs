using Kaiju.ComponentPattern;

namespace Kaiju.Command
{
    public class SpecialCommand : ICommand
    {
        private Player player;
        private int specialNumber;

        public SpecialCommand(Player player, int specialNumber)
        {
            this.player = player;
            this.specialNumber = specialNumber;
        }

        public void Execute()
        {
            player.Special(specialNumber);
        }
    }
}
