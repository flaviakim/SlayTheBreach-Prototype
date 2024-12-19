using JetBrains.Annotations;

public interface ICardEffect {
    string EffectName { get; }
    [CanBeNull] string InstructionText { get; }

    void StartEffect(CardEffectHandler handler);

    void OnSelectedTile(CardEffectHandler handler, MapTile selectedTile, out bool effectFinished);

    void UpdateEffect(CardEffectHandler handler, out bool effectFinished);

    void EndEffect(CardEffectHandler handler);


}