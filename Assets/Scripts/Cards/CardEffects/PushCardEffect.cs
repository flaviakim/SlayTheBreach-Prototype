using System.Collections.Generic;
using UnityEngine;

public enum PushIntoCreatureVersion {
    PushIntoCreatureDamage,
    ContinuePushNextCreature,
}

public class PushCardEffect : ICardEffect {
    public string EffectName => "Push";
    public string InstructionText => "Select a target to push";

    public int PushRange { get; }

    public int PushDistance { get; }

    // This will probably eventually be a global variable
    public PushIntoCreatureVersion PushIntoVersion { get; }

    // This will probably eventually be a global variable
    public int PushDamage { get; }

    public PushCardEffect(PushIntoCreatureVersion pushIntoVersion, int pushDistance, int pushRange, int pushDamage) {
        PushIntoVersion = pushIntoVersion;
        PushRange = pushRange;
        PushDistance = pushDistance;
        PushDamage = pushDamage;
    }

    public void StartEffect(CardEffectHandler handler) {

    }

    public void OnSelectedTile(CardEffectHandler handler, MapTile selectedTile, out bool effectFinished) {
        effectFinished = false;
        var currentTile = handler.CurrentCardTarget.CurrentTile;
        PushCreature(handler, selectedTile, currentTile, PushRange);
    }

    private void PushCreature(CardEffectHandler handler, MapTile creatureTile, MapTile fromTile, int distance) {
        if (creatureTile == fromTile) return;

        var distanceBetweenTiles = handler.Battle.BattleMap.GetDistanceBetweenTiles(handler.CurrentCardTarget.CurrentTile, creatureTile);
        Debug.Assert(distanceBetweenTiles >= 1, $"Distance {distanceBetweenTiles} between tiles should be at least 1, otherwise the tiles are the same");
        if (distanceBetweenTiles > distance) return;

        if (creatureTile.Occupant == null) return;

        var isStraightDirection = handler.Battle.BattleMap.GetStraightDirectionBetweenTiles(fromTile, creatureTile, out var direction);
        if (!isStraightDirection) return;

        var tiles = handler.Battle.BattleMap.GetTilesInDirection(creatureTile, direction, PushDistance, out var pushedOverEdge);
        Debug.Assert(tiles.Count == PushDistance || pushedOverEdge, "Should have gotten the correct number of tiles or pushed over the edge");
        if (pushedOverEdge) {
            // TODO pushed over the edge of the map, what to do?
            Debug.Log("Pushed over the edge of the map");
        }
        PushCreatureAlongTiles(handler, tiles, creatureTile.Occupant);
    }

    private void PushCreatureAlongTiles(CardEffectHandler handler, List<MapTile> tiles, Creature creatureToPush) {
        if (tiles.Count == 0) return;
        var nextTile = tiles[0];
        var occupant = nextTile.Occupant;
        if (occupant == null) {
            // Next tile is empty, move the creature there
            var success = creatureToPush.TryMoveTo(nextTile, false);
            Debug.Assert(success, "Creature should be able to move to the next tile");
            PushCreatureAlongTiles(handler, tiles.GetRange(1, tiles.Count - 1), creatureToPush);
        }
        else {
            // Next tile is occupied.
            if (PushIntoVersion == PushIntoCreatureVersion.ContinuePushNextCreature) {
                // The push gets transferred to the next creature
                PushCreatureAlongTiles(handler, tiles.GetRange(1, tiles.Count - 1), occupant);
            }
            else {
                // The push stops here, but the creatures might take damage
                if (PushDamage > 0) {
                    occupant.TakeDamage(PushDamage);
                    creatureToPush.TakeDamage(PushDamage);
                }
            }
        }
    }

    public void UpdateEffect(CardEffectHandler handler, out bool effectFinished) {
        effectFinished = false;
    }

    public void EndEffect(CardEffectHandler handler) {

    }
}