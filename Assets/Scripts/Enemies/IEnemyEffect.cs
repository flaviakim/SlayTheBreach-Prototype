/// <summary>
/// The Attack, Buff, Debuff, etc. that an enemy can perform.
/// </summary>
public interface IEnemyEffect {

    public void UpdateEffect(Battle battle, Enemy enemy, out bool effectFinished);


}