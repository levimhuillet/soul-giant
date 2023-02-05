using UnityEngine;
using BeauUtil;
using BeauUtil.Extensions;

namespace SoulGiant {
    [DefaultExecutionOrder(-10000)]
    public class Game : Singleton<Game>  {
        #region Inspector

        [SerializeField] private KinematicsMgr m_KinematicsMgr = null;
        [SerializeField] private Textbox m_Textbox = null;

        #endregion // Inspector

        private readonly EventDispatcher<object> m_EventDispatcher = new EventDispatcher<object>();
        private readonly GameInput m_Input = new GameInput();

        protected override void OnAssigned() {
            Application.targetFrameRate = 60;
        }

        private void LateUpdate() {
            m_EventDispatcher.FlushQueue();
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
    }
}