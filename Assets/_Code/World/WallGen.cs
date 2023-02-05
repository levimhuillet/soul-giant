using System;
using BeauUtil;
using UnityEngine;

namespace SoulGiant {
    [ExecuteAlways]
    [RequireComponent(typeof(EdgeCollider2D))]
    public class WallGen : MonoBehaviour {
        [SerializeField] private GameObject[] m_WallPrefabs;
        [SerializeField] private float m_Spacing = 4;
        [SerializeField] private Vector2 m_RandomDeviation = new Vector2(0.1f, 0.1f);

        #if UNITY_EDITOR

        private void Generate() {
            Transform[] children = new Transform[transform.childCount];
            for(int i = 0; i < children.Length; i++) {
                children[i] = transform.GetChild(i);
            }
            foreach(var child in children) {
                DestroyImmediate(child.gameObject);
            }

            var edgeCollider = GetComponent<EdgeCollider2D>();
            Vector2[] points = edgeCollider.points;
            for(int i = 0; i < points.Length - 1; i++) {
                Vector2 a = points[i];
                Vector2 b = points[i + 1];

                int count = Mathf.CeilToInt(Vector2.Distance(b, a) / m_Spacing);
                for(int j = 0; j <= count; j++) {
                    Vector2 pos = Vector2.Lerp(a, b, (float) j / count);
                    pos += RNG.Instance.NextVector2(-m_RandomDeviation, m_RandomDeviation);
                    Instantiate(RNG.Instance.Choose(m_WallPrefabs), pos, Quaternion.Euler(0, 0, RNG.Instance.NextFloat(360)), transform);
                }
            }
        }

        [SerializeField, HideInInspector] private uint m_LastHash = 0;

        private void Awake() {
            if (Application.isPlaying) {
                Destroy(gameObject);
            }
        }

        private void Update() {
            if (Application.isPlaying) {
                return;
            }

            var edgeCollider = GetComponent<EdgeCollider2D>();
            uint hash = edgeCollider.GetShapeHash();
            if (hash != m_LastHash || transform.childCount == 0) {
                m_LastHash = hash;
                Generate();
            }
        }

        #endif // UNITY_EDITOR
    }
}