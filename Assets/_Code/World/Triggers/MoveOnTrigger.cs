using System;
using System.Collections;
using BeauRoutine;
using BeauUtil;
using BeauUtil.Debugger;
using UnityEngine;

namespace SoulGiant {
    public class MoveOnTrigger : MonoBehaviour {
        [Required] public Trigger Trigger;

        [Required] public Transform Target;
        [AutoEnum] public TransformProperties Properties = TransformProperties.All;
        public TweenSettings TransitionTo = new TweenSettings(1, Curve.CubeIn);
        public TweenSettings TransitionBack = new TweenSettings(1, Curve.CubeIn);

        [NonSerialized] private TransformState m_OriginalState;
        [NonSerialized] private TransformState m_TargetState;

        [NonSerialized] private Curve m_CurrentCurve;
        [NonSerialized] private float m_CurrentPos;
        private Routine m_Anim;

        private void Awake() {
            m_OriginalState = TransformState.WorldState(transform);
            m_TargetState = TransformState.WorldState(Target);

            Trigger.OnStateChanged.Register(OnTrigger);
        }

        private void OnTrigger(Trigger trigger) {
            if (trigger.State) {
                m_CurrentCurve = TransitionTo.Curve;
                float duration = TransitionTo.Time * (1 - m_CurrentPos);
                m_Anim.Replace(this, Tween.Float(m_CurrentPos, 1, SetPosition, duration));
            } else {
                m_CurrentCurve = TransitionBack.Curve;
                float duration = TransitionBack.Time * m_CurrentPos;
                m_Anim.Replace(this, Tween.Float(m_CurrentPos, 0, SetPosition, duration));
            }
        }

        private void SetPosition(float pos) {
            TransformState newState = TransformState.Lerp(m_OriginalState, m_TargetState, m_CurrentCurve.Evaluate(pos));
            newState.Apply(transform, Properties);
            m_CurrentPos = pos;
        }
    }
}