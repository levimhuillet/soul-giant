using BeauUtil;
using BeauRoutine;
using BeauRoutine.Extensions;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections;
using System;

namespace SoulGiant
{
    public class Fader : MonoBehaviour {

        #region Inspector

        [SerializeField] private Canvas m_Canvas = null;
        [SerializeField] private CanvasGroup m_Group = null;

        #endregion // Inspector

        private Routine m_Anim;

        private void Awake() {
            m_Canvas.enabled = false;
            m_Group.alpha = 0;
        }

        public IEnumerator Play(Action action) {
            m_Anim.Replace(this, PlayRoutine(action)).TryManuallyUpdate(0);
            return m_Anim.Wait();
        }

        public IEnumerator InstantFadeIn() {
            m_Anim.Replace(this, FadeInRoutine()).TryManuallyUpdate(0);
            return m_Anim.Wait();
        }

        private IEnumerator PlayRoutine(Action onMiddle) {
            if (!m_Canvas.enabled) {
                m_Canvas.enabled = true;
                m_Group.alpha = 0;
            }

            Game.Input.BlockAll(this);
            yield return m_Group.FadeTo(1, (1 - m_Group.alpha) * 0.5f);
            yield return 0.1f;
            onMiddle?.Invoke();
            Game.Input.UnblockAll(this);
            yield return m_Group.FadeTo(0, 0.5f);
            m_Canvas.enabled = false;
        }

        private IEnumerator FadeInRoutine() {
            m_Canvas.enabled = true;
            m_Group.alpha = 1;
            Game.Input.UnblockAll(this);
            yield return m_Group.FadeTo(0, 0.5f);
            m_Canvas.enabled = false;
        }
    }
}