using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor.Profiling;
using UnityEngine;
using UnityEngine.UI;

namespace Popcron.Animations
{
    [AddComponentMenu("Popcron/Animations/Sprite Animator")]
    public class SpriteAnimator : MonoBehaviour
    {
        /// <summary>
        /// Global pause flag.
        /// </summary>
        public static bool GlobalPause { get; set; } = false;

        [SerializeField]
        private string defaultAnimation = "Animation";

        [SerializeField]
        private float speed = 1f;

        [SerializeField]
        private List<SpriteAnimationClip> animations = new List<SpriteAnimationClip>();

        private SpriteRenderer spriteRenderer;
        private Image image;
        private SpriteAnimationClip currentAnimation;
        private float nextFrame;
        private bool inReverse;
        private int frame;
        private bool finishedPlaying;
        private bool isPaused;
        private float time;
        private CancellationTokenSource cts;

        /// <summary>
        /// Is this individual animator paused?
        /// </summary>
        public bool IsPaused
        {
            get => isPaused;
            set => isPaused = value;
        }

        /// <summary>
        /// The playback speed of the animator.
        /// </summary>
        public float Speed
        {
            get => speed;
            set => speed = Mathf.Clamp(speed, 0, float.MaxValue);
        }

        /// <summary>
        /// The animation clips on this animator.
        /// </summary>
        public ReadOnlyCollection<SpriteAnimationClip> Animations => animations.AsReadOnly();

        /// <summary>
        /// The sprite renderer that this animator is using.
        /// </summary>
        public SpriteRenderer SpriteRenderer
        {
            get => spriteRenderer;
            set => spriteRenderer = value;
        }

        /// <summary>
        /// The current animation being played
        /// </summary>
        public string CurrentAnimation
        {
            get
            {
                if (currentAnimation != null)
                {
                    return currentAnimation.Name;
                }
                else
                {
                    return null;
                }
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
                    nextFrame = 0f;
                }
            }
        }

        /// <summary>
        /// The current frame being shown.
        /// </summary>
        public SpriteAnimationFrame CurrentFrame
        {
            get
            {
                if (currentAnimation is null)
                {
                    return null;
                }

                return currentAnimation[frame];
            }
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            image = GetComponent<Image>();
            CurrentAnimation = defaultAnimation;
        }

        /// <summary>
        /// Loads these animation clips and stop all animation.
        /// </summary>
        public void Set(IList<SpriteAnimationClip> animations)
        {
            this.animations = animations.ToList();
            Stop();
        }

        private void OnValidate()
        {
            if (speed < 0)
            {
                speed = 0;
            }
        }

        private void OnDisable()
        {
            if (cts != null)
            {
                cts.Cancel();
            }
        }

        private void OnDestroy()
        {
            if (cts != null)
            {
                cts.Cancel();
            }
        }

        private void Update()
        {
            if (GlobalPause || isPaused)
            {
                return;
            }

            float delta = Time.deltaTime;
            time += delta * speed;
            if (currentAnimation != null)
            {
                //play next frame
                if (time > nextFrame)
                {
                    SpriteAnimationFrame current = currentAnimation[frame];
                    if (current != null)
                    {
                        if (spriteRenderer)
                        {
                            spriteRenderer.sprite = current.Sprite;
                        }

                        if (image)
                        {
                            image.sprite = current.Sprite;
                        }

                        float frameDuration = current.Duration;
                        nextFrame = time + frameDuration;

                        if (inReverse)
                        {
                            frame--;
                        }
                        else
                        {
                            frame++;
                        }

                        //loop frames
                        if (frame >= currentAnimation.Length && !inReverse)
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
                        else if (frame < 0 && inReverse)
                        {
                            if (currentAnimation.Loop)
                            {
                                frame = currentAnimation.Length - 1;
                                finishedPlaying = false;
                            }
                            else
                            {
                                frame = 0;
                                finishedPlaying = true;
                            }
                        }
                    }
                    else
                    {
                        //frame is null, probably out of range
                        frame = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Stops playing the current animation.
        /// </summary>
        public void Stop()
        {
            nextFrame = 0f;
            finishedPlaying = true;
        }

        /// <summary>
        /// Returns true if an animation clip with this name exists.
        /// </summary>
        public bool Exists(string animationName)
        {
            foreach (SpriteAnimationClip animation in Animations)
            {
                if (animation.Name == animationName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns an animation clip by its name on this animator.
        /// </summary>
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

        /// <summary>
        /// Starts playing this animation.
        /// </summary>
        public void Play(string animationName, bool inReverse = false)
        {
            SpriteAnimationClip newAnimation = Get(animationName);
            if (currentAnimation != newAnimation || this.inReverse != inReverse)
            {
                currentAnimation = Get(animationName);
                frame = 0;
                finishedPlaying = false;
                nextFrame = 0f;
                this.inReverse = inReverse;
            }
        }

        /// <summary>
        /// Starts playing this animation as an async task.
        /// </summary>
        public async Task PlayTaskAsync(string animationName, bool inReverse = false)
        {
            cts = new CancellationTokenSource();

            //set the animation clip
            currentAnimation = Get(animationName);
            frame = 0;
            finishedPlaying = false;
            nextFrame = 0f;
            this.inReverse = inReverse;

            //wait for the animation to finish playing
            while (!finishedPlaying)
            {
                if (cts != null && cts.Token.IsCancellationRequested == true)
                {
                    return;
                }

                await Task.Delay(1);
            }
        }
    }
}
