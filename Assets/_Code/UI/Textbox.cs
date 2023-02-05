using BeauUtil;
using BeauRoutine;
using BeauRoutine.Extensions;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections;

namespace SoulGiant
{
    public class Textbox : BasePanel {

        #region Inspector

        [Header("Textbox")]
        [SerializeField] private TMP_Text m_Title = null;
        [SerializeField] private TMP_Text m_Text = null;

        #endregion // Inspector

        private Routine m_DisplayRoutine;

        private void Populate(StringSlice title, StringSlice text) {
            if (title.IsEmpty) {
                m_Title.gameObject.SetActive(false);
            } else {
                m_Title.SetText(title);
                m_Title.gameObject.SetActive(true);
            }

            if (text.IsEmpty) {
                m_Text.gameObject.SetActive(false);
            } else {
                m_Text.SetText(text);
                m_Text.gameObject.SetActive(true);
            }
        }

        public IEnumerator Display(StringSlice title, StringSlice text, bool freezeInput = false) {
            Populate(title, text);
            if (freezeInput) {
                Game.Input.BlockWorld(this);
            }
            return m_DisplayRoutine.Replace(this, DisplayRoutine()).Wait();
        }

        public void Clear() {
            Hide();
        }

        private IEnumerator DisplayRoutine() {
            Show();
            yield return 0.2f;
            while(!Game.Input.AdvancePress()) {
                yield return null;
            }
            Hide();
        }

        #region Transitions

        protected override void InstantTransitionToShow() {
            CanvasGroup.alpha = 1;
            Root.anchoredPosition = default;
            Root.gameObject.SetActive(true);
        }

        protected override void InstantTransitionToHide() {
            Root.gameObject.SetActive(false);
            CanvasGroup.alpha = 0;
            Root.anchoredPosition = default;
        }

        protected override IEnumerator TransitionToShow() {
            Root.gameObject.SetActive(true);
            Root.anchoredPosition = new Vector2(0, 16);
            CanvasGroup.alpha = 0;
            yield return Routine.Combine(
                Root.AnchorPosTo(0, 0.2f, Axis.Y).Ease(Curve.BackOut),
                CanvasGroup.FadeTo(1, 0.2f)
            );
        }

        protected override IEnumerator TransitionToHide() {
            if (Root.gameObject.activeSelf) {
                yield return Routine.Combine(
                    Root.AnchorPosTo(16, 0.2f, Axis.Y).Ease(Curve.CubeOut),
                    CanvasGroup.FadeTo(0, 0.2f)
                );
                Root.gameObject.SetActive(false);
            }
        }

        protected override void OnHide(bool instant) {
            m_DisplayRoutine.Stop();
            Game.Input.UnblockWorld(this);
            Game.Image.Clear();
        }
    
        #endregion
    }
}