using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : IBattleManager {
    public event EventHandler OnLastEnemyDiedEvent;

    private readonly List<Enemy> _enemies = new();
    private int _currentEnemyIndex = 0;
    private readonly EnemyMoveHandler _enemyMoveHandler;
    private readonly Battle _battle;

    public int EnemiesCount => _enemies.Count;


    public Enemy CurrentEnemy {
        get {
            if (_battle.CurrentTurn != Faction.Enemy) {
                Debug.LogWarning("Trying to get current enemy, but it is not enemy's turn");
                return null;
            }

            if (_enemies.Count == 0) {
                Debug.LogWarning("Trying to get current enemy, but there are no enemies");
                return null;
            }

            return _enemies[_currentEnemyIndex];
        }
    }

    public EnemyManager(Battle battle) {
        _battle = battle;
        _enemyMoveHandler = new EnemyMoveHandler(battle);
    }

    public void Initialize(Battle battle) { }

    public void SpawnEnemy(string enemyLogicId, string enemyCreatureId, MapTile spawnTile) {
        var enemyCreature = _battle.CreaturesManager.SpawnCreature(enemyCreatureId, spawnTile);
        var enemyLogic = GetLogicById(enemyLogicId);
        var enemy = new Enemy(enemyCreature, enemyLogic);
        _enemies.Add(enemy);
        enemyCreature.DeathEvent += OnEnemyDeath;
    }

    public void OnBattleStart() {
        Debug.Assert(_enemies.All(enemy => enemy.NextMove == null), "Trying to start battle, but enemies already have moves");
        Debug.Assert(_enemies.Count == _battle.CreaturesManager.CreaturesInBattle.Count(creature => !creature.IsPlayerControlled),
            "Trying to start battle, but number of enemies does not match number of enemy creatures");
        Debug.Assert(
            _battle.CreaturesManager.CreaturesInBattle.Where(creature => !creature.IsPlayerControlled)
                .All(creature => _enemies.Any(enemy => enemy.Creature == creature)),
            "Trying to start battle, but not all enemy creatures are in the list of enemies");
        foreach (var enemy in _enemies) {
            enemy.ChooseNextMove(_battle);
        }

        Debug.Assert(_enemies.All(enemy => enemy.NextMove != null), "Trying to start battle, but enemies have no moves");

        _battle.OnPlayerHasPlayedCard += (sender, args) => ChooseEnemyMoves(updateOnly: true);
    }


    public void ChooseEnemyMoves(bool updateOnly = false) {
        foreach (var enemy in _enemies) {
            enemy.ChooseNextMove(_battle, updateOnly);
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

    public void UpdateEnemies() {
        if (_battle.CurrentTurn != Faction.Enemy) return;
        if (CurrentEnemy == null) {
            Debug.LogError("Trying to update enemy manager, but there is no current enemy");
            return;
        }

        _enemyMoveHandler.UpdateCurrentEnemyMove(out var moveGotFinished);
        if (moveGotFinished) {
            MoveToNextEnemy();
        }
    }

    private void MoveToNextEnemy() {
        Debug.Log($"Moving to next enemy {_currentEnemyIndex} -> {_currentEnemyIndex + 1} (total {_enemies.Count})");
        _currentEnemyIndex++;
        if (_currentEnemyIndex >= _enemies.Count) {
            _battle.EndEnemyTurn();
            return;
        }

        Debug.Assert(_battle.CurrentTurn == Faction.Enemy, "Trying to start next enemy actionset, but it is not enemy's turn");
        Debug.Assert(CurrentEnemy != null, "Trying to start next enemy actionset, but there is no current enemy");
        _enemyMoveHandler.StartEnemyMove(CurrentEnemy);
    }

    private void OnEnemyDeath(object sender, DeathEventArgs e) {
        var deadEnemyIndex = _enemies.FindIndex(enemy => enemy.Creature == e.DeadCreature);
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
            OnLastEnemyDiedEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    private IEnemyChoiceLogic GetLogicById(string enemyLogicId) {
        Debug.Log("Enemy logic retrieving in GetLogicById not implemented, using default");
        return new DamageNearestTargetEnemyChoiceLogic();
    }
}