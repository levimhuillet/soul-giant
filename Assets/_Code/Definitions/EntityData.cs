using SoulGiant.Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoulGiant.Entity;

namespace SoulGiant {
    [CreateAssetMenu(fileName = "NewEntityData", menuName = "SoulGiant/EntityData")]
    public class EntityData : ScriptableObject
    {
        public SpriteAnimation[] SpriteAnimations;
        public bool LaunchesProjectile = true;
        public ProjectileData ProjectileData;
        public float ProximityDetectorRadius = 10;

        public float ReloadRate;
        public float LaunchSpeed;
        public float FireOffsetRange; // max aim angle deviation
    }
}