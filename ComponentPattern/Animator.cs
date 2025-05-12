using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju.ComponentPattern
{
    public class Animator : Component
    {
        public int CurrentIndex { get; private set; }
        private float elapsed;
        private SpriteRenderer sr;
        private Dictionary<string, Animation> animations = new();
        private Animation currentAnimation;

        public Animator(GameObject gameObject) : base(gameObject)
        {
            sr = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
        }

        public override void Start()
        {
            if (currentAnimation != null)
            {
                sr.Sprite = currentAnimation.SpriteSheet;
                sr.Source = currentAnimation.Frames[0];
            }
        }

        public override void Update()
        {
            if (currentAnimation == null)
            {
                return;
            }
            elapsed += GameWorld.Instance.DeltaTime;
            CurrentIndex = (int)(elapsed * currentAnimation.FPS);

            if (CurrentIndex > currentAnimation.Frames.Length - 1)
            {
                elapsed = 0;
                CurrentIndex = 0;
            }

            sr.Sprite = currentAnimation.SpriteSheet;
            sr.Source = currentAnimation.Frames[CurrentIndex];
        }

        public void AddAnimation(Animation animation)
        {
            animations.Add(animation.Name, animation);
            if (currentAnimation == null)
            {
                currentAnimation = animation;
                sr.Sprite = currentAnimation.SpriteSheet;
                sr.Source = currentAnimation.Frames[0];
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
        public Texture2D SpriteSheet { get; private set; }
        public Rectangle[] Frames { get; private set; }
        public Animation(string name, Texture2D spriteSheet, Rectangle[] frames, float fps)
        {
            this.Name = name;
            this.SpriteSheet = spriteSheet;
            this.Frames = frames;
            this.FPS = fps;
        }
    }
}
