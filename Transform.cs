using Kaiju.ComponentPattern;
using Microsoft.Xna.Framework;
using System;

namespace Kaiju
{
    public class Transform
    {
        public Vector2 Position { get; set; }

        public Vector2 Scale { get; set; } = new Vector2(1, 1);

        public float Rotation { get; set; }
        public Vector2 CurrentVelocity { get; set; }

        public void AddVelocity(Vector2 translation)
        {
            CurrentVelocity += translation;
        }
        public void Translate(Vector2 translation)
        {
            Position += translation;
        }
        public void Translate(Collider col)
        {
            TranslateX(col);
            TranslateY(col);
        }

        /// <summary>
        /// moves x position in 1 pixel increments and checks for collision, cancels if it collides
        /// </summary>
        public void TranslateX(Collider col)
        {
            int xRemainder = (int)Math.Round(CurrentVelocity.X);
            if (xRemainder != 0)
            {
                int sign = Math.Sign(xRemainder);
                while (xRemainder != 0)
                {
                    Position = new Vector2(Position.X + sign, Position.Y);
                    xRemainder -= sign;

                    if (GameWorld.Instance.CheckCollision(col))
                    {
                        CurrentVelocity = new Vector2(0, CurrentVelocity.Y);
                        Position = new Vector2(Position.X - sign, Position.Y);
                        return;
                    }

                }
            }
        }

        /// <summary>
        /// moves y position in 1 pixel increments and checks for collision, cancels if it collides
        /// </summary>
        public void TranslateY(Collider col)
        {
            if (col.Owner != null)
            {
                col.Owner.grounded = false;
            }

            while (GameWorld.Instance.CheckCollision(col))
            {
                Position = new Vector2(Position.X, Position.Y - 1);
            }

            float yRemainder = (int)Math.Round(CurrentVelocity.Y);
            if (yRemainder != 0)
            {
                int sign = Math.Sign(yRemainder);
                while (yRemainder != 0)
                {
                    Position = new Vector2(Position.X, Position.Y + sign);
                    yRemainder -= sign;

                    if (GameWorld.Instance.CheckCollision(col))
                    {
                        CurrentVelocity = new Vector2(CurrentVelocity.X, 0);
                        Position = new Vector2(Position.X, Position.Y - sign);
                        if (col.Owner != null)
                        {
                            col.Owner.grounded = true;
                        }
                        break;
                    }
                }
            }
        }
    }
}
