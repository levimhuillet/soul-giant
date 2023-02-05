using UnityEngine;
using BeauUtil;

namespace SoulGiant {
    
    public struct PhysicsContact {
        public RuntimeObjectHandle Kinematic;
        public KinematicState2D State;
        public RuntimeObjectHandle Collider;
        public Vector2 Point;
        public Vector2 Normal;

        public PhysicsContact(KinematicObject obj, KinematicState2D state, Collider2D collider, Vector2 point, Vector2 normal) {
            Kinematic = obj;
            State = state;
            Collider = collider;
            Point = point;
            Normal = normal;
        }

        static public implicit operator bool(PhysicsContact contact) {
            return contact.Collider.Id != 0;
        }
    }
}