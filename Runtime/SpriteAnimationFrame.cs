using System;
using UnityEngine;

namespace Popcron.Animations
{
    [Serializable]
    public class SpriteAnimationFrame
    {
        [SerializeField]
        private Sprite sprite;

        [SerializeField]
        private float duration = 0.1f;

        /// <summary>
        /// The sprite that this frame uses.
        /// </summary>
        public Sprite Sprite
        {
            get => sprite;
            set => sprite = value;
        }

        /// <summary>
        /// The duration of this frame in seconds.
        /// </summary>
        public float Duration
        {
            get => duration;
            set => duration = Mathf.Clamp(value, 0, float.MaxValue);
        }
    }
}