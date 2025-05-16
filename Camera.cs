using Microsoft.Xna.Framework;

namespace Kaiju
{
    internal class Camera
    {
        public Vector2 Center { get; set; }

        private Vector2 _quarterScreen;
        public Vector2 GetTopLeft() => Center - _quarterScreen;

        public Camera(GraphicsDeviceManager graphicsDeviceManager)
        {
            _quarterScreen = new Vector2(graphicsDeviceManager.PreferredBackBufferWidth / 2, graphicsDeviceManager.PreferredBackBufferHeight / 2);
            Center = Vector2.Lerp(GameWorld.Instance.player1Go.Transform.Position, GameWorld.Instance.player2Go.Transform.Position, 0.5f);
        }

        public void MoveToward(float deltaTimeInMs, float movePercentage = .02f)
        {
            Vector2 target = Vector2.Lerp(GameWorld.Instance.player1Go.Transform.Position, GameWorld.Instance.player2Go.Transform.Position, 0.5f);
            
            Vector2 differenceInPosition = target - Center;

            differenceInPosition *= movePercentage;

            var fractionOfPassedTime = deltaTimeInMs / 10;
            
            Center += differenceInPosition * fractionOfPassedTime;

            if ((target - Center).Length() < movePercentage)
            {
                Center = target;
            }
        }
    }
}
