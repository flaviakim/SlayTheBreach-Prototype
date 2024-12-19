public class DamageEffect : ICardEffect {
    public readonly int Damage;

    public string EffectName => "Damage";
    public string InstructionText => null;

    public DamageEffect(int damage) {
        this.Damage = damage;
    }

    public void StartEffect(CardEffectHandler handler) {
        foreach (var otherSelectedCreature in handler.OtherSelectedCreatures) {
            otherSelectedCreature.TakeDamage(Damage);
        }
    }
    public void UpdateEffect(CardEffectHandler handler, out bool effectFinished) {
        effectFinished = true;
    }
    public void EndEffect(CardEffectHandler handler) {

    }
}