using Kaiju.ComponentPattern;
using Microsoft.Xna.Framework;

namespace Kaiju.Command
{
    public class MoveCommand : ICommand
    {
        private Vector2 velocity;
        private Player player;

        public MoveCommand(Player player, Vector2 velocity)
        {
            this.player = player;
            this.velocity = velocity;
        }

        public void Execute()
        {
            player.Move(velocity);
        }
    }
}
