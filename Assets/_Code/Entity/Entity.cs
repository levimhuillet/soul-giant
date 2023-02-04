using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulGiant {

    [RequireComponent(typeof(Collider2D))] // for detecting proximity
    public class Entity : MonoBehaviour
    { 
        [SerializeField] private EntityData m_InitData;
        [SerializeField] private ProximityDetector m_ProximityDetector;

        #region Unity Callbacks

        private void OnEnable() {
            InitData();
        }

        private void OnDisable() {
            UnloadData();
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
            // TODO: apply data from scriptable object

            // apply sprite

            // apply proximity detector
            m_ProximityDetector.PlayerEntered += HandlePlayerEnterProximity;
            m_ProximityDetector.PlayerExited += HandlePlayerExitProximity;
        }

        private void UnloadData() {
            m_ProximityDetector.PlayerEntered -= HandlePlayerEnterProximity;
            m_ProximityDetector.PlayerExited -= HandlePlayerExitProximity;
        }

        #endregion // Helpers

        #region Handlers 

        private void HandlePlayerEnterProximity(object sender, EventArgs args) {
            Debug.Log("[Entity] Player entered " + this.gameObject.name + "'s proximity!");
        }

        private void HandlePlayerExitProximity(object sender, EventArgs args) {
            Debug.Log("[Entity] Player exited " + this.gameObject.name + "'s proximity!");
        }

        #endregion // Handlers
    }
}
