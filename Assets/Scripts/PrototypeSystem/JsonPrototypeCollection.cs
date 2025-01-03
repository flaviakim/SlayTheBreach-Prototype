using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace PrototypeSystem {
    public abstract class JsonPrototypeCollection<TPrototypeData> : DictionaryPrototypeCollection<TPrototypeData> where TPrototypeData : PrototypeData {

        private readonly string _directoryPath;

        protected JsonPrototypeCollection(string directoryPath) {
            _directoryPath = directoryPath;
        }

        protected override Dictionary<string, TPrototypeData> LoadPrototypeDatas() {
            Debug.Log($"Loading prototypes from {_directoryPath}");
            var loadedPrototypes = new Dictionary<string, TPrototypeData>();

            var prototypeDatas = AssetLoader.LoadAllJson<TPrototypeData>(_directoryPath);
            foreach (var prototypeData in prototypeDatas) {
                if (prototypeData == null) {
                    Debug.LogError("Failed to load prototypeData");
                    continue;
                }
                if (prototypeData.IDName is null or "") {
                    Debug.LogError("PrototypeData has no IdName");
                    continue;
                }
                if (!loadedPrototypes.TryAdd(prototypeData.IDName, prototypeData)) {
                    Debug.LogError($"Duplicate prototype {prototypeData.IDName}");
                    continue;
                }
            }

            return loadedPrototypes;
        }

        public bool TrySavePrototypeData([NotNull] TPrototypeData prototypeData, bool overwrite) {
            if (string.IsNullOrEmpty(prototypeData.IDName)) {
                Debug.LogWarning("Prototype ID cannot be null or empty.");
                return false;
            }
            if (!AssetSaver.TrySaveJson(prototypeData, Path.Combine(_directoryPath, prototypeData.IDName + ".json"), overwrite)) {
                Debug.LogError($"Failed to save prototype {prototypeData.IDName}");
                return false;
            }
            Debug.Log($"Saved prototype {prototypeData.IDName}");
            return true;
        }


    }
}