using Kaiju.ComponentPattern;

namespace Kaiju.Command
{
    public class BlockCommand : ICommand
    {
        private Player player;

        public BlockCommand(Player player)
        {
            this.player = player;
        }

        public void Execute()
        {
            player.Block();
        }
    }
}
