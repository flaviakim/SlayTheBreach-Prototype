using JetBrains.Annotations;
using UnityEngine;

public class EnemyMoveHandler {

    private const float TIME_BETWEEN_ENEMY_MOVES = 1f;

    public Battle Battle { get; }

    [CanBeNull] public Enemy CurrentEnemy { get; private set; }
    private MovementPhase _currentPhase;
    private float _timeSinceLastMove = 0f;

    public EnemyMoveHandler(Battle battle) {
        Battle = battle;
    }

    public void StartEnemyMove(Enemy enemy) {
        CurrentEnemy = enemy;
        if (enemy.NextMove == null) {
            Debug.LogError($"Enemy {enemy.Name} has no next turn, choosing new turn");
            enemy.ChooseNextMove(Battle);
        }
        _currentPhase = MovementPhase.Initialization;
        _timeSinceLastMove = 0f;
        Debug.Log($"Starting enemy move for {enemy.Name}");
    }


    public void UpdateCurrentEnemyMove(out bool moveGotFinished) {
        moveGotFinished = false;
        var enemy = CurrentEnemy;
        if (enemy == null) {
            Debug.LogError("Trying to update current enemy move, but there is no current enemy");
            return;
        }
        var enemyMove = enemy.NextMove;
        if (enemyMove == null) {
            Debug.LogError("Trying to update current enemy move, but there is no next move");
            moveGotFinished = true;
            return;
        }

        _timeSinceLastMove += Time.deltaTime;

        switch (_currentPhase) {
            case MovementPhase.Initialization:
                // Wait for a bit before starting the move
                if (_timeSinceLastMove < TIME_BETWEEN_ENEMY_MOVES) {
                    break;
                }
                _currentPhase = MovementPhase.Movement;
                break;
            case MovementPhase.Movement:
                enemyMove.Movement.UpdateMovement(Battle, enemy, out var movementFinished);
                if (movementFinished) {
                    _currentPhase = MovementPhase.Effect;
                }
                break;
            case MovementPhase.Effect:
                enemyMove.Effect.UpdateEffect(Battle, enemy, out var effectFinished);
                if (effectFinished) {
                    _currentPhase = MovementPhase.End;
                }
                break;
            case MovementPhase.End:
                moveGotFinished = true;
                break;
            default:
                Debug.LogError("Trying to update enemy move, but in unknown phase");
                break;
        }
    }

    private enum MovementPhase {
        Initialization,
        Movement,
        Effect,
        End,
    }

}