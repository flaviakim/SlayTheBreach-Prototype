using System.Linq;
using UnityEngine;

public class DamageNearestTargetEnemyChoiceLogic : IEnemyChoiceLogic {

    public EnemyMove ChooseMove(Battle battle, Enemy enemy) {
        var targetCreature = battle.CreaturesManager.CreaturesInBattle
            .Where(creature => creature.Faction == Faction.Player)
            .OrderBy(creature => battle.BattleMap.GetDistanceBetweenTiles(enemy.Creature.CurrentTile, creature.CurrentTile))
            .FirstOrDefault();
        if (targetCreature == null) {
            Debug.LogError("Trying to choose enemy turn, but there is no target player creature");
            return new EnemyMove(enemy, new StayStillEnemyMovement(), new DoNothingEnemyEffect());
        }
        var targetTile = targetCreature.CurrentTile;
        var movement = new TeleportEnemyMovement(targetTile);
        // var movement = new PathEnemyMovement(battle.BattleMap.GetPathBetweenTiles(enemy.Creature.CurrentTile, targetTile, stopNextToTarget: true, maxMovement: enemy.MovementRange));
        var effect = new DamageEnemyEffect(enemy.AttackDamage, targetTile);
        return new EnemyMove(enemy, movement, effect);
    }

}