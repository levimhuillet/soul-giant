using UnityEngine;
using BeauUtil;
using BeauUtil.Extensions;
using BeauRoutine;
using UnityEngine.SceneManagement;
using System;

namespace SoulGiant {
    [DefaultExecutionOrder(-10000)]
    public class Game : Singleton<Game>  {
        #region Inspector

        [SerializeField] private KinematicsMgr m_KinematicsMgr = null;
        [SerializeField] private Textbox m_Textbox = null;
        [SerializeField] private ImageDisplay m_ImageDisplay = null;
        [SerializeField] private Fader m_Fader = null;

        #endregion // Inspector

        private readonly EventDispatcher<object> m_EventDispatcher = new EventDispatcher<object>();
        private readonly GameInput m_Input = new GameInput();

        private Routine m_LoadRoutine;

        protected override void OnAssigned() {
            Application.targetFrameRate = 60;
        }

        private void Start() {
            m_Fader.InstantFadeIn();
        }

        private void LateUpdate() {
            m_EventDispatcher.FlushQueue();
        }

        static public void LoadScene(string sceneName) {
            if (I.m_LoadRoutine) {
                return;
            }

            Game.Kinematics.Enabled = false;
            I.m_LoadRoutine.Replace(I, I.m_Fader.Play(() => {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
                Game.Kinematics.Enabled = true;
            }));
        }

        static public void FadeTransition(Action action) {
            I.m_Fader.Play(action);
        }

        static public EventDispatcher<object> Event {
            get { return I.m_EventDispatcher; }
        }

        static public GameInput Input {
            get { return I?.m_Input; }
        }

        static public KinematicsMgr Kinematics {
            get { return I?.m_KinematicsMgr; }
        }

        static public Textbox Textbox {
            get { return I?.m_Textbox; }
        }

        static public ImageDisplay Image {
            get { return I?.m_ImageDisplay; }
        }
    }
}