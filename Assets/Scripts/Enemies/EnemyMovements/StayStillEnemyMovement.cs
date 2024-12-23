using UnityEngine;

public class StayStillEnemyMovement : IEnemyMovement {
    public IEnemyMovement.EnemyMovementStatus Status { get; private set; } = IEnemyMovement.EnemyMovementStatus.NotStarted;

    public IEnemyMovement.EnemyMovementStatus UpdateMovement(Battle battle, Enemy enemy) {
        Debug.Log($"{enemy.Creature.CreatureName} stays still");
        Status = IEnemyMovement.EnemyMovementStatus.FinishedSuccessfully;
        return Status;
    }

}