public interface IEnemyChoiceLogic {

    /// <summary>
    /// Whether the logic should be recalculated when the player moves their creatures.
    /// </summary>
    public bool UpdatesDuringPlayerMove { get; }
    public EnemyMove ChooseMove(Battle battle, Enemy enemy);

}