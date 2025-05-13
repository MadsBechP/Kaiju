using Microsoft.Xna.Framework.Graphics;

namespace Kaiju.ComponentPattern
{
    public class Component
    {
        public GameObject gameObject { get; private set; }
        public Component(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }
        public virtual void Awake()
        {

        }
        public virtual void Start()
        {

        }
        public virtual void Update()
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

        public virtual void OnCollisionEnter(Collider collider)
        {

        }
    }
}
