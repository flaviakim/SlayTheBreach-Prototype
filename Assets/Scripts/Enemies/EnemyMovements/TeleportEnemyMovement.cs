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