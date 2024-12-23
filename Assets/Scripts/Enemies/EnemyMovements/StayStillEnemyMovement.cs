using UnityEngine;

public class StayStillEnemyMovement : IEnemyMovement {

    public void UpdateMovement(Battle battle, Enemy enemy, out bool movementFinished) {
        Debug.Log($"{enemy.Creature.CreatureName} stays still");
        movementFinished = true;
    }

}