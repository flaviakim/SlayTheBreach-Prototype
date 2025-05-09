using System.Linq;
using UnityEngine;

public class DamageNearestTargetEnemyChoiceLogic : IEnemyChoiceLogic {

    public bool UpdatesDuringPlayerMove => true;

    public EnemyMove ChooseMove(Battle battle, Enemy enemy) {
        var enemyTile = enemy.Creature.CurrentTile;
        var targetTile = battle.BattleMap.GetTilesInRange(enemyTile, enemy.MovementRange + 1,
            tile => tile.Occupant != null && tile.Occupant.Faction != Faction.Enemy, includeFromTile: false)
            .OrderBy(tile => battle.BattleMap.GetDistanceBetweenTiles(enemyTile, tile))
            .FirstOrDefault();
        if (targetTile == null) {
            Debug.Log($"No player creature in range");
            return new EnemyMove(enemy, new StayStillEnemyMovement(), new DoNothingEnemyEffect());
        }

        var tileNextToTarget = battle.BattleMap
            .GetTilesInRange(targetTile, 1, includeFromTile: false)
            .Where(tile => enemy.Creature.CanMoveTo(tile, true) || tile == enemyTile) // tile == enemyTile is for the case when the enemy is already next to the target
            .FirstOrDefault(tile => battle.BattleMap.GetDistanceBetweenTiles(enemyTile, tile) <= enemy.MovementRange);
        if (tileNextToTarget == null) {
            Debug.Log($"Enemy {enemy.Name} cannot reach target {targetTile.Occupant?.CreatureName}");
            return new EnemyMove(enemy, new StayStillEnemyMovement(), new DoNothingEnemyEffect());
        }

        var movement = new TeleportEnemyMovement(tileNextToTarget);
        // var movement = new PathEnemyMovement(battle.BattleMap.GetPathBetweenTiles(enemyTile, targetTile, stopNextToTarget: true, maxMovement: enemy.MovementRange));
        var effect = new DamageEnemyEffect(enemy.AttackDamage, targetTile);
        return new EnemyMove(enemy, movement, effect);
    }

}