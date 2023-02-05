using System.Collections;
using System.Collections.Generic;
using BeauRoutine;
using BeauUtil;
using UnityEngine;

namespace SoulGiant
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        static private CameraController s_Instance;

        [SerializeField] private Transform m_Target;
        [SerializeField] private Camera m_Cam;
        [SerializeField] private CameraFOVPlane m_Plane;

        private RingBuffer<CameraBox> m_CurrentBounds = new RingBuffer<CameraBox>(4);

        private void Awake() {
            s_Instance = this;
            m_Cam = this.GetComponent<Camera>();
            m_Cam.transparencySortMode = TransparencySortMode.Orthographic;
            m_Plane = GetComponent<CameraFOVPlane>();
        }

        private void Start() {
            SnapToTarget();
        }

        private void OnDestroy() {
            s_Instance = null;
        }

        private void LateUpdate() {
            Vector2 targetPos = m_Target.position;
            Vector2 currentPos = transform.position;

            currentPos = Vector2.Lerp(currentPos, targetPos, TweenUtil.Lerp(4));
            currentPos = ConstrainToBounds(currentPos);

            this.transform.SetPosition(currentPos, Axis.XY, Space.World);
        }

        private Vector2 ConstrainToBounds(Vector2 pos) {
            Rect rect = default;
            rect.height = m_Plane.ZoomedHeight();
            rect.width = rect.height * m_Cam.aspect;
            rect.center = pos;

            for(int i = 0; i < m_CurrentBounds.Count; i++) {
                CameraBox box = m_CurrentBounds[i];
                Rect bounds = default(Rect);
                bounds.size = box.Collider.size;
                bounds.center = (Vector2) box.transform.position + box.Collider.offset;
                Geom.Constrain(ref rect, bounds, box.Edges);
            }

            return rect.center;
        }

        public void SnapToTarget() {
            Vector2 pos = ConstrainToBounds(m_Target.position);
            this.transform.SetPosition(pos, Axis.XY, Space.World);
        }

        public void AddBox(CameraBox box) {
            m_CurrentBounds.PushBack(box);
        }

        public void RemoveBox(CameraBox box) {
            m_CurrentBounds.FastRemove(box);
        }

        static public CameraController Current {
            get { return s_Instance; }
        }
    }
}