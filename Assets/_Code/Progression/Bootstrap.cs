using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoulGiant
{
    /// <summary>
    /// Placeholder Bootstrap, simply loads first scene
    /// </summary>
    public class Bootstrap : MonoBehaviour
    {
        private void Start() {
            SceneManager.LoadScene("MainMenu");
        }
    }
}