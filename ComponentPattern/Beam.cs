using Microsoft.Xna.Framework;

namespace Kaiju.ComponentPattern
{
    /// <summary>
    /// Class used to create Godzillas "Atomic Breath"
    /// Is part of Component Design Pattern
    /// Made by: Julius
    /// </summary>
    internal class Beam : Component
    {
        public Player owner;
        public SpriteRenderer spriteRenderer;

        /// <summary>
        /// Constuctor 
        /// </summary>
        /// <param name="gameObject">specifies which gameobject it is tied to</param>
        public Beam(GameObject gameObject) : base(gameObject)
        {
        }

        /// <summary>
        /// Updates the beams positions and direction every frame based on owners position
        /// </summary>
        public override void Update()
        {
            gameObject.Transform.Position = owner.gameObject.Transform.Position + new Vector2(owner.facingRight ? 300 : -300, -25);
            spriteRenderer.SetFlipHorizontal(!owner.facingRight);
        }
    }
}
