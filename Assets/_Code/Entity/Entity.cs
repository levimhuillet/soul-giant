using BeauPools;
using BeauUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

namespace SoulGiant
{

    public class Entity : MonoBehaviour
    {
        [SerializeField] private EntityData m_InitData;
        [SerializeField] private SpriteRenderer m_BodySpriteSR;
        [SerializeField] private ProximityDetector m_ProximityDetector;

        private bool m_TrackingPlayer;
        private GameObject m_TargetObj;

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

            m_FiringState = FiringState.Armed;
            m_TrackingPlayer = false;
            m_TargetObj = null;
        }

        private void OnDisable() {
            UnloadData();
        }

        private void Update() {
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

        private void OnTriggerEnter2D(Collider2D collision) {
            // TODO: check if collision is player
        }

        private void OnTriggerExit2D(Collider2D collision) {
            // TODO: check if collision is player
        }

        #endregion // Unity Callbacks

        #region Helpers 

        private void InitData() {
            // apply data from scriptable object

            // apply sprite
            m_BodySpriteSR.sprite = m_InitData.BodySprite;

            // apply proximity detector
            m_ProximityDetector.Collider.radius = m_InitData.ProximityDetectorRadius;

            m_ProximityDetector.PlayerEntered += HandlePlayerEnterProximity;
            m_ProximityDetector.PlayerExited += HandlePlayerExitProximity;
        }

        private void UnloadData() {
            m_ProximityDetector.PlayerEntered -= HandlePlayerEnterProximity;
            m_ProximityDetector.PlayerExited -= HandlePlayerExitProximity;
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
                Debug.Log("[Entity] Launching projectile!");

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
