using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Kaiju.ComponentPattern
{
    public class Animator : Component
    {
        public int CurrentIndex { get; private set; }
        private float elapsed;
        private SpriteRenderer spriteRenderer;
        private Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
        private Animation currentAnimation;
        private bool held = false;

        private Dictionary<string, Dictionary<int, System.Action>> animationEvents = new();
        private int lastFrameIndex = -1;

        public Animator(GameObject gameObject) : base(gameObject)
        {
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
        }

        public override void Update()
        {
            elapsed += GameWorld.Instance.DeltaTime;
            CurrentIndex = (int)(elapsed * currentAnimation.FPS);

            if (CurrentIndex != lastFrameIndex)
            {
                lastFrameIndex = CurrentIndex;
                if (animationEvents.TryGetValue(currentAnimation.Name, out var frameEvents))
                {
                    if (frameEvents.TryGetValue(CurrentIndex, out var callback))
                    {
                        callback.Invoke();
                    }
                }
            }

            if (currentAnimation.HeldAnimation && held && CurrentIndex > currentAnimation.Sprites.Length - 1)
            {
                elapsed = 0;
                CurrentIndex = 0;
            }
            else if (currentAnimation.HeldAnimation && !held || CurrentIndex > currentAnimation.Sprites.Length - 1)
            {
                elapsed = 0;
                CurrentIndex = 0;
                if (currentAnimation.Name != "Breath" && currentAnimation.Name != "SawCont")
                {
                    PlayAnimation("Idle");
                }
            }
            spriteRenderer.Sprite = currentAnimation.Sprites[CurrentIndex];
            spriteRenderer.Source = new Rectangle(0, 0, spriteRenderer.Sprite.Width, spriteRenderer.Sprite.Height);
            held = false;
        }

        public void AddAnimation(Animation animation)
        {
            //animations.Add(animation.Name, animation);
            //if (currentAnimation == null)
            //{
            //    currentAnimation = animation;
            //}

            if (!animations.ContainsKey(animation.Name))
            {
                animations.Add(animation.Name, animation);
                if(currentAnimation == null)
                {
                    currentAnimation = animation;
                }
            }
        }

        public void PlayAnimation(string animationName)
        {
            if (animationName != "Walk" || (animationName == "Walk" && (currentAnimation.Name == "Idle" || currentAnimation.Name == "Walk")))
            {
                if (animationName != currentAnimation.Name)
                {
                    currentAnimation = animations[animationName];
                    elapsed = 0;
                    CurrentIndex = 0;
                }
                if (currentAnimation.HeldAnimation)
                {
                    held = true;
                }
            }
        }

        public void RegisterFrameEvent(string animationName, int frameIndex, System.Action callback)
        {
            if (!animationEvents.ContainsKey(animationName))
            {
                animationEvents[animationName] = new Dictionary<int, System.Action>();
            }

            animationEvents[animationName][frameIndex] = callback;
        }
    }

    public class Animation
    {
        public float FPS { get; private set; }
        public string Name { get; private set; }
        public Texture2D[] Sprites { get; private set; }
        public bool HeldAnimation { get; private set; }
        public Animation(string name, Texture2D[] sprites, float fps, bool heldAnimation)
        {
            Name = name;
            Sprites = sprites;
            FPS = fps;
            HeldAnimation = heldAnimation;
        }

    }
}
