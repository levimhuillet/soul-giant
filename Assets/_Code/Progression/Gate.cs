using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoulGiant
{
    public class Gate : MonoBehaviour
    {
        [SerializeField] private string m_NextLevel;

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
                SceneManager.LoadScene(m_NextLevel);
            }
        }
    }
}
