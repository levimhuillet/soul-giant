using SoulGiant.Regions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulGiant {
    public class StateMgr : MonoBehaviour
    {
        private Dictionary<RegionId, bool> m_CompletedMap;

        private void OnEnable() {
            m_CompletedMap = new Dictionary<RegionId, bool>();
            m_CompletedMap.Add(RegionId.Leg, false);
            m_CompletedMap.Add(RegionId.Leg_1p5, false);
            m_CompletedMap.Add(RegionId.Hub, false);
            m_CompletedMap.Add(RegionId.Stomach, false);
            m_CompletedMap.Add(RegionId.Lungs, false);
            m_CompletedMap.Add(RegionId.Heart, false);
            m_CompletedMap.Add(RegionId.Brain, false);
        }

        public void CompleteRegion(RegionId id) {
            if (!m_CompletedMap.ContainsKey(id)) {
                Debug.Log("[StateMgr] region " + id + " not initialized in the dictionary!");
                return;
            }
            m_CompletedMap[id] = true;
        }

        public bool IsRegionComplete(RegionId id) {
            return m_CompletedMap[id];
        }
    }
}
