using JetBrains.Annotations;

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