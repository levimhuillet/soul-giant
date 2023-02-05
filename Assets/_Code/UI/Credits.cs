using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulGiant {
    public class Credits : MonoBehaviour
    {
        [SerializeField] private Button m_QuitButton;

        private void Awake() {
            m_QuitButton.onClick.AddListener(HandleQuit);
        }

        private void HandleQuit() {
            Application.Quit();
        }
    }

}