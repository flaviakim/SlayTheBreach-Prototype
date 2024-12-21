using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class JsonCreatureLoader : ICreatureLoader {
    private readonly Dictionary<string, CreaturePrototype> _playerCreaturePrototypes = new();
    private readonly Dictionary<string, CreaturePrototype> _autonomousCreaturePrototypes = new();

    public void LoadAllCreaturePrototypes(Transform parentTransform) {
        const string playerCreaturesDirectoryName = "PlayerCreatures";
        const string autonomousCreaturesDirectoryName = "AutonomousCreatures";

        var playerCreaturesDirectoryPath = Path.Combine(Application.streamingAssetsPath, playerCreaturesDirectoryName);
        if (!Directory.Exists(playerCreaturesDirectoryPath)) {
            Debug.LogError($"Directory '{playerCreaturesDirectoryPath}' does not exist.");
            return;
        }

        var playerCreatureFiles = Directory.GetFiles(playerCreaturesDirectoryPath, "*.json", SearchOption.AllDirectories);
        foreach (var creatureFilePath in playerCreatureFiles) {
            var creature = LoadPlayerCreatureFromJson(creatureFilePath, parentTransform);
            if (!_playerCreaturePrototypes.TryAdd(creature.CreatureId, creature)) {
                Debug.LogError($"Creature with ID {creature.CreatureId} already exists");
                continue;
            }
        }

        var autonomousCreaturesDirectoryPath = Path.Combine(Application.streamingAssetsPath, autonomousCreaturesDirectoryName);
        if (!Directory.Exists(autonomousCreaturesDirectoryPath)) {
            Debug.LogError($"Directory '{autonomousCreaturesDirectoryPath}' does not exist.");
            return;
        }

        var autonomousCreatureFiles = Directory.GetFiles(autonomousCreaturesDirectoryPath, "*.json", SearchOption.AllDirectories);
        foreach (var creatureFilePath in autonomousCreatureFiles) {
            var creature = LoadAutonomousCreatureFromJson(creatureFilePath, parentTransform);
            if (!_autonomousCreaturePrototypes.TryAdd(creature.CreatureId, creature)) {
                Debug.LogError($"Creature with ID {creature.CreatureId} already exists");
                continue;
            }
        }
    }

    public PlayerCreaturePrototype GetPlayerCreaturePrototypeById(string creatureId) {
        if (!_playerCreaturePrototypes.TryGetValue(creatureId, out var creature)) {
            Debug.LogError($"Creature with ID {creatureId} not found");
            return null!;
        }

        return creature as PlayerCreaturePrototype;
    }

    public AutonomousCreaturePrototype GetAutonomousCreaturePrototypeById(string creatureId) {
        if (!_autonomousCreaturePrototypes.TryGetValue(creatureId, out var creature)) {
            Debug.LogError($"Creature with ID {creatureId} not found");
            return null!;
        }

        return creature as AutonomousCreaturePrototype;
    }

    private AutonomousCreaturePrototype LoadAutonomousCreatureFromJson(string jsonFilePath, Transform parentTransform) {
        var creatureData = JsonConvert.DeserializeObject<AutonomousCreatureData>(File.ReadAllText(jsonFilePath));
        var sprite = AssetLoader.LoadSprite(creatureData.SpritePath);
        var creature = new AutonomousCreaturePrototype(creatureData.CreatureId, creatureData.CreatureName, creatureData.Health, creatureData.Strength, creatureData.Defense,
            creatureData.RangedAttack, creatureData.Speed, creatureData.Faction, sprite, parentTransform);
        Debug.Log($"Loaded creature: {creature.CreatureName}");
        return creature;
    }


    private PlayerCreaturePrototype LoadPlayerCreatureFromJson(string jsonFilePath, Transform parentTransform) {
        var creatureData = JsonConvert.DeserializeObject<PlayerCreatureData>(File.ReadAllText(jsonFilePath));
        var sprite = AssetLoader.LoadSprite(creatureData.SpritePath);
        var creature = new PlayerCreaturePrototype(creatureData.CreatureId, creatureData.CreatureName, creatureData.Health, creatureData.Strength, creatureData.Defense,
            creatureData.RangedAttack, creatureData.Speed, creatureData.Faction, sprite, parentTransform);
        Debug.Log($"Loaded creature: {creature.CreatureName}");
        return creature;
    }



    private abstract class CreatureData {
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

    private class PlayerCreatureData : CreatureData {

    }

    private class AutonomousCreatureData : CreatureData {

    }
}