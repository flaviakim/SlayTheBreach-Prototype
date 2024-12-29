using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace PrototypeSystem {
    public abstract class JsonPrototypeCollection<TPrototypeData> : DictionaryPrototypeCollection<TPrototypeData> where TPrototypeData : PrototypeData {

        private readonly string _directoryPath;

        private string CompleteDirectoryPath => Path.Combine(Application.streamingAssetsPath, _directoryPath);

        protected JsonPrototypeCollection(string directoryPath) {
            _directoryPath = directoryPath;
        }

        protected override Dictionary<string, TPrototypeData> LoadPrototypeDatas() {
            Debug.Log($"Loading prototypes from {_directoryPath}");
            var loadedPrototypes = new Dictionary<string, TPrototypeData>();

            var completeDirectoryPath = CompleteDirectoryPath;
            if (!Directory.Exists(completeDirectoryPath)) {
                Debug.LogError($"Directory '{completeDirectoryPath}' does not exist.");
                return loadedPrototypes;
            }

            var filesNames = Directory.GetFiles(completeDirectoryPath, "*.json", SearchOption.AllDirectories);
            foreach (var fileName in filesNames) {
                var prototypeData = JsonConvert.DeserializeObject<TPrototypeData>(File.ReadAllText(fileName));
                if (prototypeData == null) {
                    Debug.LogError($"Failed to load prototypeData from {fileName}");
                    continue;
                }
                if (prototypeData.IDName is null or "") {
                    Debug.LogError($"PrototypeData {fileName} has no IdName");
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
            var json = JsonConvert.SerializeObject(prototypeData, Formatting.Indented);
            var prototypeDataFilePath = Path.Combine(CompleteDirectoryPath, $"{prototypeData.IDName}.json");
            if (File.Exists(prototypeDataFilePath) && !overwrite) {
                Debug.LogWarning($"PrototypeData file with ID {prototypeData.IDName} already exists.");
                return false;
            }
            File.WriteAllText(prototypeDataFilePath, json);
            Debug.Log($"Saved data to {prototypeDataFilePath}");
            return true;
        }


    }
}