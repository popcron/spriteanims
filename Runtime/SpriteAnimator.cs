using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

namespace Popcron.Animations
{
    [AddComponentMenu("Popcron/Animations/Sprite Animator")]
    public class SpriteAnimator : MonoBehaviour
    {
        [SerializeField]
        private string defaultAnimation = "Animation";

        [SerializeField]
        private List<SpriteAnimationClip> animations = new List<SpriteAnimationClip>();

        private SpriteRenderer spriteRenderer;
        private Image image;
        private SpriteAnimationClip currentAnimation;
        private float nextFrame;
        private int frame;
        private bool finishedPlaying;
        private CancellationTokenSource cts;

        public List<SpriteAnimationClip> Animations 
		{
			get
			{
				return animations;
			}
		}
		
		public SpriteRenderer SpriteRenderer
		{
			get
			{
				return spriteRenderer;
			}
			set
			{
				spriteRenderer = value;
			}
		}

        /// <summary>
        /// The current animation being played
        /// </summary>
        public string Animation
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
                    nextFrame = Time.time + 0f;
                }
            }
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            image = GetComponent<Image>();
            Animation = defaultAnimation;
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
            if (currentAnimation != null)
            {
                //play next frame
                if (Time.time > nextFrame)
                {
                    if (spriteRenderer) 
					{
						spriteRenderer.sprite = currentAnimation[frame].Sprite;
                    }
					if (image) 
					{
						image.sprite = currentAnimation[frame].Sprite;
					}
					
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
        /// Returns an animation clip by its name on this animator
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

        public async Task Play(string animationName)
        {
            cts = new CancellationTokenSource();
            string lastAnimation = Animation;

            //set the animation clip
            currentAnimation = Get(animationName);
            frame = 0;
            finishedPlaying = false;
            nextFrame = Time.time + 0f;

            //wait for the animation to finish playing
            while (!finishedPlaying)
            {
                if (cts != null && cts.Token.IsCancellationRequested == true)
                {
                    return;
                }

                await Task.Delay(1);
            }

            //play the previous animation
            Animation = lastAnimation;
        }
    }
}