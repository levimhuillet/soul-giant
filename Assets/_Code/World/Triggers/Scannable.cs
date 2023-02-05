using System;
using System.Collections;
using BeauRoutine;
using BeauUtil;
using BeauUtil.Debugger;
using UnityEngine;

namespace SoulGiant {
    public class Scannable : Trigger {

        #region Inspector

        public SerializedHash32 ScanId;
        public Collider2D Collider;
        public Sprite AdditionalImage;

        #endregion // Inspector

        [NonSerialized] public ScanData ScanData;
        [NonSerialized] public bool Scanned;

        public CastableEvent<Scannable> OnScanBegin = new CastableEvent<Scannable>(1);
        public CastableEvent<Scannable> OnScanCancel = new CastableEvent<Scannable>(1);
        public CastableEvent<Scannable> OnScanned = new CastableEvent<Scannable>(1);
        public CastableEvent<Scannable> OnLoaded = new CastableEvent<Scannable>(1);
    }
}