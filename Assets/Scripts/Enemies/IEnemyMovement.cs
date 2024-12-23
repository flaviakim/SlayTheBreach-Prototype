public interface IEnemyMovement {

    public EnemyMovementStatus Status { get; }

    public EnemyMovementStatus UpdateMovement(Battle battle, Enemy enemy);

    public enum EnemyMovementStatus {
        NotStarted,
        InProgress,
        FinishedSuccessfully,
        Failed
    }
}

