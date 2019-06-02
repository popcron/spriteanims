using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dinky.Animations
{
    [Serializable]
    public class PlayerAnimation
    {
        [Serializable]
        public class PlayerFrame
        {
            [SerializeField]
            private Sprite _sprite;

            [SerializeField]
            private float duration = 0.1f;

            [SerializeField]
            private float minPaintY = -32f;

            [SerializeField]
            private float maxPaintY = 0f;

            public float Min => minPaintY;
            public float Max => maxPaintY;
            public Sprite Sprite => _sprite;
            public float Duration => duration;
        }

        [SerializeField]
        private string _name = "Animation name";

        [SerializeField]
        private List<PlayerFrame> _frames = new List<PlayerFrame>();

        [SerializeField]
        private bool _loop = true;

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public int Length
        {
            get
            {
                return _frames.Count;
            }
        }

        public bool Loop
        {
            get
            {
                return _loop;
            }
        }

        public PlayerFrame this[int index]
        {
            get
            {
                if (_frames.Count <= index) return null;
                if (index < 0) return null;

                return _frames[index];
            }
        }
    }
}