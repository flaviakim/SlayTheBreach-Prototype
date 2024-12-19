using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectOtherTargetsEffect : ICardEffect {
    public readonly int TargetCount;
    public readonly int RangeFromCardTarget;
    public readonly FactionRelationship Relationship;

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
    }

    public void UpdateEffect(CardEffectHandler handler, out bool effectFinished) {
        if (Input.GetMouseButtonUp(0)) {
            // TODO this is a bit of a hack, we should have a better way to get the tile the mouse is over, a bit more abstraction
            var mouseWorldPosition = CameraController.Instance.GetMouseWorldPosition();
            var isTileUnderMouse = BattleMap.CurrentBattleMap.TryGetTile(mouseWorldPosition, out var tile);
            var isTileOccupied = isTileUnderMouse && tile.Occupant != null;
            var validTarget = isTileOccupied && Relationship switch {
                FactionRelationship.Enemy => tile.Occupant.IsPlayerControlled !=
                                             handler.CurrentCardTarget!.IsPlayerControlled,
                FactionRelationship.Ally => tile.Occupant.IsPlayerControlled ==
                                            handler.CurrentCardTarget!.IsPlayerControlled,
                FactionRelationship.All => true,
                _ => throw new ArgumentOutOfRangeException()
            };
            var inRange = isTileUnderMouse && BattleMap.DistanceBetweenTiles(handler.CurrentCardTarget!.CurrentTile, tile) <= RangeFromCardTarget;
            if (validTarget && inRange) {
                var creature = tile.Occupant;
                if (!_selectedCreatures.Contains(creature)) {
                    _selectedCreatures.Add(creature);
                }
            }
        }

        Debug.Assert(_selectedCreatures.Count <= TargetCount);
        effectFinished = _selectedCreatures.Count >= TargetCount;
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