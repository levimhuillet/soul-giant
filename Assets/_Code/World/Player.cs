using System;
using System.Collections;
using BeauRoutine;
using BeauUtil;
using BeauUtil.Debugger;
using UnityEngine;

namespace SoulGiant {
    public class Player : MonoBehaviour {

        static public readonly StringHash32 Event_Damaged = "player::damaged";
        static public readonly StringHash32 Event_Death = "player::death";
        static public readonly StringHash32 Event_Respawn = "player::respawn";

        public struct DamageParams {
            public Transform Source;
            public Vector2? Direction;
            public float Impulse;
        }

        private enum HealthState {
            Dead,
            Hurt,
            Full
        }

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

        [Header("Hurt")]
        [SerializeField] private ParticleSystem m_HurtParticles = null;
        [SerializeField] private float m_HurtDuration = 1;
        [SerializeField] private float m_RegenDuration = 5;
        [SerializeField] private ParticleSystem m_RegenParticles = null;

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
        [NonSerialized] private HealthState m_HurtState = HealthState.Full;
        [NonSerialized] private Vector3 m_LastCheckpoint;
        private Routine m_ScanRoutine;
        private Routine m_HurtFlash;
        private Routine m_DeathRoutine;
        [NonSerialized] private float m_HealthRegenTimer;
        [NonSerialized] private float m_InvulnerabilityTimer;

        private void Awake() {
            m_ScanRotator.gameObject.SetActive(false);
            m_ScanParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            s_Current = this;
        }

        private void Start() {
            m_LastCheckpoint = transform.position;
            m_InvulnerabilityTimer = 2;
        }

        private void OnDestroy() {
            s_Current = null;
        }

        private void FixedUpdate() {
            UpdateMovement();
        }

        private void LateUpdate() {
            UpdateScanning();

            if (m_HealthRegenTimer > 0) {
                m_HealthRegenTimer -= Routine.DeltaTime;
                if (m_HealthRegenTimer <= 0) {
                    RestoreHealth();
                }
            }

            if (m_InvulnerabilityTimer > 0) {
                m_InvulnerabilityTimer -= Routine.DeltaTime;

                if (m_InvulnerabilityTimer <= 0) {
                    m_HurtFlash.Stop();
                    SetVisible(true);
                }
            }
        }

        private void UpdateMovement() {
            Vector2 input;
            if (m_HurtState != HealthState.Dead) {
                input = Game.Input.MovementInput();
                if (m_HurtState == HealthState.Hurt && m_InvulnerabilityTimer > 0) {
                    input *= 0.5f;
                }
            } else {
                input = default(Vector2);
            }

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
            if (m_HurtState == HealthState.Dead) {
                return;
            }

            if (!m_Scanning) {
                if (Game.Input.InteractPress()) {
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
                if (!m_ScanTarget || !IsScanActivated(m_ScanTarget) || !IsScanInRange(m_ScanTarget) || !Game.Input.InteractDown()) {
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

            Game.Textbox.Display(m_ScanTarget.ScanData.Header, m_ScanTarget.ScanData.Description, m_ScanTarget.AdditionalImage);
            if (m_ScanTarget.AdditionalImage) {
                Game.Image.Display(m_ScanTarget.AdditionalImage);
            }

            m_ScanTarget.Scanned = true;
            m_ScanTarget.OnScanned.Invoke(m_ScanTarget);
            m_ScanTarget.Invoke();
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

        #region Damage

        public void Damage(DamageParams damage = default(DamageParams)) {
            if (Game.Input.WorldBlocked() || m_HurtState == HealthState.Dead) {
                return;
            }

            if (damage.Impulse > 0) {
                Vector2 impulse;
                if (damage.Direction.HasValue) {
                    impulse = damage.Direction.Value;
                } else if (damage.Source != null) {
                    impulse = damage.Source.position - m_Kinematics.Transform.position;
                } else {
                    impulse = default(Vector2);
                }

                if (impulse.sqrMagnitude > 0) {
                    impulse.Normalize();
                    impulse *= damage.Impulse;
                    m_Kinematics.AccumulatedForce += impulse;
                }
            }

            m_HealthRegenTimer = m_RegenDuration;

            if (m_InvulnerabilityTimer > 0) {
                // m_InvulnerabilityTimer *= 1.1f; // <- causing issues with OnTriggerStay2D damage
                return;
            }

            CancelScan(false, true);

            switch(m_HurtState) {
                case HealthState.Full: {
                    Hurt();
                    break;
                }

                case HealthState.Hurt: {
                    Die();
                    break;
                }
            }
        }

        public void RestoreHealth() {
            m_HealthRegenTimer = 0;
            m_HurtFlash.Stop();
            SetVisible(true);
            if (m_HurtState == HealthState.Dead) {
                m_HurtParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            } else {
                m_HurtParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                m_RegenParticles.Emit(1);
            }
            m_HurtState = HealthState.Full;
            m_DeathRoutine.Stop();
            m_InvulnerabilityTimer = 0;
        }

        private void Hurt() {
            m_HurtState = HealthState.Hurt;
            m_HealthRegenTimer = m_RegenDuration;
            m_InvulnerabilityTimer = m_HurtDuration;
            m_HurtFlash.Replace(this, HurtFlash());
            m_HurtParticles.Play();
            Game.Event.Dispatch(Event_Damaged);
        }

        public void Die() {
            if (m_HurtState != HealthState.Dead) {
                m_HurtParticles.Stop();
                m_HurtState = HealthState.Dead;
                m_HealthRegenTimer = 0;
                m_InvulnerabilityTimer = 0;
                Game.Event.Dispatch(Event_Death);
                m_HurtFlash.Replace(this, HurtFlash());
                m_DeathRoutine.Replace(this, DeathRespawn());
            }
        }

        private IEnumerator DeathRespawn() {
            yield return 1;
            Game.FadeTransition(Respawn);
        }

        private void Respawn() {
            RestoreHealth();
            transform.position = m_LastCheckpoint;
            m_Trail.Clear();
            CameraController.Current.SnapToTarget();
            Game.Event.Dispatch(Event_Respawn);
            m_InvulnerabilityTimer = 1;
            m_HurtFlash.Replace(this, HurtFlash());
        }

        private IEnumerator HurtFlash() {
            while(true) {
                SetVisible(false);
                int frames = 4;
                while(frames-- > 0) {
                    yield return null;
                }
                SetVisible(true);
                frames = 4;
                while(frames-- > 0) {
                    yield return null;
                }
            }
        }

        #endregion // Damage

        private void SetVisuals(int state, Sprite sprite, Color outline) {
            m_SpriteRenderer.sprite = sprite;
            m_Trail.startColor = m_Trail.endColor = outline;
            m_VisualState = state;
        }

        private void SetVisible(bool visible) {
            m_SpriteRenderer.enabled = visible;
            m_Trail.forceRenderingOff = !visible;
        }

        static private Player s_Current;

        static public Player Current {
            get { return s_Current; }
        }
    }
}