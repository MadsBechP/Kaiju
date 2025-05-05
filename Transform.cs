using Microsoft.Xna.Framework;

namespace Kaiju
{
    public class Transform
    {
        public Vector2 Position { get; set; }

        public Vector2 Scale { get; set; } = new Vector2(1, 1);

        public float Rotation { get; set; }

        public void Translate(Vector2 translation)
        {
            Position += translation;
        }
    }
}
