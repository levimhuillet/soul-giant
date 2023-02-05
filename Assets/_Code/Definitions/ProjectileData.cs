using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulGiant {
    [CreateAssetMenu (fileName = "NewProjectileData", menuName = "SoulGiant/ProjectileData")]
    public class ProjectileData : ScriptableObject
    {
        public Sprite BodySprite;
    }
}

