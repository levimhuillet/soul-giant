using SoulGiant.Regions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulGiant
{
    public class Wire : MonoBehaviour
    {
        [SerializeField] private RegionId[] m_RequiredRegions;
        [SerializeField] private SpriteRenderer m_SR;
        [SerializeField] private Color m_ActiveColor, m_InactiveColor;

        private void Start() {
            bool requirementsMet = true;

            for (int i = 0; i < m_RequiredRegions.Length; i++) {
                if (!Game.State.IsRegionComplete(m_RequiredRegions[i])) {
                    requirementsMet = false;
                    break;
                }
            }

            if (requirementsMet) {
                ActivateWire();
            }
            else {
                DeactivateWire();
            }
        }

        private void ActivateWire() {
            m_SR.color = m_ActiveColor;
        }

        private void DeactivateWire() {
            m_SR.color = m_InactiveColor;
        }
    }
}
