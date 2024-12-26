using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

public class JsonCreatureLoader : ICreatureLoader {
    private readonly Dictionary<string, CreaturePrototype> _creatures = new();

    public void LoadAllCreaturePrototypes(Transform parentTransform) {
        var creaturesDirectoryPath = Path.Combine(Application.streamingAssetsPath, "Creatures");
        if (!Directory.Exists(creaturesDirectoryPath)) {
            Debug.LogError($"Directory '{creaturesDirectoryPath}' does not exist.");
            return;
        }

        var creatureFiles = Directory.GetFiles(creaturesDirectoryPath, "*.json", SearchOption.AllDirectories);
        foreach (var creatureFilePath in creatureFiles) {
            var creature = LoadCreatureFromJson(creatureFilePath, parentTransform);
            _creatures.Add(creature.CreatureId, creature);
        }
    }

    public CreaturePrototype GetCreaturePrototypeById(string creatureId) {
        if (!_creatures.TryGetValue(creatureId, out var creature)) {
            Debug.LogError($"Creature with ID {creatureId} not found");
            return null!;
        }

        return creature;
    }

    public CreaturePrototype LoadCreatureFromJson(string jsonFilePath, Transform parentTransform) {
        var creatureData = JsonConvert.DeserializeObject<Creature.CreatureData>(File.ReadAllText(jsonFilePath));
        var creature = new CreaturePrototype(creatureData, parentTransform);
        Debug.Log($"Loaded creature: {creature.CreatureName}");
        return creature;
    }

    public bool TrySaveCreatureData([NotNull] Creature.CreatureData creatureData, bool overwrite) {
        if (string.IsNullOrEmpty(creatureData.CreatureId)) {
            Debug.LogWarning("Creature ID cannot be null or empty.");
            return false;
        }
        var json = JsonConvert.SerializeObject(creatureData, Formatting.Indented);
        var creatureFilePath = Path.Combine(Application.streamingAssetsPath, "Creatures", $"{creatureData.CreatureId}.json");
        if (File.Exists(creatureFilePath) && !overwrite) {
            Debug.LogWarning($"Creature with ID {creatureData.CreatureId} already exists.");
            return false;
        }
        File.WriteAllText(creatureFilePath, json);
        return true;
    }
}


