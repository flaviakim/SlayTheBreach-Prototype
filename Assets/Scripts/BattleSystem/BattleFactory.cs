using System;
using PrototypeSystem;
using UnityEngine;

public class BattleFactory : InstanceFactory<Battle, BattleData, BattleFactory> {
    protected override IPrototypeCollection<BattleData> PrototypeCollection { get; } = new BattlePrototypeCollection();

    private readonly BattleMapFactory _battleMapFactory = new();
    private readonly WinConditionFactory _winConditionFactory = new();

    public Battle CreateBattleInstance(string battleId) {
        var battleData = PrototypeCollection.TryGetPrototypeForName(battleId);
        if (battleData == null) {
            Debug.LogError($"Battle with id {battleId} not found.");
            return null;
        }

        var battleMap = _battleMapFactory.CreateBattleMap(battleData.BattleMapIDName);
        var winConditions = new IWinCondition[battleData.WinConditionIds.Length];
        for (var i = 0; i < battleData.WinConditionIds.Length; i++) {
            winConditions[i] = _winConditionFactory.CreateWinCondition(battleData.WinConditionIds[i]);
        }

        var battle = new Battle(battleId, battleData.StartHandSize, battleMap, winConditions);

        SpawnEnemies(battle, battleMap, battleData.EnemyCreatureIdsToSpawn);
        SpawnPlayerCreatures(battle, battleMap, battleData.PlayerCreatureIdsToSpawn);

        battle.Initialize();

        return battle;
    }


    private void SpawnEnemies(Battle battle, BattleMap map, string[] enemyCreatureIds) {
        var enemyManager = battle.EnemyManager;
        foreach (var enemyCreatureId in enemyCreatureIds) {
            enemyManager.SpawnEnemy("Debug", enemyCreatureId, GetRandomFreePosition(map, Faction.Enemy));
        }
    }

    private void SpawnPlayerCreatures(Battle battle, BattleMap map, string[] playerCreatureIds) {
        var creatureManager = battle.CreaturesManager;
        foreach (var playerCreatureId in playerCreatureIds) {
            creatureManager.SpawnCreature(playerCreatureId, GetRandomFreePosition(map, Faction.Player));
        }
    }


    private MapTile GetRandomFreePosition(BattleMap map, Faction faction) {
        var validTiles = map.GetTilesWhere(tile => {
            var isCorrectStartingArea = faction == Faction.Enemy
                ? tile.Position.y > map.Height / 2
                : tile.Position.y < map.Height / 2;
            // var isWalkable = tile.IsWalkable;
            var isOccupied = tile.IsOccupied;
            return isCorrectStartingArea && !isOccupied;
        });
        if (validTiles.Count == 0) {
            throw new Exception("No free tiles found");
        }
        var randomTile = validTiles[UnityEngine.Random.Range(0, validTiles.Count)];
        return randomTile;
    }
}