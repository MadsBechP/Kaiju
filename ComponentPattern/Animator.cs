using Kaiju.ComponentPattern;
using Kaiju;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.ComponentPattern
{
    public class Animator : Component
    {
        public int CurrentIndex { get; private set; }
        private float elapsed;
        private SpriteRenderer spriteRenderer;
        private Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
        private Animation currentAnimation;

        public Animator(GameObject gameObject) : base(gameObject)
        {
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
        }

        public override void Update()
        {
            elapsed += GameWorld.Instance.DeltaTime;
            CurrentIndex = (int)(elapsed * currentAnimation.FPS);

            if (CurrentIndex > currentAnimation.Sprites.Length - 1)
            {
                elapsed = 0;
                CurrentIndex = 0;
            }
            spriteRenderer.Sprite = currentAnimation.Sprites[CurrentIndex];
        }

        public void AddAnimation(Animation animation)
        {
            animations.Add(animation.Name, animation);
            if (currentAnimation == null)
            {
                currentAnimation = animation;
            }
        }

        public void PlayAnimation(string animationName)
        {
            if (animationName != currentAnimation.Name)
            {
                currentAnimation = animations[animationName];
                elapsed = 0;
                CurrentIndex = 0;
            }
        }
    }

    public class Animation
    {
        public float FPS { get; private set; }
        public string Name { get; private set; }
        public Texture2D[] Sprites { get; private set; }
        public Animation(string name, Texture2D[] sprites, float fps)
        {
            this.Name = name;
            this.Sprites = sprites;
            this.FPS = fps;
        }

    }
}
