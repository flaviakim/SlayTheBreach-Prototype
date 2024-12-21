using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    private CreaturePrototype LoadCreatureFromJson(string jsonFilePath, Transform parentTransform) {
        var creatureData = JsonConvert.DeserializeObject<CreatureData>(File.ReadAllText(jsonFilePath));
        var sprite = AssetLoader.LoadSprite(creatureData.SpritePath);
        var creature = new CreaturePrototype(creatureData.CreatureId, creatureData.CreatureName, creatureData.Health, creatureData.Strength, creatureData.Defense,
            creatureData.RangedAttack, creatureData.Speed, creatureData.Faction, sprite, parentTransform);
        Debug.Log($"Loaded creature: {creature.CreatureName}");
        return creature;
    }



    private class CreatureData {
        public string CreatureId;
        public string CreatureName;
        public int Health;
        public int Strength;
        public int Defense;
        public int RangedAttack;
        public int Speed;
        public Faction Faction;
        public string SpritePath;
    }
}