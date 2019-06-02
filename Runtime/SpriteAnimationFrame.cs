using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Popcron.Animations
{
    [Serializable]
    public class SpriteAnimationFrame
    {
        [SerializeField]
        [FormerlySerializedAs("_sprite")]
        private Sprite sprite;

        [SerializeField]
        private float duration = 0.1f;

        public Sprite Sprite => sprite;
        public float Duration
        {
            get
            {
                return duration;
            }
            set
            {
                duration = value;
            }
        }
    }
}