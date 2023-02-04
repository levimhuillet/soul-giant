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
    }
}