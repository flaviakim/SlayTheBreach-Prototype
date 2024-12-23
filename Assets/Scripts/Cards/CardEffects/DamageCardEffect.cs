public class DamageCardEffect : ICardEffect {
    public int Damage { get; }

    public string EffectName => "Damage";
    public string InstructionText => null;

    public DamageCardEffect(int damage) {
        Damage = damage;
    }

    public void StartEffect(CardEffectHandler handler) {
        foreach (var otherSelectedCreature in handler.OtherSelectedCreatures) {
            otherSelectedCreature.TakeDamage(Damage);
        }
    }

    public void OnSelectedTile(CardEffectHandler handler, MapTile selectedTile, out bool effectFinished) {
        effectFinished = false;
    }

    public void UpdateEffect(CardEffectHandler handler, out bool effectFinished) {
        effectFinished = true;
    }

    public void EndEffect(CardEffectHandler handler) { }
}