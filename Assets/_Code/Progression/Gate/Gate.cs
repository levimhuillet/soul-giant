using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoulGiant
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Gate : MonoBehaviour
    {
        private enum SequenceType {
            Fixed,
            Random
        }

        [SerializeField] private string m_NextLevel;
        [SerializeField] private bool m_StartsUnlocked = true;

        [SerializeField] private Sprite m_UnlockedSprite, m_LockedSprite;
        private SpriteRenderer m_SR;

        [SerializeField] private SequenceType m_SequenceType;
        [SerializeField] private GateLight[] m_Lights;
        [SerializeField] private GateLight[] m_LightSequence;
        [SerializeField] private float m_FlashTime; // how long player has to reach light

        [SerializeField] private int m_NumLights; // for randomized sequence
        private int m_SequenceIndex;

        private bool m_Locked;
        private bool m_InSequence;

        private void Awake() {
            m_SR = this.GetComponent<SpriteRenderer>();

            m_Locked = true;
            m_SR.sprite = m_LockedSprite;
            m_SequenceIndex = 0;
            m_InSequence = false;

            if (m_StartsUnlocked) {
                Unlock();
            }

            InitLights();
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
                if (!m_Locked) {
                    SceneManager.LoadScene(m_NextLevel);
                }
                else {
                    // TEMP begin sequence (should be on scan)
                    if (!m_InSequence) {
                        PerformLightSequence();
                    }
                }
            }
        }

        #region Helpers

        private void Unlock() {
            m_Locked = false;

            // change sprite
            m_SR.sprite = m_UnlockedSprite;
        }

        private void InitLights() {
            for(int i = 0; i < m_Lights.Length; i++) {
                m_Lights[i].Init(this);

                m_Lights[i].ReachedLight += HandleLightReached;
                m_Lights[i].MissedLight += HandleLightMissed;
            }
        }

        private void PerformLightSequence() {
            m_SequenceIndex = 0;
            m_InSequence = true;

            TryNextLight();
        }

        private void TerminateSequence() {
            m_SequenceIndex = 0;
            m_InSequence = false;
        }

        private void TryNextLight() {
            if (m_SequenceType == SequenceType.Fixed) {
                // try trigger next in sequence
                if (m_SequenceIndex < m_LightSequence.Length) {
                    m_LightSequence[m_SequenceIndex].Activate(m_FlashTime);
                }
                else {
                    // unlock gate
                    Unlock();
                }
            }
            else if (m_SequenceType == SequenceType.Random) {
                // try trigger next in sequence
                if (m_SequenceIndex < m_NumLights) {
                    // TODO: generate random number
                    int randIndex = 0;

                    m_LightSequence[randIndex].Activate(m_FlashTime);
                }
                else {
                    // unlock gate

                    Unlock();
                }
            }
        }

        #endregion // Helpers

        #region Handlers

        private void HandleLightReached(object sender, EventArgs args) {
            m_SequenceIndex++;

            TryNextLight();
        }

        private void HandleLightMissed(object sender, EventArgs args) {
            TerminateSequence();
        }

        #endregion // Handlers
    }
}
