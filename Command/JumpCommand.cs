using Kaiju.ComponentPattern;

namespace Kaiju.Command
{
    public class JumpCommand : ICommand
    {
        private Player player;

        public JumpCommand(Player player)
        {
            this.player = player;
        }

        public void Execute()
        {
            player.Jump();
        }
    }
}
