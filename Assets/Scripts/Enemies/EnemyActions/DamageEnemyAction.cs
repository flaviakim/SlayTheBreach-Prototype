using UnityEngine;

public class DamageEnemyAction : IEnemyAction {
    public int Damage { get; }

    public string EffectName => "Damage";
    public string ExplanationText => $"Deals {Damage} damage to the target(s)";

    public DamageEnemyAction(int damage) {
        Damage = damage;
    }

    public void StartEffect(EnemyActionHandler handler) {
        if (handler.OtherTargets.Count == 0) {
            Debug.Log($"No targets to deal damage to for {handler.CurrentEnemyActor}");
            return;
        }
        foreach (var target in handler.OtherTargets) {
            Debug.Log($"{handler.CurrentEnemyActor} deals {Damage} damage to {target}");
            target.TakeDamage(Damage);
        }
    }

    public void UpdateEffect(EnemyActionHandler handler, out bool effectFinished) {
        effectFinished = true;
    }

    public void EndEffect(EnemyActionHandler handler) { }
}