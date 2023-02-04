using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulGiant
{
    [CreateAssetMenu(fileName = "NewRegionData", menuName = "SoulGiant/RegionData")]
    public class RegionData : ScriptableObject
    {
        public AudioClip MusicTrack;
    }
}
