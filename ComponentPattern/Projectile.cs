using Microsoft.Xna.Framework;

namespace Kaiju.ComponentPattern
{
    /// <summary>
    /// Class used to create Gigan beam attack
    /// Is part of Component Design Pattern
    /// Made by: Julius
    /// </summary>
    internal class Projectile : Component
    {
        public Vector2 direction;
        public float speed;
        public Player owner;

        /// <summary>
        /// Constuctor 
        /// </summary>
        /// <param name="gameObject">specifies which gameobject it is tied to</param>
        public Projectile(GameObject gameObject) : base(gameObject)
        {
        }

        /// <summary>
        /// Updates the beams positions and direction every frame based on owners position
        /// </summary>
        public override void Update()
        {
            gameObject.Transform.Position += direction * speed * GameWorld.Instance.DeltaTime;
        }
    }
}
