using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Kaiju.ComponentPattern
{
    /// <summary>
    /// Controls sprite animations for a GameObject by cycling through frames at a given frame rate
    /// Manages animation transitions, playback control, and frame-specific event callbacks
    /// Based on the Animator Design Pattern
    /// Is part of Component Design Pattern
    /// Made by: Mads & Julius
    /// </summary>
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

        /// <summary>
        /// Constuctor 
        /// </summary>
        /// <param name="gameObject">specifies which gameobject it is tied to</param>
        public Animator(GameObject gameObject) : base(gameObject)
        {
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
        }

        /// <summary>
        /// Updates the animation state based on elapsed time
        /// Handles frame changes, triggers frame-specific events, and transitions animations based on their settings
        /// </summary>
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

        /// <summary>
        /// Adds a new animation to the Animator's animation list
        /// </summary>
        /// <param name="animation">The specific animation to add</param>
        public void AddAnimation(Animation animation)
        {
            animations.Add(animation.Name, animation);
            if (currentAnimation == null)
            {
                currentAnimation = animation;
            }

            if (!animations.ContainsKey(animation.Name))
            {
                animations.Add(animation.Name, animation);
                if (currentAnimation == null)
                {
                    currentAnimation = animation;
                }
            }
        }

        /// <summary>
        /// Plays an animation based on name
        /// Supports held animations that should continue playing as long as they are held
        /// </summary>
        /// <param name="animationName">The name of the animation to play</param>
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

        /// <summary>
        /// Can do specific actions on specific frames of animations
        /// Used primarily to spawn hitboxes on the right frames of attack animations
        /// </summary>
        /// <param name="animationName">The name of the animation</param>
        /// <param name="frameIndex">The specific frame the action should take place on</param>
        /// <param name="callback">The method to call when the frame is reached</param>
        public void RegisterFrameEvent(string animationName, int frameIndex, System.Action callback)
        {
            if (!animationEvents.ContainsKey(animationName))
            {
                animationEvents[animationName] = new Dictionary<int, System.Action>();
            }

            animationEvents[animationName][frameIndex] = callback;
        }
    }

    /// <summary>
    /// Represents single animations and their information
    /// Based on the Animator Design Pattern
    /// Made by: Julius
    /// </summary>
    public class Animation
    {
        public float FPS { get; private set; }
        public string Name { get; private set; }
        public Texture2D[] Sprites { get; private set; }
        public bool HeldAnimation { get; private set; }

        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="name">Name of the animation</param>
        /// <param name="sprites">What sprites are part of the animation</param>
        /// <param name="fps">The number of frames the animation should play at</param>
        /// <param name="heldAnimation">Value indicating whether the animation should loop until interrupted</param>
        public Animation(string name, Texture2D[] sprites, float fps, bool heldAnimation)
        {
            Name = name;
            Sprites = sprites;
            FPS = fps;
            HeldAnimation = heldAnimation;
        }
    }
}
