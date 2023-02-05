using BeauUtil;
using UnityEngine;

namespace SoulGiant {
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class CameraBox : MonoBehaviour {
        [AutoEnum] public RectEdges Edges = RectEdges.All;
        public BoxCollider2D Collider;

        private void OnTriggerEnter2D(Collider2D collider) {
            if (collider.gameObject.layer != Layers.Player) {
                return;
            }

            CameraController.Current.AddBox(this);
        }

        private void OnTriggerExit2D(Collider2D collider) {
            if (collider.gameObject.layer != Layers.Player) {
                return;
            }

            CameraController.Current?.RemoveBox(this);
        }

        private void Reset() {
            Collider = GetComponent<BoxCollider2D>();
        }
    }
}