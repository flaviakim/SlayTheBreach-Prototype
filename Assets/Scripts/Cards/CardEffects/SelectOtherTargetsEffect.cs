using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectOtherTargetsEffect : ICardEffect {
    public int TargetCount { get; }
    public int RangeFromCardTarget { get; }
    public FactionRelationship Relationship { get; }

    protected readonly List<Creature> SelectedCreatures = new();
    protected int RemainingTargets => TargetCount - SelectedCreatures.Count;

    public SelectOtherTargetsEffect(int targetCount, int rangeFromCardTarget, FactionRelationship relationship) {
        TargetCount = targetCount;
        RangeFromCardTarget = rangeFromCardTarget;
        Relationship = relationship;
    }

    protected virtual bool IsTileValidTarget(CardEffectHandler handler, MapTile tile) {
        var isTileOccupied = tile.Occupant != null;
        var validTarget = isTileOccupied && Relationship switch {
            FactionRelationship.Enemy => tile.Occupant.IsPlayerControlled !=
                                         handler.CurrentCardTarget!.IsPlayerControlled,
            FactionRelationship.Ally => tile.Occupant.IsPlayerControlled ==
                                        handler.CurrentCardTarget!.IsPlayerControlled,
            FactionRelationship.All => true,
            _ => throw new ArgumentOutOfRangeException()
        };
        var inRange = BattleMap.DistanceBetweenTiles(handler.CurrentCardTarget!.CurrentTile, tile) <= RangeFromCardTarget;
        return validTarget && inRange;
    }

    protected bool TryAddTileToSelection(CardEffectHandler handler, MapTile tile) {
        if (IsTileValidTarget(handler, tile)) {
            var creature = tile.Occupant;
            Debug.Assert(creature != null, "Selected tile should have an occupant");
            Debug.Assert(handler.OtherSelectedCreatures.Contains(creature) == SelectedCreatures.Contains(creature), "Selected creatures list should match handler's list");
            if (!handler.OtherSelectedCreatures.Contains(creature)) {
                handler.OtherSelectedCreatures.Add(creature);
                SelectedCreatures.Add(creature);
                return true;
            }
        }

        return false;
    }

    public abstract string EffectName { get; }
    public abstract string InstructionText { get; }

    public void StartEffect(CardEffectHandler handler) {
        handler.OtherSelectedCreatures.Clear();
        SelectedCreatures.Clear();
        SelectionStartEffect(handler);
    }

    protected abstract void SelectionStartEffect(CardEffectHandler handler);
    public abstract void OnSelectedTile(CardEffectHandler handler, MapTile selectedTile, out bool effectFinished);
    public abstract void UpdateEffect(CardEffectHandler handler, out bool effectFinished);
    public abstract void EndEffect(CardEffectHandler handler);
}

public class PlayerSelectOtherTargetsEffect : SelectOtherTargetsEffect {
    public override string EffectName => "Target Selection";
    public override string InstructionText => $"Select {RemainingTargets} target{(RemainingTargets == 1 ? "" : "s")}";

    public PlayerSelectOtherTargetsEffect(int targetCount, int rangeFromCardTarget, FactionRelationship relationship) : base(targetCount, rangeFromCardTarget,
        relationship) { }

    protected override void SelectionStartEffect(CardEffectHandler handler) { }

    public override void OnSelectedTile(CardEffectHandler handler, MapTile selectedTile, out bool effectFinished) {
        TryAddTileToSelection(handler, selectedTile);

        Debug.Assert(SelectedCreatures.Count <= TargetCount, "Selected more targets than allowed");
        effectFinished = SelectedCreatures.Count >= TargetCount;
    }

    public override void UpdateEffect(CardEffectHandler handler, out bool effectFinished) {
        // we need to wait for the player to select the targets and end it in the OnSelectedTile method
        effectFinished = false;
    }

    public override void EndEffect(CardEffectHandler handler) {
        Debug.Assert(SelectedCreatures.Count == handler.OtherSelectedCreatures.Count, "Selected creatures list should match handler's list");
    }
}

public class AutoSelectAllOtherTargetsEffect : SelectOtherTargetsEffect {
    public override string EffectName => "Auto Target Selection";
    public override string InstructionText => null;

    public AutoSelectAllOtherTargetsEffect(int rangeFromCardTarget, FactionRelationship relationship) : base(-1, rangeFromCardTarget, relationship) { }

    protected override void SelectionStartEffect(CardEffectHandler handler) {
        foreach (var tile in Battle.CurrentBattle.BattleMap.GetTilesWhere(tile => IsTileValidTarget(handler, tile))) {
            TryAddTileToSelection(handler, tile);
        }
    }

    public override void OnSelectedTile(CardEffectHandler handler, MapTile selectedTile, out bool effectFinished) {
        // Debug.LogWarning("AutoSelectAllOtherTargetsEffect should not be waiting for player input");
        effectFinished = false;
    }

    public override void UpdateEffect(CardEffectHandler handler, out bool effectFinished) {
        effectFinished = true;
    }

    public override void EndEffect(CardEffectHandler handler) {
        Debug.Assert(SelectedCreatures.Count == handler.OtherSelectedCreatures.Count, "Selected creatures list should match handler's list");
    }
}

public enum FactionRelationship {
    Enemy,
    Ally,
    All
}