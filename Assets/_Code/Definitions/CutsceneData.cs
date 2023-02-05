using System;
using UnityEngine;

namespace SoulGiant {
    [CreateAssetMenu (fileName = "NewCutsceneData", menuName = "SoulGiant/CutsceneData")]
    public class CutsceneData : ScriptableObject {
        [Serializable]
        public struct Frame {
            public Sprite Image;
            public string Text;
            public float Delay;
        }

        public Frame[] Frames;
    }
}