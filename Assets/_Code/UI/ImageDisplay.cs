using BeauUtil;
using BeauRoutine;
using BeauRoutine.Extensions;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections;

namespace SoulGiant
{
    public class ImageDisplay : BasePanel {

        #region Inspector

        [Header("Textbox")]
        [SerializeField] private Image m_Image = null;

        #endregion // Inspector

        private void Populate(Sprite image) {
            m_Image.sprite = image;
        }

        public void Display(Sprite image) {
            if (image) {
                Populate(image);
                Show();
            }
        }

        public void Clear() {
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

        protected override void OnShow(bool inbInstant) {
            Game.Input.BlockWorld(this);
        }

        protected override void OnHide(bool instant) {
            Game.Input.UnblockWorld(this);
        }

        protected override void OnHideComplete(bool instant) {
            m_Image.sprite = null;
        }
    
        #endregion
    }
}