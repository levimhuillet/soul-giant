using System;
using System.Collections;
using BeauRoutine;
using BeauUtil;
using BeauUtil.Debugger;
using UnityEngine;

namespace SoulGiant {
    public abstract class Trigger : MonoBehaviour {
        [SerializeField] protected bool m_OnlyOnce = false;

        [NonSerialized] public bool State;
        public readonly CastableEvent<Trigger> OnStateChanged = new CastableEvent<Trigger>(4);

        public bool Invoke() {
            if (State && m_OnlyOnce) {
                return false;
            }

            State = true;
            OnStateChanged.Invoke(this);
            return true;
        }

        public bool ResetState() {
            if (State) {
                State = false;
                OnStateChanged.Invoke(this);
                return true;
            }

            return false;
        }
    }
}