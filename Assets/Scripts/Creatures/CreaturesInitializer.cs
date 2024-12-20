using System;
using UnityEngine;

public class CreaturesInitializer : MonoBehaviour {

    [SerializeField] private string[] enemyCreatureIdsToSpawn = Array.Empty<string>();
    [SerializeField] private string[] playerCreatureIdsToSpawn = Array.Empty<string>();

    private void Start() {
        var map = Battle.CurrentBattle.BattleMap;
        var creatureManager = Battle.CurrentBattle.CreaturesManager;
        if (map == null) {
            throw new Exception("BattleMap not found");
        }
        if (creatureManager == null) {
            throw new Exception("CreaturesManager not found");
        }

        foreach (var enemyCreatureId in enemyCreatureIdsToSpawn) {
            creatureManager.SpawnCreature(enemyCreatureId, GetRandomFreePosition(map, Faction.Enemy));
        }

        foreach (var playerCreatureId in playerCreatureIdsToSpawn) {
            creatureManager.SpawnCreature(playerCreatureId, GetRandomFreePosition(map, Faction.Player));
        }

        Debug.Log("Creatures initialized");
    }

    private MapTile GetRandomFreePosition(BattleMap map, Faction faction) {
        var validTiles = map.GetTilesWhere(tile => {
            var isCorrectStartingArea = faction == Faction.Enemy
                ? tile.Position.y > map.Height / 2
                : tile.Position.y < map.Height / 2;
            var isWalkable = tile.Type != TileType.Water;
            var isOccupied = tile.IsOccupied;
            return isCorrectStartingArea && isWalkable && !isOccupied;
        });
        if (validTiles.Count == 0) {
            throw new Exception("No free tiles found");
        }
        var randomTile = validTiles[UnityEngine.Random.Range(0, validTiles.Count)];
        return randomTile;
    }

}