using UnityEngine;
using BeauUtil;
using BeauUtil.Extensions;
using System.Collections.Generic;

namespace SoulGiant {
    [DefaultExecutionOrder(-50000)]
    public class DB : Singleton<DB>  {
        [SerializeField] private ScanDataPackage m_ScanData = null;

        private Dictionary<string, AudioData> m_audioMap;

        [SerializeField] private AudioData[] m_audioData;

        protected override void OnAssigned() {
            base.OnAssigned();

            m_ScanData.Parse(new ScanDataPackage.Generator());
        }

        static public ScanDataPackage Scans {
            get { return I.m_ScanData; }
        }

        static public AudioData GetAudioData(string id) {
            // initialize the map if it does not exist
            if (I.m_audioMap == null) {
                I.m_audioMap = new Dictionary<string, AudioData>();
                foreach (AudioData data in I.m_audioData) {
                    I.m_audioMap.Add(data.ID, data);
                }
            }
            if (I.m_audioMap.ContainsKey(id)) {
                return I.m_audioMap[id];
            }
            else {
                throw new KeyNotFoundException(string.Format("No Audio " +
                    "with id `{0}' is in the database", id
                ));
            }
        }
    }
}