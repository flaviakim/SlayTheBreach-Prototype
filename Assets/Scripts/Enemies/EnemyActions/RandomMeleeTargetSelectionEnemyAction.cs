using UnityEngine;

public class RandomMeleeTargetSelectionEnemyAction : IEnemyAction {
    public string EffectName => "Nearest Target Selection";
    public string ExplanationText => "Selects the nearest target";

    public void StartEffect(EnemyActionHandler handler) {
        handler.OtherTargets.Clear();
        var tiles = handler.Battle.BattleMap.GetTilesInRange(handler.CurrentEnemyActor.CurrentTile, 1,
            tile => tile.Occupant != null && tile.Occupant.Faction != handler.CurrentEnemyActor.Faction);
        if (tiles.Count == 0) {
            Debug.Log($"No targets in range for {handler.CurrentEnemyActor}");
            return;
        }

        var targetTile = tiles[Random.Range(0, tiles.Count)];
        handler.OtherTargets.Add(targetTile.Occupant);
        Debug.Log($"{handler.CurrentEnemyActor} targets {targetTile.Occupant}");
    }

    public void UpdateEffect(EnemyActionHandler handler, out bool effectFinished) {
        effectFinished = true;
    }

    public void EndEffect(EnemyActionHandler handler) { }
}