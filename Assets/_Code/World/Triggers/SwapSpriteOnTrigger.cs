using System;
using System.Collections;
using BeauRoutine;
using BeauUtil;
using BeauUtil.Debugger;
using UnityEngine;

namespace SoulGiant {
    public class SwapSpriteOnTrigger : MonoBehaviour {
        [Required] public Trigger Trigger;
        [Required] public SpriteRenderer Target;
        public Sprite ScannedSprite;

        private Sprite m_OriginalSprite;

        private void Awake() {
            Trigger.OnStateChanged.Register(OnTrigger);
            m_OriginalSprite = Target.sprite;
        }

        private void OnTrigger(Trigger trigger) {
            Target.sprite = Trigger.State ? ScannedSprite : m_OriginalSprite;
        }
    }
}