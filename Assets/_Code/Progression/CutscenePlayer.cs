using System.Collections;
using BeauRoutine;
using UnityEngine;

namespace SoulGiant {
    public class CutscenePlayer : MonoBehaviour {
        public CutsceneData Data;
        public string NextScene;

        public void Start() {
            Routine.Start(this, Play());
        }

        private IEnumerator Play() {
            for(int i = 0; i < Data.Frames.Length; i++) {
                var frame = Data.Frames[i];
                Game.Image.Display(frame.Image);
                if (!string.IsNullOrEmpty(frame.Text)) {
                    yield return frame.Delay;
                    yield return Game.Textbox.Display(null, frame.Text, true);
                    yield return 1;
                } else {
                    yield return frame.Delay;
                    Game.Image.Hide();
                    yield return 0.5f;
                }
            }

            Game.LoadScene(NextScene);
        }
    }
}