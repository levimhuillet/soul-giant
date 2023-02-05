using BeauPools;
using BeauRoutine;
using BeauUtil;
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

        private Vector3 m_Velocity;
        private float m_Decay;

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
                    this.transform.Translate(m_Velocity * Time.deltaTime, Space.World);
                    if (m_InitData.Gravity != 0) {
                        m_Velocity.y -= Time.deltaTime * m_InitData.Gravity;
                    }
                    if (m_Decay > 0) {
                        m_Decay -= Time.deltaTime;
                        if (m_Decay <= 0) {
                            m_TempAlloc.Dispose();
                        }
                    }
                    break;
                case State.Impacting:
                    break;
                default:
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.layer == Layers.Player) {
                Player player = Player.Current;
                player.Damage(new Player.DamageParams() {
                    Source = transform,
                    Impulse = m_InitData.PlayerImpulse
                });
                // Apply damage (dispatch event)

                // Dispose projectile
                m_TempAlloc.Dispose();
            }
            else if (collision.gameObject.layer == Layers.Solid) {
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
            m_Decay = pData.Duration;

            if (pData.RotateToDirection) {
                transform.SetRotation(Vector2.Angle(Vector2.right, travelDir), Axis.Z, Space.Self);
            } else {
                transform.SetRotation(0, Axis.Z, Space.Self);
            }

            m_state = State.Init;
            m_Velocity = travelDir * speed;
        }

        public void Launch() {
            m_state = State.Launching;

            // any launch things here

            m_state = State.Traveling;
        }

        #endregion // External
    }
}
