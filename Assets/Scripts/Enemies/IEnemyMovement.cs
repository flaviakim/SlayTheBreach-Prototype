public interface IEnemyMovement {

    public void UpdateMovement(Battle battle, Enemy enemy, out bool movementFinished);

}