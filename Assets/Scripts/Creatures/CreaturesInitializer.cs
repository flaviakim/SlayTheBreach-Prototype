using System;
using UnityEngine;

public class CreaturesInitializer : MonoBehaviour {

    [SerializeField] private int playerCreaturesCount = 3;
    [SerializeField] private int enemyCreaturesCount = 3;

    [SerializeField] private Creature[] playerCreaturePrefabs;
    [SerializeField] private Creature[] enemyCreaturePrefabs;


    private void Start() {
        var map = FindFirstObjectByType<BattleMap>();
        if (map == null) {
            throw new Exception("BattleMap not found");
        }

        for (int i = 0; i < playerCreaturesCount; i++) {
            var creature = Instantiate(playerCreaturePrefabs[UnityEngine.Random.Range(0, playerCreaturePrefabs.Length)]);
            creature.Initialize(GetRandomFreePosition(map, Faction.Player));
        }

        for (int i = 0; i < enemyCreaturesCount; i++) {
            var creature = Instantiate(enemyCreaturePrefabs[UnityEngine.Random.Range(0, enemyCreaturePrefabs.Length)]);
            creature.Initialize(GetRandomFreePosition(map, Faction.Enemy));
        }
    }

    private void Update() {
        Destroy(gameObject);
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