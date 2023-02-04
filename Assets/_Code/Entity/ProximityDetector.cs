using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulGiant
{
    public class ProximityDetector : MonoBehaviour
    {
        [SerializeField] private Collider2D Collider;

        public event EventHandler PlayerEntered;

        public event EventHandler PlayerExited;

        #region Unity Callbacks

        private void OnTriggerEnter2D(Collider2D collision) {
            // check if collision is player
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
                PlayerEntered?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            // check if collision is player
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
                PlayerExited?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion // Unity Callbacks
    }
}
