using BeauPools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulGiant
{
    public class GameMechanics : MonoBehaviour
    {
        public static GameMechanics Instance;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
            }
        }

        [SerializeField] private ProjectilePools m_ProjectilePools;

        public static Projectile AllocProjectile(ProjectileData pData, float speed, Vector3 launchDir) {
            TempAlloc<Projectile> projectileAlloc = Instance.m_ProjectilePools.ProjectilePool.TempAlloc();
            Projectile projectile = projectileAlloc.Object;

            projectile.Init(projectileAlloc, pData, speed, launchDir);

            return projectile;
        }
    }
}
