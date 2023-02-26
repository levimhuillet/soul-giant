using BeauPools;
using BeauUtil;
using SoulGiant.Animation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoulGiant
{

    public class Entity : MonoBehaviour
    {
        [SerializeField] private EntityData m_InitData;
        [SerializeField] private SpriteAnimator m_Animator;
        [SerializeField] private ProximityDetector m_ProximityDetector;

        private bool m_TrackingPlayer;
        private GameObject m_TargetObj;

        private bool m_LaunchesProjectile;

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
            // check if collision is player
            if (collision.gameObject.layer == Layers.Player) {
                Player player = Player.Current;
                player.Damage(new Player.DamageParams() {
                    Source = transform,
                    Impulse = 0
                });
            }
        }

        private void OnTriggerStay2D(Collider2D collision) {
            // check if collision is player
            if (collision.gameObject.layer == Layers.Player) {
                Player player = Player.Current;
                player.Damage(new Player.DamageParams() {
                    Source = transform,
                    Impulse = 0
                });
            }
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

                Vector2 startPos = (Vector2) this.transform.position + m_InitData.ProjectileOffset;
                float launchDirDeg;
                if (m_InitData.ProjectileAim == EntityData.AimType.Direct) {
                    launchDirDeg = Vector2.SignedAngle(new Vector2(1, 0), ((Vector2)m_TargetObj.transform.position - startPos).normalized);
                } else {
                    launchDirDeg = Vector2.SignedAngle(new Vector2(1, 0), m_InitData.FixedProjectileAnim);
                }

                Debug.Log("[Launch] launchDirDeg: " + launchDirDeg);

                float spreadStart = launchDirDeg - m_InitData.ProjectileCone / 2;
                float deviationPerBullet = 0;
                if (m_InitData.ProjectileCount > 1) {
                    deviationPerBullet = m_InitData.ProjectileCone;
                    if (m_InitData.ProjectileCone >= 360) {
                        deviationPerBullet /= m_InitData.ProjectileCount;
                    } else {
                        deviationPerBullet /= m_InitData.ProjectileCount - 1;
                    }
                }

                for(int i = 0; i < m_InitData.ProjectileCount; i++) {
                    Vector2 projectilePos = startPos + RNG.Instance.NextVector2(0, m_InitData.FirePositionRange);
                    float direction;
                    if (m_InitData.ProjectileSpreadType == EntityData.SpreadType.Cluster) {
                        direction = launchDirDeg + RNG.Instance.NextFloat(-m_InitData.ProjectileCone, m_InitData.ProjectileCone) / 2;
                    } else {
                        direction = spreadStart + deviationPerBullet * i;
                    }

                    direction += RNG.Instance.NextFloat(-m_InitData.FireOffsetRange, m_InitData.FireOffsetRange);

                    float launchSpeed = m_InitData.LaunchSpeed + RNG.Instance.NextFloat(m_InitData.LaunchSpeedRange);

                    Debug.Log("[Launch] direction: " + direction);
                    Debug.Log("[Launch] directionRadians: " + direction * Mathf.Deg2Rad);
                    Debug.Log("[Launch] directionNormalized: " + Geom.Normalized(direction * Mathf.Deg2Rad));

                    Projectile projectile = GameMechanics.AllocProjectile(m_InitData.ProjectileData, launchSpeed, Geom.Normalized(direction * Mathf.Deg2Rad));
                    projectile.transform.parent = this.transform;
                    projectile.transform.position = startPos;
                    projectile.Launch();
                }


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
