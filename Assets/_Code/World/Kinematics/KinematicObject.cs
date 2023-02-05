using UnityEngine;
using BeauUtil;
using System;

namespace SoulGiant {
    
    public class KinematicObject : MonoBehaviour {
        
        [Inline(InlineAttribute.DisplayType.HeaderLabel)]
        public KinematicState2D State;

        [Inline]
        public KinematicConfig2D Config;

        [Header("Components")]

        [Required(ComponentLookupDirection.Self)] public Rigidbody2D Body;
        [Required(ComponentLookupDirection.Children)] public Collider2D Collider;

        [Header("Solid")]

        public LayerMask SolidLayerMask;
        public float SolidBounce = 0;

        [NonSerialized] public Transform Transform;
        [NonSerialized] public unsafe PhysicsContact* ContactBuffer;
        [NonSerialized] public int ContactCount;

        [NonSerialized] public Vector2 AccumulatedForce;
        [NonSerialized] public float AdditionalDrag;

        private void OnEnable() {
            this.CacheComponent(ref Transform);
            Game.Kinematics.Register(this);
        }

        private void OnDisable() {
            Game.Kinematics?.Deregister(this);
        }
    }
}