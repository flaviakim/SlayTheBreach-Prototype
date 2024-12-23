public interface IEnemyChoiceLogic {

    public bool UpdatesDuringPlayerMove { get; }
    public EnemyMove ChooseMove(Battle battle, Enemy enemy);

}