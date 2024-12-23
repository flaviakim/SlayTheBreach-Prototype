using UnityEngine;

public class DoNothingEnemyEffect : IEnemyEffect {

    public void UpdateEffect(Battle battle, Enemy enemy, out bool effectFinished) {
        Debug.Log($"{enemy.Creature.CreatureName} does nothing");
        effectFinished = true;
    }

}