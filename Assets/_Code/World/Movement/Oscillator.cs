using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace SoulGiant
{
    public class Oscillator : MonoBehaviour
    {
        [SerializeField] private Transform m_ToOscillate;
        [SerializeField] private Transform m_Transform1, m_Transform2;
        [SerializeField] private float m_MoveTime;
        [SerializeField] private float m_Delay;

        private Routine m_OscillateRoutine;

        private Vector3 m_Pos1, m_Pos2;
        private bool m_TerminateOscillate;

        private void Start() {
            m_TerminateOscillate = false;

            m_Pos1 = m_Transform1.position;
            m_Pos2 = m_Transform2.position;

            m_ToOscillate.transform.position = m_Pos1;

            m_OscillateRoutine.Replace(OscillateRoutine());
        }

        private void OnDisable() {
            m_OscillateRoutine.Stop();
        }

        private IEnumerator OscillateRoutine() {
            yield return m_Delay;
            while (m_ToOscillate.gameObject.activeInHierarchy) {
                Vector3 dest = m_Pos2;
                yield return m_ToOscillate.MoveTo(dest, m_MoveTime);

                if (!m_ToOscillate.gameObject.activeInHierarchy) { break; }
                dest = m_Pos1;
                yield return m_ToOscillate.MoveTo(dest, m_MoveTime);
            }
        }
    }
}
