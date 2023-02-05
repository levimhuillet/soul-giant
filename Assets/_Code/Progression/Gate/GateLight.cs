using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulGiant {
    [RequireComponent(typeof(SpriteRenderer))]
    public class GateLight : MonoBehaviour
    {
        private Gate m_ParentGate;

        [SerializeField] private Color m_OffColor, m_OnColor, m_MissedColor;

        private SpriteRenderer m_SR;

        public event EventHandler MissedLight;
        public event EventHandler ReachedLight;

        private bool m_Active;

        #region Unity Callbacks

        private void Awake() {
            m_SR = this.GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {

                if (m_Active) {
                    ReachLight();
                }
            }
        }

        #endregion // Unity Callbacks

        public void Init(Gate parentGate) {
            m_ParentGate = parentGate;
        }

        public void Activate(float duration) {
            StartCoroutine(ShowLight(duration));
        }

        private IEnumerator ShowLight(float duration) {
            m_SR.color = m_OnColor;
            m_Active = true;

            yield return new WaitForSeconds(duration);

            if (m_Active) {
                MissLight();
            }
        }

        private void ReachLight() {
            ReachedLight?.Invoke(this, EventArgs.Empty);

            m_SR.color = m_OffColor;

            m_Active = false;
        }

        private void MissLight() {
            m_SR.color = m_MissedColor;
            MissedLight?.Invoke(this, EventArgs.Empty);

            m_Active = false;

            // switch back to off color
            StartCoroutine(Wait(0.5f));
        }



        private IEnumerator Wait(float duration) {
            yield return new WaitForSeconds(duration);

            if (!m_Active) {
                m_SR.color = m_OffColor;
            }
        }
    }
}
