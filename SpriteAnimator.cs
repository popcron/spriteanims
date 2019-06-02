using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Popcron.Animations
{
    public class SpriteAnimator : Entity
    {
        [SerializeField]
        [FormerlySerializedAs("_defaultAnimation")]
        private string defaultAnimation = "Animation";

        [SerializeField]
        [FormerlySerializedAs("_animations")]
        private List<SpriteAnimationClip> animations = new List<SpriteAnimationClip>();

        private SpriteRenderer spriteRenderer;
        private Image image;
        private SpriteAnimationClip currentAnimation;
        private float nextFrame;
        private int frame;
        private bool finishedPlaying;
        private CancellationTokenSource cts;

        public List<SpriteAnimationClip> Animations => animations;

        /// <summary>
        /// The current animation being played
        /// </summary>
        public string Animation
        {
            get
            {
                return currentAnimation?.Name;
            }
            set
            {
                SpriteAnimationClip lastAnimation = currentAnimation;
                SpriteAnimationClip newAnimation = Get(value);
                if (newAnimation != null)
                {
                    currentAnimation = newAnimation;
                }

                if (currentAnimation != lastAnimation)
                {
                    //animation state changed
                    frame = 0;
                    finishedPlaying = false;
                    nextFrame = Time.time + 0f;
                }
            }
        }

        public override void OnAwake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            image = GetComponent<Image>();
            Animation = defaultAnimation;
        }

        public override void OnDisabled()
        {
            cts?.Cancel();
        }

        public override void OnUpdate()
        {
            if (Game.IsPaused) return;

            if (currentAnimation != null)
            {
                //play next frame
                if (Time.time > nextFrame)
                {
                    if (spriteRenderer) spriteRenderer.sprite = currentAnimation[frame].Sprite;
                    if (image) image.sprite = currentAnimation[frame].Sprite;

                    float frameDuration = currentAnimation[frame].Duration;
                    nextFrame = Time.time + frameDuration;
                    frame++;

                    //loop frames
                    if (frame >= currentAnimation.Length)
                    {
                        if (currentAnimation.Loop)
                        {
                            frame = 0;
                            finishedPlaying = false;
                        }
                        else
                        {
                            frame = currentAnimation.Length - 1;
                            finishedPlaying = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns an animation by its name on this animator
        /// </summary>
        /// <param name="animationName"></param>
        /// <returns></returns>
        public SpriteAnimationClip Get(string animationName)
        {
            foreach (SpriteAnimationClip animation in Animations)
            {
                if (animation.Name == animationName)
                {
                    return animation;
                }
            }

            return null;
        }

        public async Task Play(string animationName)
        {
            cts = new CancellationTokenSource();
            string lastAnimation = Animation;
            Animation = animationName;

            if (currentAnimation != null)
            {
                float duration = currentAnimation[currentAnimation.Length - 1].Duration;
                await Delay.Wait(duration, cts.Token);
                while (!finishedPlaying)
                {
                    if (cts?.Token.IsCancellationRequested == true)
                    {
                        return;
                    }

                    if (Animation != animationName)
                    {
                        return;
                    }
                    await Delay.Wait(1);
                }
            }

            Animation = lastAnimation;
        }
    }
}