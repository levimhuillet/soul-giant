using System;
using System.Collections;
using BeauRoutine;
using BeauUtil;
using BeauUtil.Debugger;
using UnityEngine;

namespace SoulGiant {
    public class ActivateOnTrigger : MonoBehaviour {
        [Required] public Trigger Trigger;
        [Required] public GameObject Target;
        public bool Invert;

        private void Awake() {
            Trigger.OnStateChanged.Register(OnTrigger);
            Target.SetActive(Trigger.State ^ Invert);
        }

        private void OnTrigger(Trigger trigger) {
            Target.SetActive(Trigger.State ^ Invert);
        }
    }
}