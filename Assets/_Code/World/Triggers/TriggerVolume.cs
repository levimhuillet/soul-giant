using System;
using System.Collections;
using BeauRoutine;
using BeauUtil;
using BeauUtil.Debugger;
using UnityEngine;

namespace SoulGiant {
    [RequireComponent(typeof(Collider2D))]
    public class TriggerVolume : Trigger {
        public LayerMask Mask = (1 << 10);

        private void OnTriggerEnter2D(Collider2D collider) {
            if (!Bits.Contains(Mask, collider.gameObject.layer)) {
                return;
            }

            Invoke();
        }

        private void OnTriggerExit2D(Collider2D collider) {
            if (!m_OnlyOnce) {
                ResetState();
            }
        }
    }
}