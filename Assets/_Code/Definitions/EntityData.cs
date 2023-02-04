using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoulGiant.Entity;

namespace SoulGiant {
    [CreateAssetMenu(fileName = "NewEntityData", menuName = "SoulGiant/EntityData")]
    public class EntityData : ScriptableObject
    {
        public Sprite BodySprite;
        public ProjectileData ProjectileData;
        public float ProximityDetectorRadius = 2;

        public float ReloadRate;
        public float LaunchSpeed;
        public float FireOffsetRange; // max aim angle deviation
    }
}