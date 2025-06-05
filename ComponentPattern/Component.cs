using Microsoft.Xna.Framework.Graphics;

namespace Kaiju.ComponentPattern
{
    /// <summary>
    /// The main component class other components inherit from
    /// Part of the component Design Pattern
    /// </summary>
    public class Component
    {
        public GameObject gameObject { get; private set; }
        /// <summary>
        /// Constuctor 
        /// </summary>
        /// <param name="gameObject">specifies which gameobject it is tied to</param>
        public Component(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }

        /// <summary>
        /// Initializes the component
        /// Gets run before start
        /// </summary>
        public virtual void Awake()
        {

        }

        /// <summary>
        /// Initializes the component
        /// Gets run after awake
        /// </summary>
        public virtual void Start()
        {

        }

        /// <summary>
        /// The main loop of the component
        /// Updates every frame
        /// </summary>
        public virtual void Update()
        {

        }

        /// <summary>
        /// The main draw loop of the component
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used for drawing</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

        /// <summary>
        /// Handles collision when called
        /// </summary>
        /// <param name="collider">The collider the component collided with</param>
        public virtual void OnCollisionEnter(Collider collider)
        {

        }
    }
}
