using System;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

public class Creature {
    public event EventHandler<DeathEventArgs> DeathEvent;

    public MapTile CurrentTile { get; private set; } = null!;
    public Vector2Int Position => CurrentTile.Position;

    private readonly CreatureData _data;
    public string CreatureId => _data.CreatureId;
    public string CreatureName => _data.CreatureName;
    public int BaseHealth => _data.BaseHealth;
    public int Strength => _data.Strength;
    public int Defense => _data.Defense;
    public int RangedAttack => _data.RangedAttack;
    public int Speed => _data.Speed;

    public Faction Faction => _data.Faction;
    public bool IsPlayerControlled => Faction == Faction.Player;
    public int CurrentHealth { get; private set; }

    private readonly GameObject _gameObject;

    public Creature(CreaturePrototype prototype, MapTile tile) {
        _data = new CreatureData(prototype.Data);

        CurrentHealth = BaseHealth;

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

        CurrentHealth -= damage;
        Debug.Log($"{CreatureName} takes {damage} damage, now has {CurrentHealth} health");
        if (CurrentHealth <= 0) {
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

    public class CreatureData {
        public string CreatureId;
        public string CreatureName;
        public int BaseHealth;
        public int Strength;
        public int Defense;
        public int RangedAttack;
        public int Speed;
        public Faction Faction;
        public string SpritePath;

        public CreatureData(string creatureId, string creatureName, int baseHealth, int strength, int defense, int rangedAttack, int speed, Faction faction, string spritePath) {
            CreatureId = creatureId;
            CreatureName = creatureName;
            BaseHealth = baseHealth;
            Strength = strength;
            Defense = defense;
            RangedAttack = rangedAttack;
            Speed = speed;
            Faction = faction;
            SpritePath = spritePath;
        }

        public CreatureData(CreatureData other) {
            CreatureId = other.CreatureId;
            CreatureName = other.CreatureName;
            BaseHealth = other.BaseHealth;
            Strength = other.Strength;
            Defense = other.Defense;
            RangedAttack = other.RangedAttack;
            Speed = other.Speed;
            Faction = other.Faction;
            SpritePath = other.SpritePath;
        }

        public CreatureData() {

        }
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