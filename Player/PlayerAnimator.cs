using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Dinky.Animations
{
    public class PlayerAnimator : Entity
    {
        [SerializeField]
        private string _defaultAnimation = "Animation";

        [SerializeField]
        private List<PlayerAnimation> _animations = new List<PlayerAnimation>();

        private SpriteRenderer spriteRenderer;
        private Image image;
        private PlayerAnimation currentAnimation;
        private float nextFrame;
        private int frame;
        private bool finishedPlaying;
        private CancellationTokenSource cts;
        private int currentFrame;

        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public List<PlayerAnimation> Animations => _animations;

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
                PlayerAnimation lastAnimation = currentAnimation;
                PlayerAnimation newAnimation = Get(value);
                if (newAnimation != null)
                {
                    currentAnimation = newAnimation;
                }

                if (currentAnimation != lastAnimation)
                {
                    //animation state changed
                    frame = 0;
                    currentFrame = 0;
                    finishedPlaying = false;
                    nextFrame = 0f;
                }
            }
        }

        public PlayerAnimation.PlayerFrame CurrentFrame
        {
            get
            {
                if (currentAnimation == null) return null;

                return currentAnimation[currentFrame];
            }
        }

        public override void OnAwake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            image = GetComponent<Image>();
            Animation = _defaultAnimation;
        }

        public override void OnDisabled()
        {
            cts?.Cancel();
        }

        private void OnDrawGizmos()
        {
            PlayerAnimation.PlayerFrame frame = CurrentFrame;
            if (frame != null)
            {
                Vector2 min = new Vector3(0, frame.Min) + transform.position;
                Gizmos.DrawLine(min + Vector2.left * 16, min + Vector2.right * 16);

                Vector2 max = new Vector3(0, frame.Max) + transform.position;
                Gizmos.DrawLine(max + Vector2.left * 16, max + Vector2.right * 16);
            }
        }

        public override void OnUpdate()
        {
            if (Game.IsPaused) return;

            if (currentAnimation != null)
            {
                //play next frame
                if (Time.time > nextFrame)
                {
                    currentFrame = frame;
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

        public void Stop()
        {
            nextFrame = 0f;
            finishedPlaying = true;
        }

        /// <summary>
        /// Returns an animation by its name on this animator
        /// </summary>
        public PlayerAnimation Get(string animationName)
        {
            foreach (PlayerAnimation animation in Animations)
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