using UnityEngine;

public class DamageEnemyEffect : IEnemyEffect {

    public int Damage { get; }
    public MapTile TargetTile { get; }

    public DamageEnemyEffect(int damage, MapTile targetTile) {
        Damage = damage;
        TargetTile = targetTile;
    }

    public void UpdateEffect(Battle battle, Enemy enemy, out bool effectFinished) {
        var target = TargetTile.Occupant;
        if (target == null) {
            Debug.Log($"Trying to damage a target, but there is no target on the tile {TargetTile}");
            effectFinished = true;
            return;
        }

        Debug.Log($"{enemy.Creature.CreatureName} attacks {target.CreatureName} for {Damage} damage");
        target.TakeDamage(Damage);
        effectFinished = true;
    }

}