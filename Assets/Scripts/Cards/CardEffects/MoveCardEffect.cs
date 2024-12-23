using UnityEngine;

public class MoveCardEffect : ICardEffect {
    public string EffectName => "Move";
    public string InstructionText => "Select a tile to move to";

    public int Range { get; }
    public int RemainingMoves { get; private set; }

    public MoveCardEffect(int range) {
        Range = range;
    }

    public void StartEffect(CardEffectHandler handler) {
        RemainingMoves = Range;
    }

    public void OnSelectedTile(CardEffectHandler handler, MapTile selectedTile, out bool effectFinished) {
        // TODO for now we just move one tile at a time, but we should add pathfinding at some point
        effectFinished = false;
        var currentCreature = handler.CurrentCardTarget;
        var currentTile = handler.CurrentCardTarget.CurrentTile;
        if (BattleMap.DistanceBetweenTiles(handler.CurrentCardTarget.CurrentTile, selectedTile) != 1) {
            return;
        }
        if (selectedTile.Occupant != null) {
            return;
        }
        if (!currentCreature.TryMoveTo(selectedTile)) {
            return;
        }

        RemainingMoves--;

        Debug.Assert(RemainingMoves >= 0, "Remaining moves should never be negative");
        effectFinished = RemainingMoves <= 0;
    }

    public void UpdateEffect(CardEffectHandler handler, out bool effectFinished) {
        effectFinished = false;
    }
    public void EndEffect(CardEffectHandler handler) { }
}