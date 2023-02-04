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

        private float m_Speed;
        private Vector3 m_TravelDir;

        private enum State
        {
            Init,
            Launching,
            Traveling,
            Impacting
        }

        private State m_state;

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

        #region External

        public void Init(ProjectileData pData, float speed, Vector3 travelDir) {
            m_InitData = pData;
            // TODO: apply pData (sprite, etc.)

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
