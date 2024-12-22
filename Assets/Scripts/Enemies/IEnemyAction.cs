public interface IEnemyAction {
    string EffectName { get; }
    string ExplanationText { get; }

    void StartEffect(EnemyActionHandler handler);

    void UpdateEffect(EnemyActionHandler handler, out bool effectFinished);

    void EndEffect(EnemyActionHandler handler);
}