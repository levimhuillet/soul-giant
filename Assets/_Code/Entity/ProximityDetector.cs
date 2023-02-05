using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulGiant
{
    public class ProximityEventArgs : EventArgs
    {
        public GameObject Target;

        public ProximityEventArgs(GameObject target) {
            Target = target;
        }
    }

    public class ProximityDetector : MonoBehaviour
    {
        public CircleCollider2D Collider;

        public event EventHandler<ProximityEventArgs> PlayerEntered;

        public event EventHandler<ProximityEventArgs> PlayerExited;

        #region Unity Callbacks

        private void OnTriggerEnter2D(Collider2D collision) {
            // check if collision is player
            if (collision.gameObject.layer == Layers.Player) {
                PlayerEntered?.Invoke(this, new ProximityEventArgs(collision.gameObject));
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            // check if collision is player
            if (collision.gameObject.layer == Layers.Player) {
                PlayerExited?.Invoke(this, new ProximityEventArgs(collision.gameObject));
            }
        }

        #endregion // Unity Callbacks
    }
}
