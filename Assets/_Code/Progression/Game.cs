using UnityEngine;
using BeauUtil;
using BeauUtil.Extensions;

namespace SoulGiant {
    [DefaultExecutionOrder(-10000)]
    public class Game : Singleton<Game>  {
        [SerializeField] private KinematicsMgr m_KinematicsMgr = null;

        private readonly EventDispatcher<object> m_EventDispatcher = new EventDispatcher<object>();

        static public EventDispatcher<object> Event {
            get { return I.m_EventDispatcher; }
        }

        static public KinematicsMgr Kinematics {
            get { return I?.m_KinematicsMgr; }
        }
    }
}