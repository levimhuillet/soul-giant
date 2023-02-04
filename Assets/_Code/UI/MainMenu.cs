﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SoulGiant
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button m_NewGameButton;

        private void Awake() {
            m_NewGameButton.onClick.AddListener(HandleNewGameClicked);
        }

        #region Handlers

        private void HandleNewGameClicked() {
            // Start new game
            SceneManager.LoadScene("Gameplay");
        }

        #endregion // Handlers
    }
}
