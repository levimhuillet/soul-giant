using UnityEngine;
using BeauUtil;
using System;
using BeauUtil.Debugger;
using System.Collections.Generic;

namespace SoulGiant {
    
    [DefaultExecutionOrder(-1000)]
    public class KinematicsMgr : MonoBehaviour {

        public const float DefaultContactOffset = (1f / 128f);
        private const float OverlapThreshold = DefaultContactOffset;
        
        #region Inspector

        [NonSerialized] private RingBuffer<KinematicObject> m_Objects = new RingBuffer<KinematicObject>(16);

        #endregion // Inspector

        public bool Enabled = true;
        public LayerMask Mask = Bits.All32;

        private void Awake() {
            Time.fixedDeltaTime = 1f / 60;
            Physics2D.autoSimulation = false;
            Physics2D.autoSyncTransforms = false;
        }

        private void FixedUpdate() {
            if (!Enabled) {
                return;
            }

            float dt = Time.fixedDeltaTime;
            if (dt <= 0) {
                return;
            }

            BeginTick();
            Tick(m_Objects, Mask, dt);
            EndTick();
        }

        public void Register(KinematicObject obj) {
            m_Objects.PushBack(obj);
            obj.Body.simulated = true;
            obj.Body.isKinematic = true;
            obj.Body.simulated = Bits.Contains(Mask, obj.Body.gameObject.layer);
        }

        public void Deregister(KinematicObject obj) {
            m_Objects.FastRemove(obj);
            obj.Body.velocity = default;
            obj.Body.simulated = true;
        }

        #region Tick

        static private void BeginTick() {
            Physics2D.autoSyncTransforms = false;
            Physics2D.SyncTransforms();
        }

        static private void EndTick() {
            Physics2D.SyncTransforms();
            Physics2D.autoSyncTransforms = true;
        }

        static private unsafe void Tick(RingBuffer<KinematicObject> objects, LayerMask mask, float dt) {
            KinematicObject obj;
            int objCount = objects.Count;
            Vector2* offsets = stackalloc Vector2[objCount];
            Vector2* positions = stackalloc Vector2[objCount];
            KinematicConfig2D config;
            bool simulate;

            float invDT = 1f / dt;

            for(int i = 0; i < objCount; i++) {
                obj = objects[i];

                simulate = Bits.Contains(mask, obj.Body.gameObject.layer);
                if (simulate) {
                    obj.State.Velocity += obj.AccumulatedForce;
                    obj.AccumulatedForce = default;
                }

                obj.ContactBuffer = null;
                obj.ContactCount = 0;
                obj.Body.useFullKinematicContacts = simulate;
                obj.Body.simulated = simulate;

                if (simulate) {
                    config = obj.Config;
                    config.Drag += obj.AdditionalDrag;

                    offsets[i] = KinematicMath2D.Integrate(ref obj.State, ref config, dt);
                    positions[i] = obj.Body.position + offsets[i];

                    obj.Body.velocity = offsets[i] * invDT;
                    SyncPosition(positions[i], obj.Body, obj.Transform);
                }
            }

            bool bRun = Physics2D.Simulate(dt);
            Assert.True(bRun);

            ContactFilter2D filter = default;

            for(int i = 0; i < objCount; i++) {
                obj = objects[i];

                if (obj.SolidLayerMask == 0 || !obj.Body.simulated) {
                    continue;
                }

                ContactPoint2D contact;
                float sep;
                Vector2 sepVec;

                float accumulatedSep = 0;
                Vector2 accumulatedSepVec = default;

                filter.SetLayerMask(obj.SolidLayerMask);

                int contactCount = Physics2D.GetContacts(obj.Collider, filter, s_ContactPointBuffer);
                if (contactCount > 0) {
                    Array.Sort(s_ContactPointBuffer, 0, contactCount, ContactSorter.Instance);

                    for(int contactIdx = 0; contactIdx < contactCount; contactIdx++) {
                        contact = s_ContactPointBuffer[contactIdx];
                        if (contact.otherCollider != obj.Collider) {
                            continue;
                        }

                        sep = contact.separation + OverlapThreshold + Vector2.Dot(accumulatedSepVec, contact.normal) * accumulatedSep;

                        if (sep >= 0) {
                            continue;
                        }

                        sepVec = contact.normal;
                        sepVec.x *= -sep;
                        sepVec.y *= -sep;

                        accumulatedSepVec += sepVec;
                        accumulatedSep = sepVec.magnitude;

                        AdjustVelocity(ref obj.State.Velocity, contact.normal, obj.SolidBounce);
                    }

                    if (accumulatedSepVec.sqrMagnitude > 0) {
                        positions[i] += accumulatedSepVec;
                    }
                }
            }

            for(int i = 0; i < objCount; i++) {
                obj = objects[i];
                if (obj.Body.simulated) {
                    obj.Body.useFullKinematicContacts = false;
                    SyncPosition(positions[i], obj.Body, obj.Transform);
                }
            }
        }

        static private void SyncPosition(Vector2 pos, Rigidbody2D body, Transform transform) {
            body.position = pos;

            if (!transform.IsReferenceNull()) {
                Vector3 transformPos = transform.position;
                transformPos.x = pos.x;
                transformPos.y = pos.y;
                transform.position = transformPos;
            }
        }

        static private readonly ContactPoint2D[] s_ContactPointBuffer = new ContactPoint2D[16];

        private class ContactSorter : IComparer<ContactPoint2D>
        {
            static public readonly ContactSorter Instance = new ContactSorter();

            public int Compare(ContactPoint2D x, ContactPoint2D y)
            {
                return x.collider.GetInstanceID().CompareTo(y.collider.GetInstanceID());
            }
        }

        #endregion // Tick

        #region Math

        static private void AdjustVelocity(ref Vector2 vel, Vector2 normal, float bounce) {
            float val = (-1f - bounce) * Vector2.Dot(normal, vel);
            vel.x = val * normal.x + vel.x;
            vel.y = val * normal.y + vel.y;
        }

        #endregion // Math
    }
}