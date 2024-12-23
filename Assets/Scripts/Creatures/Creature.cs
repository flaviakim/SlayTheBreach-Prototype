using System;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

public class Creature {
    public event EventHandler<DeathEventArgs> DeathEvent;

    public MapTile CurrentTile { get; private set; } = null!;
    public Vector2Int Position => CurrentTile.Position;

    public string CreatureId { get; }
    public string CreatureName { get; }

    public int Health { get; private set; }
    public int Strength { get; }
    public int Defense { get; }
    public int RangedAttack { get; }
    public int Speed { get; }

    public Faction Faction { get; }
    public bool IsPlayerControlled => Faction == Faction.Player;

    private readonly GameObject _gameObject;

    public Creature(CreaturePrototype prototype, MapTile tile) {
        CreatureId = prototype.CreatureId;
        CreatureName = prototype.CreatureName;
        Health = prototype.Health;
        Strength = prototype.Strength;
        Defense = prototype.Defense;
        RangedAttack = prototype.RangedAttack;
        Speed = prototype.Speed;
        Faction = prototype.Faction;

        _gameObject = prototype.CloneGameObject(this, tile);


        TryMoveTo(tile);
        Debug.Assert(CurrentTile == tile, "Creature not moved to the correct tile");
        Debug.Assert(CurrentTile.Occupant == this, "Creature not set as the occupant of the tile");
    }

    public bool TryMoveTo([NotNull] MapTile tile, bool allowTeleport = false) {
        // if we don't have a current tile, it's just initialization we can skip steps 1 & 2, checks must be made in where we call this method
        if (CurrentTile != null) {
            // 1. check if we can move to the new tile
            if (!CanMoveTo(tile, allowTeleport)) {
                return false;
            }

            // 2. move away from the current tile
            CurrentTile.Occupant = null;
        }

        // 3. move to the new tile
        CurrentTile = tile;
        CurrentTile.Occupant = this;
        const float tileOffset = 0.5f;
        _gameObject.transform.position = new Vector3(tile.Position.x + tileOffset, tile.Position.y + tileOffset, 0);

        return true;
    }

    public bool CanMoveTo(MapTile tile, bool allowTeleport) {
        if (tile.IsOccupied) {
            return false;
        }

        if (!allowTeleport && Vector2Int.Distance(Position, tile.Position) > 1) {
            Debug.Log($"Trying to move too far in one step, from {Position} to {tile.Position}");
            return false;
        }

        // For debugging this makes it easier
        // if (tile.Type == TileType.Water) {
        //     Debug.Log($"Trying to move to water tile {tile.Position}");
        //     return false;
        // }

        return true;
    }

    public void TakeDamage(int damage) {
        if (damage <= 0) {
            Debug.LogError("Trying to deal non-positive damage");
            return;
        }

        Health -= damage;
        Debug.Log($"{CreatureName} takes {damage} damage, now has {Health} health");
        if (Health <= 0) {
            Die();
        }
    }

    private void Die() {
        Debug.Log($"{CreatureName} dies");
        DeathEvent?.Invoke(this, new DeathEventArgs(this));
        CurrentTile.Occupant = null;
        CurrentTile = null;
        Object.Destroy(_gameObject);
    }
}

public enum Faction {
    Player,
    Enemy,
}

public class DeathEventArgs : EventArgs {
    public Creature DeadCreature { get; }

    public DeathEventArgs(Creature deadCreature) {
        DeadCreature = deadCreature;
    }
}