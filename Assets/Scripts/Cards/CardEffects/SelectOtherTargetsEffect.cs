using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectOtherTargetsEffect : ICardEffect {
    public int TargetCount { get; }
    public int RangeFromCardTarget { get; }
    public FactionRelationship Relationship { get; }

    private readonly List<Creature> _selectedCreatures = new();
    private int RemainingTargets => TargetCount - _selectedCreatures.Count;

    public string EffectName => "Target Selection";
    public string InstructionText => $"Select {RemainingTargets} target{(RemainingTargets == 1 ? "" : "s")}";

    public SelectOtherTargetsEffect(int targetCount, int rangeFromCardTarget, FactionRelationship relationship) {
        TargetCount = targetCount;
        RangeFromCardTarget = rangeFromCardTarget;
        Relationship = relationship;
    }

    public void StartEffect(CardEffectHandler handler) {
        handler.OtherSelectedCreatures.Clear();
        _selectedCreatures.Clear();
    }

    public void OnSelectedTile(CardEffectHandler handler, MapTile selectedTile, out bool effectFinished) {
        var isTileOccupied = selectedTile.Occupant != null;
        var validTarget = isTileOccupied && Relationship switch {
            FactionRelationship.Enemy => selectedTile.Occupant.IsPlayerControlled !=
                                         handler.CurrentCardTarget!.IsPlayerControlled,
            FactionRelationship.Ally => selectedTile.Occupant.IsPlayerControlled ==
                                        handler.CurrentCardTarget!.IsPlayerControlled,
            FactionRelationship.All => true,
            _ => throw new ArgumentOutOfRangeException()
        };
        var inRange = BattleMap.DistanceBetweenTiles(handler.CurrentCardTarget!.CurrentTile, selectedTile) <= RangeFromCardTarget;
        if (validTarget && inRange) {
            var creature = selectedTile.Occupant;
            if (!_selectedCreatures.Contains(creature)) {
                _selectedCreatures.Add(creature);
            }
        }

        Debug.Assert(_selectedCreatures.Count <= TargetCount);
        effectFinished = _selectedCreatures.Count >= TargetCount;
    }

    public void UpdateEffect(CardEffectHandler handler, out bool effectFinished) {
        effectFinished = false;
    }

    public void EndEffect(CardEffectHandler handler) {
        foreach (var creature in _selectedCreatures) {
            handler.OtherSelectedCreatures.Add(creature);
        }
    }
}

public enum FactionRelationship {
    Enemy,
    Ally,
    All
}