using BeauPools;
using BeauUtil;
using SoulGiant.Animation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.WSA;

namespace SoulGiant
{

    public class Entity : MonoBehaviour
    {
        [SerializeField] private EntityData m_InitData;
        [SerializeField] private SpriteAnimator m_Animator;
        [SerializeField] private ProximityDetector m_ProximityDetector;

        private bool m_TrackingPlayer;
        private GameObject m_TargetObj;

        [SerializeField] private bool m_LaunchesProjectile;

        private enum FiringState
        {
            Armed,
            Reloading
        }

        private FiringState m_FiringState;
        private float m_ReloadTimer;

        #region Unity Callbacks

        private void OnEnable() {
            InitData();

            if (m_LaunchesProjectile) {
                m_FiringState = FiringState.Armed;
                m_TrackingPlayer = false;
                m_TargetObj = null;
            }
        }

        private void OnDisable() {
            UnloadData();
        }

        private void Update() {
            if (m_LaunchesProjectile) {
                switch (m_FiringState) {
                    case FiringState.Reloading:
                        Reload();
                        break;
                    case FiringState.Armed:
                        if (m_TrackingPlayer) {
                            LaunchProjectile();
                        }
                        break;
                    default:
                        break;

                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            // TODO: check if collision is player
            if (collision.gameObject.layer == Layers.Player) {
                Player player = Player.Current;
                player.Damage(new Player.DamageParams() {
                    Source = transform,
                    Impulse = 0
                });
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            // TODO: check if collision is player
        }

        #endregion // Unity Callbacks

        #region Helpers 

        private void InitData() {
            // apply data from scriptable object
            m_LaunchesProjectile = m_InitData.LaunchesProjectile;

            // apply sprite animation
            if (m_InitData.SpriteAnimations.Length > 0) {
                m_Animator.Animation = m_InitData.SpriteAnimations[0];
                m_Animator.Play(m_InitData.SpriteAnimations[0], true);
            }

            // apply proximity detector
            if (m_ProximityDetector != null) {
                m_ProximityDetector.Collider.radius = m_InitData.ProximityDetectorRadius;

                m_ProximityDetector.PlayerEntered += HandlePlayerEnterProximity;
                m_ProximityDetector.PlayerExited += HandlePlayerExitProximity;
            }
        }

        private void UnloadData() {
            if (m_ProximityDetector != null) {
                m_ProximityDetector.PlayerEntered -= HandlePlayerEnterProximity;
                m_ProximityDetector.PlayerExited -= HandlePlayerExitProximity;
            }
        }

        private void Reload() {
            m_ReloadTimer -= Time.deltaTime;

            if (m_ReloadTimer <= 0) {
                m_FiringState = FiringState.Armed;
            }
        }

        private void LaunchProjectile() {
            if (m_InitData.ReloadRate != 0) {
                // Fire projectile w/ aim offset

                // (TODO: launch from launching pos)
                Vector2 projectileStartPos = this.transform.position;

                // calculate direction vector
                Vector2 launchDir = ((Vector2)m_TargetObj.transform.position - projectileStartPos).normalized;

                Projectile projectile = GameMechanics.AllocProjectile(m_InitData.ProjectileData, m_InitData.LaunchSpeed, launchDir);
                projectile.transform.parent = this.transform;
                projectile.transform.position = projectileStartPos;
                projectile.Launch();

                // Reload
                m_FiringState = FiringState.Reloading;
                m_ReloadTimer = m_InitData.ReloadRate;
            }
        }

        #endregion // Helpers

        #region Handlers 

        private void HandlePlayerEnterProximity(object sender, ProximityEventArgs args) {
            m_TrackingPlayer = true;
            m_TargetObj = args.Target;
        }

        private void HandlePlayerExitProximity(object sender, ProximityEventArgs args) {
            m_TrackingPlayer = false;
            m_TargetObj = null;
        }

        #endregion // Handlers
    }
}
