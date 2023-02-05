using System;
using BeauUtil;
using UnityEngine;

namespace SoulGiant {
    [ExecuteAlways]
    [RequireComponent(typeof(EdgeCollider2D))]
    public class WallGen : MonoBehaviour {
        [SerializeField] private GameObject[] m_WallPrefabs;
        [SerializeField] private float m_Spacing = 4;
        [SerializeField] private Vector2 m_RandomPosition = new Vector2(0.1f, 0.1f);
        [SerializeField] private float m_RandomScale = 0;

        #if UNITY_EDITOR

        private void Generate() {
            int childCount = transform.childCount;
            while(childCount > 0) {
                DestroyImmediate(transform.GetChild(0).gameObject);
                childCount--;
            }

            var edgeCollider = GetComponent<EdgeCollider2D>();
            Vector2[] points = edgeCollider.points;
            if (points.Length <= 1) {
                return;
            }

            for(int i = 0; i < points.Length - 1; i++) {
                Vector2 a = points[i];
                Vector2 b = points[i + 1];

                int count = Mathf.CeilToInt(Vector2.Distance(b, a) / m_Spacing);
                if (count > 0) {
                    for(int j = 0; j <= count; j++) {
                        Vector2 pos = Vector2.Lerp(a, b, (float) j / count);
                        pos += RNG.Instance.NextVector2(-m_RandomPosition, m_RandomPosition);
                        float scale = 1 + RNG.Instance.NextFloat(-m_RandomScale, m_RandomScale);
                        GameObject wall = Instantiate(RNG.Instance.Choose(m_WallPrefabs), pos, Quaternion.Euler(0, 0, RNG.Instance.NextFloat(360)), transform);
                        wall.transform.localScale = new Vector3(scale, scale, 1);
                    }
                }
            }
        }

        [SerializeField, HideInInspector] private uint m_LastHash = 0;

        private void Awake() {
            if (Application.isPlaying) {
                Destroy(this);
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