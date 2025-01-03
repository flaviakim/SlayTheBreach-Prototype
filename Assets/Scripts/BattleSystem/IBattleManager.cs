public interface IBattleManager {
    /// <summary>
    /// The managers are created before the battle is created. This method is called immediately after the battle is created.
    /// </summary>
    /// <param name="battle"> The battle that the manager is managing. </param>
    public void Initialize(Battle battle);
}