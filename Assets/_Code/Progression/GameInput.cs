using System;
using System.Collections;
using System.Collections.Generic;
using BeauRoutine;
using BeauUtil;
using BeauUtil.Debugger;
using UnityEngine;

namespace SoulGiant {
    public class GameInput {
        private bool m_WorldInputBlock = false;
        private bool m_AllInputBlock = false;

        private readonly HashSet<int> m_BlockingWorldObjects = new HashSet<int>();
        private readonly HashSet<int> m_BlockingAllObjects = new HashSet<int>();

        public void BlockWorld(Component component) {
            if (m_BlockingWorldObjects.Add(UnityHelper.Id(component))) {
                m_WorldInputBlock = true;
            }
        }

        public void UnblockWorld(Component component) {
            if (m_BlockingWorldObjects.Remove(UnityHelper.Id(component))) {
                m_WorldInputBlock = m_BlockingWorldObjects.Count == 0;
            }
        }

        public void BlockAll(Component component) {
            if (m_BlockingAllObjects.Add(UnityHelper.Id(component))) {
                m_AllInputBlock = true;
            }
        }

        public void UnblockAll(Component component) {
            if (m_BlockingAllObjects.Remove(UnityHelper.Id(component))) {
                m_AllInputBlock = m_BlockingAllObjects.Count == 0;
            }
        }

        public Vector2 MovementInput() {
            if (m_WorldInputBlock || m_AllInputBlock) {
                return default(Vector2);
            }

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

        public bool InteractPress() {
            return !m_WorldInputBlock && !m_AllInputBlock && Input.GetKeyDown(KeyCode.Space);
        }

        public bool InteractDown() {
            return !m_WorldInputBlock && !m_AllInputBlock && Input.GetKey(KeyCode.Space);
        }

        public bool AdvancePress() {
            return !m_AllInputBlock && Input.GetKeyDown(KeyCode.Space);
        }
    }
}