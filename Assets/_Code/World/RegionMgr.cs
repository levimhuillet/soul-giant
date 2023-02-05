using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulGiant {
    public class RegionMgr : MonoBehaviour
    {
        [SerializeField] private RegionData m_RegionData;

        private void Start() {
            ApplyData();
        }

        #region Helpers 

        private void ApplyData() {
            AudioMgr.Instance.PlayAudio(m_RegionData.MusicID, true);
        }

        #endregion // Helpers
    }
}
