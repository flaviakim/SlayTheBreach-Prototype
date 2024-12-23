public class TeleportEnemyMovement : IEnemyMovement {
    public MapTile TargetTile { get; }

    public IEnemyMovement.EnemyMovementStatus Status { get; private set; } = IEnemyMovement.EnemyMovementStatus.NotStarted;

    public TeleportEnemyMovement(MapTile targetTile) {
        TargetTile = targetTile;
    }

    public IEnemyMovement.EnemyMovementStatus UpdateMovement(Battle battle, Enemy enemy) {
        var success = enemy.Creature.TryMoveTo(TargetTile, true);
        Status = success ? IEnemyMovement.EnemyMovementStatus.FinishedSuccessfully : IEnemyMovement.EnemyMovementStatus.Failed;
        return Status;
    }

}