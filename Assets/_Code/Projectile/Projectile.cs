using BeauPools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulGiant {
    public class Projectile : MonoBehaviour
    {
        [Serializable] public sealed class Pool : SerializablePool<Projectile> { }

        [SerializeField] private ProjectileData m_InitData;

        private TempAlloc<Projectile> m_TempAlloc;

        private float m_Speed;
        private Vector3 m_TravelDir;

        private SpriteRenderer m_SR;

        private enum State
        {
            Init,
            Launching,
            Traveling,
            Impacting
        }

        private State m_state;

        #region Unity Callbacks

        private void Update() {
            switch (m_state) {
                case State.Init:
                    break;
                case State.Launching:
                    break;
                case State.Traveling:
                    this.transform.Translate(m_TravelDir * m_Speed * Time.deltaTime, Space.World);
                    break;
                case State.Impacting:
                    break;
                default:
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
                // Apply damage (dispatch event)

                // Dispose projectile
                m_TempAlloc.Dispose();
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Solid")) {
                // Dispose projectile
                m_TempAlloc.Dispose();
            }
        }

        #endregion // Unity Callbacks

        #region External

        public void Init(TempAlloc<Projectile> alloc, ProjectileData pData, float speed, Vector3 travelDir) {
            m_SR = this.GetComponent<SpriteRenderer>();

            m_TempAlloc = alloc;
            m_InitData = pData;
            if (m_SR != null) {
                m_SR.sprite = pData.BodySprite;
            }

            m_state = State.Init;
            m_Speed = speed;
            m_TravelDir = travelDir;
        }

        public void Launch() {
            m_state = State.Launching;

            // any launch things here

            m_state = State.Traveling;
        }

        #endregion // External
    }
}
