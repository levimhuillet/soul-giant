using System;
using System.Collections;
using BeauRoutine;
using BeauUtil;
using BeauUtil.Debugger;
using UnityEngine;

namespace SoulGiant {
    public class Interactable : Trigger {

        public enum ButtonType {
            Button,
            Toggle
        }

        public ButtonType Type;
        public CastableEvent<Interactable> OnInteracted = new CastableEvent<Interactable>(1);

        public bool CanInteract() {
            return !State || !m_OnlyOnce || Type == ButtonType.Toggle;
        }

        public void Interact() {
            switch(Type) {
                case ButtonType.Button:
                    Invoke();
                    break;

                case ButtonType.Toggle:
                    if (State) {
                        ResetState();
                    } else {
                        Invoke();
                    }
                    break;
            }

            OnInteracted.Invoke(this);
        }
    }
}