namespace Kaiju.ComponentPattern.Characters
{
    /// <summary>
    /// Base class for all character components in the game.
    /// Inherits from component
    /// Made by: Mads
    /// </summary>
    public class Character : Component
    {
        protected SpriteRenderer sr;
        protected Animator ani;

        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="gameObject">specifies which gameobject it is tied to</param>
        public Character(GameObject gameObject) : base(gameObject)
        {
        }

        /// <summary>
        /// Called once at the beginning of the component's lifecycle
        /// Retrieves and stores references to the SpriteRenderer and Animator components
        /// </summary>
        public override void Start()
        {
            sr = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            ani = gameObject.GetComponent<Animator>() as Animator;
        }

        /// <summary>
        /// Used to define the direction the character faces
        /// Implemented because Gigan and Godzillas sprites faces different directions
        /// </summary>
        /// <param name="x">If true, character should face right; otherwise, face left</param>
        public virtual void FaceRight(bool x)
        {
        }
    }
}
