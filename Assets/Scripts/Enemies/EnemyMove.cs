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