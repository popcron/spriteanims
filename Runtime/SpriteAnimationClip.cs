using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Popcron.Animations
{
    [Serializable]
    public class SpriteAnimationClip
    {
        [SerializeField]
        [FormerlySerializedAs("_name")]
        private string name = "Animation name";

        [SerializeField]
        [FormerlySerializedAs("_frames")]
        private List<SpriteAnimationFrame> frames = new List<SpriteAnimationFrame>();

        [SerializeField]
        [FormerlySerializedAs("_loop")]
        private bool loop = true;

        public string Name => name;
        public int Length => frames.Count;
        public bool Loop => loop;
        public List<SpriteAnimationFrame> Frames => frames;
        public SpriteAnimationFrame this[int index] => (index >= 0 && index < frames.Count) ? frames[index] : null;
    }
}
