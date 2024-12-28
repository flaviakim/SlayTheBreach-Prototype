using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

public interface IPrototypeCollection<TPrototypeData> where TPrototypeData : PrototypeData {
    [CanBeNull] public TPrototypeData TryGetPrototypeForName(string idName);
    public List<string> GetPrototypeNames();
    public List<TPrototypeData> GetPrototypes();
}

public abstract class DictionaryPrototypeCollection<TPrototypeData> : IPrototypeCollection<TPrototypeData> where TPrototypeData : PrototypeData {
    private Dictionary<string, TPrototypeData> _prototypes;
    private Dictionary<string, TPrototypeData> Prototypes {
        get {
            if (_prototypes != null) return _prototypes;
            _prototypes = LoadPrototypes();
            return _prototypes;
        }
    }

    public TPrototypeData TryGetPrototypeForName(string name) {
        Prototypes.TryGetValue(name, out var prototype);
        return prototype;
    }

    public List<string> GetPrototypeNames() {
        return new List<string>(Prototypes.Keys);
    }

    public List<TPrototypeData> GetPrototypes() {
        return new List<TPrototypeData>(Prototypes.Values);
    }

    protected abstract Dictionary<string, TPrototypeData> LoadPrototypes();
}

public class JsonPrototypeCollection<TPrototypeData> : DictionaryPrototypeCollection<TPrototypeData> where TPrototypeData : PrototypeData {

    private readonly string _directoryPath;

    public JsonPrototypeCollection(string directoryPath) {
        _directoryPath = directoryPath;
    }

    protected override Dictionary<string, TPrototypeData> LoadPrototypes() {
        Debug.Log($"Loading prototypes from {_directoryPath}");
        var loadedPrototypes = new Dictionary<string, TPrototypeData>();

        var directoryPath = Path.Combine(Application.streamingAssetsPath, _directoryPath);
        if (!Directory.Exists(directoryPath)) {
            Debug.LogError($"Directory '{directoryPath}' does not exist.");
            return loadedPrototypes;
        }

        var filesNames = Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories);
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


}