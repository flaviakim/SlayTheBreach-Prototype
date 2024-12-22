using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class EnemyManager {
    public Battle Battle { get; }
    private readonly List<Creature> _enemies = new();
    private int _currentEnemyIndex = 0;
    private readonly EnemyActionHandler _enemyActionHandler;

    [CanBeNull] public Creature CurrentEnemy {
        get {
            if (Battle.CurrentTurn != Faction.Enemy) {
                Debug.LogWarning("Trying to get current enemy, but it is not enemy's turn");
                return null;
            }
            if (_enemies.Count == 0) {
                return null;
            }
            else {
                return _enemies[_currentEnemyIndex];
            }
        }
    }

    private EnemyActionSet DebugEnemyActionSet => new(
        "DebugEnemyActionSet",
        "Debug enemy action set",
        new RandomMeleeTargetSelectionEnemyAction(),
        new DamageEnemyAction(5)
    );

    public EnemyManager(Battle battle) {
        Battle = battle;
        _enemyActionHandler = new EnemyActionHandler(battle);
    }

    public void UpdateEnemies() {
        if (Battle.CurrentTurn != Faction.Enemy) return;
        if (CurrentEnemy == null) {
            Debug.LogError("Trying to update enemy manager, but there is no current enemy");
            return;
        }

        _enemyActionHandler.UpdateCurrentEnemyActionSet(out var moveSetGotFinished);
        if (moveSetGotFinished) {
            MoveToNextEnemy();
        }
    }


    public void OnBattleStart() {
        foreach (var enemyCreature in Battle.CreaturesManager.CreaturesInBattle.Where(creature => creature.Faction == Faction.Enemy)) {
            _enemies.Add(enemyCreature);
            enemyCreature.DeathEvent += OnEnemyDeath;
        }
    }

    private void OnEnemyDeath(object sender, DeathEventArgs e) {
        var deadEnemyIndex = _enemies.IndexOf(e.DeadCreature);
        if (deadEnemyIndex == -1) {
            Debug.LogError("Trying to remove dead enemy, but it is not in the list of enemies");
            return;
        }
        _enemies.RemoveAt(deadEnemyIndex);
        if (_currentEnemyIndex > deadEnemyIndex) {
            _currentEnemyIndex--;
        }
        if (_enemies.Count == 0) {
            Debug.Log("EnemyManager - All enemies are dead");
        }
    }

    public void StartEnemyTurn() {
        Debug.Log("Starting enemy turn");
        if (_enemies.Count == 0) {
            Debug.LogError("Trying to start enemy turn, but there are no enemies");
            return;
        }
        _currentEnemyIndex = -1;
        MoveToNextEnemy();
    }

    private void MoveToNextEnemy() {
        Debug.Log($"Moving to next enemy {_currentEnemyIndex} -> {_currentEnemyIndex + 1} (total {_enemies.Count})");
        _currentEnemyIndex++;
        if (_currentEnemyIndex >= _enemies.Count) {
            Battle.EndEnemyTurn();
            return;
        }
        Debug.Assert(Battle.CurrentTurn == Faction.Enemy, "Trying to start next enemy actionset, but it is not enemy's turn");
        Debug.Assert(CurrentEnemy != null, "Trying to start next enemy actionset, but there is no current enemy");
        _enemyActionHandler.StartEnemyActionSet(CurrentEnemy, DebugEnemyActionSet);
    }

}