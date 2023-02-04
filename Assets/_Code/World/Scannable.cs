using System;
using System.Collections;
using BeauRoutine;
using BeauUtil;
using BeauUtil.Debugger;
using UnityEngine;

namespace SoulGiant {
    public class Scannable : MonoBehaviour {

        #region Inspector

        public SerializedHash32 ScanId;
        public Collider2D Collider;

        #endregion // Inspector

        [NonSerialized] public ScanData ScanData;
        [NonSerialized] public bool Scanned;

        public CastableEvent<Scannable> OnScanBegin = new CastableEvent<Scannable>(1);
        public CastableEvent<Scannable> OnScanCancel = new CastableEvent<Scannable>(1);
        public CastableEvent<Scannable> OnScanned = new CastableEvent<Scannable>(1);
        public CastableEvent<Scannable> OnLoaded = new CastableEvent<Scannable>(1);
    }
}