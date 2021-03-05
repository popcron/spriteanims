using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Popcron.Animations
{
    [Serializable]
    public class SpriteAnimationClip : IEnumerable<SpriteAnimationFrame>
    {
        [SerializeField]
        private string name = "Animation name";

        [SerializeField]
        private bool loop = true;

        [SerializeField]
        private List<SpriteAnimationFrame> frames = new List<SpriteAnimationFrame>();

        /// <summary>
        /// The name of this animation clip.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// The length in frames of this animation.
        /// </summary>
        public int Length => frames.Count;

        /// <summary>
        /// Should this animation loop?
        /// </summary>
        public bool Loop => loop;

        /// <summary>
        /// All of the animation frames.
        /// </summary>
        public ReadOnlyCollection<SpriteAnimationFrame> Frames => frames.AsReadOnly();

        public SpriteAnimationFrame this[int index]
        {
            get => (index >= 0 && index < frames.Count) ? frames[index] : null;
        }

        public IEnumerator<SpriteAnimationFrame> GetEnumerator() => ((IEnumerable<SpriteAnimationFrame>)frames).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)frames).GetEnumerator();
    }
}
