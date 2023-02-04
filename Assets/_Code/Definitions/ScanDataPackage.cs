using BeauUtil.Blocks;
using BeauUtil;
using System.Collections.Generic;
using BeauUtil.Tags;

namespace SoulGiant {
    public class ScanDataPackage : ScriptableDataBlockPackage<ScanData> { 
        private readonly Dictionary<StringHash32, ScanData> m_Scans = new Dictionary<StringHash32, ScanData>();

        public override int Count { 
            get { return m_Scans.Count; }
        }

        public override IEnumerator<ScanData> GetEnumerator(){
            return m_Scans.Values.GetEnumerator();
        }

        public override void Clear() {
            base.Clear();

            m_Scans.Clear();
        }

        public ScanData GetScan(StringHash32 id) {
            m_Scans.TryGetValue(id, out ScanData data);
            return data;
        }

        public class Generator : GeneratorBase<ScanDataPackage> {
            public override bool TryCreateBlock(IBlockParserUtil inUtil, ScanDataPackage inPackage, TagData inId, out ScanData outBlock) {
                ScanData data = new ScanData(inId.Id);
                inPackage.m_Scans.Add(data.Id, data);
                outBlock = data;
                return true;
            }
        }

        #if UNITY_EDITOR

        [ScriptedExtension(0, "scan")]
        protected class Importer : ImporterBase<ScanDataPackage> { }

        #endif // UNITY_EDITOR
    }

    public class ScanData : IDataBlock {
        public readonly StringHash32 Id;

        [BlockMeta("header")] public string Header;
        [BlockMeta("duration")] public float Duration = 2;
        [BlockContent] public string Description;

        public ScanData(StringHash32 id) {
            Id = id;
        }
    }
}