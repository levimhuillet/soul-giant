using UnityEngine;
using BeauUtil;
using BeauUtil.Extensions;

namespace SoulGiant {
    [DefaultExecutionOrder(-50000)]
    public class DB : Singleton<DB>  {
        [SerializeField] private ScanDataPackage m_ScanData = null;

        protected override void OnAssigned() {
            base.OnAssigned();

            m_ScanData.Parse(new ScanDataPackage.Generator());
        }

        static public ScanDataPackage Scans {
            get { return I.m_ScanData; }
        }
    }
}