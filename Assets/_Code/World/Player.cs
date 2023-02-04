using System;
using System.Collections;
using BeauRoutine;
using BeauUtil;
using BeauUtil.Debugger;
using UnityEngine;

namespace SoulGiant {
    public class Player : MonoBehaviour {

        #region Inspector

        [Header("Components")]
        [SerializeField, PrefabModeOnly] private SpriteRenderer m_SpriteRenderer = null;
        [SerializeField, PrefabModeOnly] private TrailRenderer m_Trail = null;
        [SerializeField, PrefabModeOnly] private KinematicObject m_Kinematics = null;
        [SerializeField, PrefabModeOnly] private Collider2D m_HurtCollider = null;
        [SerializeField, PrefabModeOnly] private Collider2D m_ScanCollider = null;

        [Header("Scanning")]
        [SerializeField] private LayerMask m_ScanMask = 0;
        [SerializeField] private Transform m_ScanEffectsRoot = null;
        [SerializeField] private Transform m_ScanRotator = null;
        [SerializeField] private ParticleSystem m_ScanParticles = null;
        [SerializeField] private ParticleSystemForceField m_ScanDisperser = null;

        [Header("Sprites")]
        [SerializeField] private Sprite m_NormalSprite = null;
        [SerializeField] private Sprite m_ScanSprite = null;
        [SerializeField] private Color m_NormalOutline = Color.white;
        [SerializeField] private Color m_ScanOutline = Color.white;

        [Header("Tuning")]
        [SerializeField] private float m_MaxSpeed = 2;
        [SerializeField] private float m_Acceleration = 2;
        [SerializeField] private float m_DragMoving = 1;
        [SerializeField] private float m_DragIdle = 2;

        #endregion // Inspector

        [NonSerialized] private bool m_Scanning;
        [NonSerialized] private Scannable m_ScanTarget;
        [NonSerialized] private int m_VisualState;
        private Routine m_ScanRoutine;

        private void Awake() {
            m_ScanRotator.gameObject.SetActive(false);
            m_ScanParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void FixedUpdate() {
            UpdateMovement();
        }

        private void LateUpdate() {
            UpdateScanning();
        }

        private void UpdateMovement() {
            Vector2 input = GetDesiredMovement();

            if (input.x != 0 || input.y != 0) {
                Vector2 currentVel = m_Kinematics.State.Velocity;
                Vector2 desiredVel = input * m_MaxSpeed;
                Vector2 diff = desiredVel - currentVel;

                m_Kinematics.Config.Drag = m_DragMoving;

                if (diff.sqrMagnitude >= 0.01f) {
                    Vector2 velChange = diff * (Time.fixedDeltaTime * m_Acceleration);
                    if (m_Scanning) {
                        velChange *= 0.5f;
                    }

                    m_Kinematics.State.Velocity += velChange;
                }
            } else {
                m_Kinematics.Config.Drag = m_DragIdle;
            }
        }

        #region Scanning

        private void UpdateScanning() {
            if (!m_Scanning) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    ContactFilter2D filter = default;
                    Scannable scan;
                    filter.useTriggers = true;
                    if (m_ScanCollider.IsOverlapping(m_ScanMask, filter, out Collider2D collider) && (scan = collider.GetComponent<Scannable>()) && IsScanActivated(scan)) {
                        StartScan(scan);
                    } else {
                        m_ScanRoutine.Replace(this, NoScanFlash());
                    }
                }
            }

            if (m_Scanning) {
                if (!m_ScanTarget || !IsScanActivated(m_ScanTarget) || !IsScanInRange(m_ScanTarget) || !Input.GetKey(KeyCode.Space)) {
                    CancelScan(true, true);
                } else {
                    m_ScanEffectsRoot.position = m_ScanTarget.transform.position;
                    m_ScanRotator.Rotate(0, 0, -80 * Time.deltaTime, Space.Self);
                    if (Time.frameCount % 8 == 0) {
                        int newState = (m_VisualState + 1) % 2;
                        if (newState == 0) {
                            SetVisuals(0, m_NormalSprite, m_NormalOutline);
                        } else {
                            SetVisuals(1, m_ScanSprite, m_ScanOutline);
                        }
                    }
                }
            }
        }

        private IEnumerator NoScanFlash() {
            int times = 2;
            while(times-- > 0) {
                SetVisuals(1, m_ScanSprite, m_ScanOutline);
                int frames = 8;
                while(frames-- > 0) {
                    yield return null;
                }
                SetVisuals(0, m_NormalSprite, m_NormalOutline);
                frames = 8;
                while(frames-- > 0) {
                    yield return null;
                }
            }
        }

        private IEnumerator ScanRoutine() {
            m_ScanTarget.ScanData = DB.Scans.GetScan(m_ScanTarget.ScanId);
            // TODO: Check scan
            float duration = m_ScanTarget.ScanData.Duration;
            if (m_ScanTarget.Scanned) {
                duration = 0.2f;
            }

            float inv = 1f / duration;

            float progress = 0;
            while(progress < 1) {
                progress += Routine.DeltaTime * inv;
                var emission = m_ScanParticles.emission;
                emission.rateOverTimeMultiplier = 10 + progress * 50;
                yield return null;
            }

            m_ScanParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            m_ScanDisperser.gameObject.SetActive(true);
            
            yield return null;

            m_ScanTarget.Scanned = true;
            m_ScanTarget.OnScanned.Invoke(m_ScanTarget);
            CancelScan(false, false);
        }

        private bool IsScanActivated(Scannable scannable) {
            return scannable.isActiveAndEnabled;
        }

        private bool IsScanInRange(Scannable scannable) {
            return m_ScanCollider.IsTouching(scannable.Collider);
        }

        private void StartScan(Scannable scannable) {
            CancelScan(false, true);
            m_Scanning = true;
            m_ScanTarget = scannable;
            Log.Msg("[Player] Starting scan");
            m_ScanRoutine.Replace(this, ScanRoutine());
            m_ScanRotator.gameObject.SetActive(true);

            m_ScanEffectsRoot.position = scannable.transform.position;
            var emissions = m_ScanParticles.emission;
            emissions.rateOverTimeMultiplier = m_ScanTarget.Scanned ? 60 : 10;
            m_ScanParticles.Play();

            m_ScanTarget.OnScanBegin.Invoke(m_ScanTarget);
        }

        private void CancelScan(bool flashError, bool invokeCancel) {
            if (m_Scanning) {
                m_ScanRotator.gameObject.SetActive(false);
                m_ScanParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);

                m_Scanning = false;
                m_ScanDisperser.gameObject.SetActive(false);
                
                if (m_ScanTarget) {
                    if (invokeCancel) {
                        m_ScanTarget.OnScanCancel.Invoke(m_ScanTarget);
                    }
                    m_ScanTarget = null;
                }

                if (flashError) {
                    m_ScanRoutine.Replace(this, NoScanFlash());
                } else {
                    m_ScanRoutine.Stop();
                    SetVisuals(0, m_NormalSprite, m_NormalOutline);
                }
            }
        }

        #endregion // Scanning

        private void SetVisuals(int state, Sprite sprite, Color outline) {
            m_SpriteRenderer.sprite = sprite;
            m_Trail.startColor = m_Trail.endColor = outline;
            m_VisualState = state;
        }

        static private Vector2 GetDesiredMovement() {
            Vector2 input = default;
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
                input.x -= 1;
            }
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
                input.x += 1;
            }
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
                input.y += 1;
            }
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
                input.y -= 1;
            }
            return input.normalized;
        }
    }
}