using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class Enemy {

    [CanBeNull] public EnemyMove NextMove { get; private set; } = null;

    public readonly Creature Creature;
    private readonly IEnemyChoiceLogic _logic;

    public int MovementRange => Creature.Speed;
    public int AttackDamage => Creature.Strength;
    public string Name => Creature.CreatureName;

    public Enemy(Creature creature, IEnemyChoiceLogic enemyLogic) {
        Creature = creature;
        _logic = enemyLogic;
    }

    public void ChooseNextMove(Battle battle) {
        NextMove = _logic.ChooseMove(battle, this);
    }

}

public interface IEnemyChoiceLogic {

    public EnemyMove ChooseMove(Battle battle, Enemy enemy);

}

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

public class EnemyMove {

    public readonly Enemy Enemy;
    public readonly IEnemyMovement Movement;
    public readonly IEnemyEffect Effect;

    public EnemyMove(Enemy enemy, IEnemyMovement movement, IEnemyEffect effect) {
        Enemy = enemy;
        Movement = movement;
        Effect = effect;
    }

}

public interface IEnemyMovement {

    public void UpdateMovement(Battle battle, Enemy enemy, out bool movementFinished);

}

public class TeleportEnemyMovement : IEnemyMovement {

    public readonly MapTile TargetTile;

    public TeleportEnemyMovement(MapTile targetTile) {
        TargetTile = targetTile;
    }

    public void UpdateMovement(Battle battle, Enemy enemy, out bool movementFinished) {
        enemy.Creature.TryMoveTo(TargetTile, true);
        movementFinished = true;
    }

}

/// <summary>
/// The movement that an enemy can perform.
/// </summary>
public class PathEnemyMovement : IEnemyMovement {

    public readonly List<MapTile> Path;

    public PathEnemyMovement(List<MapTile> path) {
        Path = path;
    }

    public void UpdateMovement(Battle battle, Enemy enemy, out bool movementFinished) {
        if (Path.Count == 0) {
            Debug.Log($"{enemy.Creature.CreatureName} has no path to move");
            movementFinished = true;
            return;
        }

        // TODO for now we skip the path and just move to the last tile
        var targetTile = Path.First();

        if (!enemy.Creature.TryMoveTo(targetTile, allowTeleport: true)) { // TODO allow teleport for now
            Debug.LogError($"Trying to move {enemy.Creature.CreatureName} along it's path to {targetTile}, but it failed.");
            movementFinished = true;
            return;
        }

        Path.RemoveAt(0);
        movementFinished = Path.Count == 0;
    }

}

public class StayStillEnemyMovement : IEnemyMovement {

    public void UpdateMovement(Battle battle, Enemy enemy, out bool movementFinished) {
        Debug.Log($"{enemy.Creature.CreatureName} stays still");
        movementFinished = true;
    }

}

/// <summary>
/// The Attack, Buff, Debuff, etc. that an enemy can perform.
/// </summary>
public interface IEnemyEffect {

    public void UpdateEffect(Battle battle, Enemy enemy, out bool effectFinished);


}

public class DamageEnemyEffect : IEnemyEffect {

    public int Damage { get; }
    public MapTile TargetTile { get; }

    public DamageEnemyEffect(int damage, MapTile targetTile) {
        Damage = damage;
        TargetTile = targetTile;
    }

    public void UpdateEffect(Battle battle, Enemy enemy, out bool effectFinished) {
        var target = TargetTile.Occupant;
        if (target == null) {
            Debug.Log($"Trying to damage a target, but there is no target on the tile {TargetTile}");
            effectFinished = true;
            return;
        }

        Debug.Log($"{enemy.Creature.CreatureName} attacks {target.CreatureName} for {Damage} damage");
        target.TakeDamage(Damage);
        effectFinished = true;
    }

}

public class DoNothingEnemyEffect : IEnemyEffect {

    public void UpdateEffect(Battle battle, Enemy enemy, out bool effectFinished) {
        Debug.Log($"{enemy.Creature.CreatureName} does nothing");
        effectFinished = true;
    }

}
