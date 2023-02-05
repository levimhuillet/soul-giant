using BeauUtil;
using SoulGiant.Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoulGiant.Entity;

namespace SoulGiant {
    [CreateAssetMenu(fileName = "NewEntityData", menuName = "SoulGiant/EntityData")]
    public class EntityData : ScriptableObject
    {
        public enum AimType {
            Direct,
            Fixed
        }

        public enum SpreadType {
            Spread,
            Cluster
        }

        [Header("Animations")]
        public SpriteAnimation[] SpriteAnimations;
        public float ProximityDetectorRadius = 10;

        [Header("Projectile")]
        public bool LaunchesProjectile = true;
        public ProjectileData ProjectileData;
        public AimType ProjectileAim;
        [ShowIfField("AimType")] public Vector2 FixedProjectileAnim;
        public Vector2 ProjectileOffset;

        [Header("Base Projectile")]
        public float ReloadRate;
        public float LaunchSpeed;
        public float LaunchSpeedRange; // max launch speed range
        public float FirePositionRange; // max position range
        public float FireOffsetRange; // max aim angle deviation
        public int ProjectileCount = 1;
        public SpreadType ProjectileSpreadType;
        public float ProjectileCone = 0;
    }
}