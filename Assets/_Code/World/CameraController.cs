using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulGiant
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform m_Target;
        [SerializeField] private Camera m_Cam;

        private void Awake() {
            m_Cam = this.GetComponent<Camera>();
            m_Cam.transparencySortMode = TransparencySortMode.Orthographic;
        }

        private void FixedUpdate() {
            this.transform.position = new Vector3(
                m_Target.position.x,
                m_Target.position.y,
                this.transform.position.z
                );
        }
    }
}