using BeauUtil;
using UnityEngine;

namespace SoulGiant {
    public class Player : MonoBehaviour {

        [SerializeField] private KinematicObject m_Kinematics = null;
        [SerializeField] private float m_MaxSpeed = 2;
        [SerializeField] private float m_Acceleration = 2;

        private void FixedUpdate() {
            UpdateMovement();
        }

        private void UpdateMovement() {
            Vector2 input = GetDesiredMovement();

            Vector2 currentVel = m_Kinematics.State.Velocity;
            Vector2 desiredVel = input * m_MaxSpeed;
            Vector2 diff = desiredVel - currentVel;

            if (diff.sqrMagnitude >= 0.01f) {
                Vector2 velChange = diff * (Time.fixedDeltaTime * m_Acceleration);

                m_Kinematics.State.Velocity += velChange;
            }
        }

        static private Vector2 GetDesiredMovement() {
            Vector2 input = default;
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
                input.x -= 1;
            }
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
                input.x += 1;
            }
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
                input.y += 1;
            }
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
                input.y -= 1;
            }
            return input.normalized;
        }
    }
}