using Microsoft.Xna.Framework;

namespace Kaiju
{
    public class Camera
    {
        public Vector2 Center { get; set; }

        private Vector2 _quarterScreen { get { return new Vector2(GameWorld.Instance.GraphicsDevice.Viewport.Width / 2, GameWorld.Instance.GraphicsDevice.Viewport.Height / 2); } }
        /// <summary>
        /// Gets top left of screen
        /// </summary>
        public Vector2 GetTopLeft() => Center - _quarterScreen;

        /// <summary>
        /// Moves camera towards center of both players
        /// </summary>
        /// <param name="deltaTimeInMs">How long since last update</param>
        /// <param name="movePercentage">How far to move</param>
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
