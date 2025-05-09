using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using PrototypeSystem;
using UnityEngine;

public class Creature : IInstance {
    public event EventHandler<DeathEventArgs> DeathEvent;
    public event EventHandler<CreatureMoveEventArgs> CreatureMovedEvent;

    public MapTile CurrentTile { get; private set; } = null!;
    public Vector2Int Position => CurrentTile.Position;

    protected CreatureData PrototypeData { get; }
    public string IDName => PrototypeData.IDName;
    public string CreatureName => PrototypeData.CreatureName;
    public int BaseHealth => PrototypeData.BaseHealth;
    public int Strength => PrototypeData.Strength;
    public int Speed => PrototypeData.Speed;

    public Faction Faction => PrototypeData.Faction;
    public bool IsPlayerControlled => Faction == Faction.Player;
    public int CurrentHealth { get; private set; }

    public Creature([NotNull] CreatureData prototypeData, [NotNull] MapTile tile) {
        PrototypeData = prototypeData;

        CurrentHealth = BaseHealth;

        TryMoveTo(tile);
        Debug.Assert(CurrentTile != null, "Creature not moved to a tile");
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
        var oldTile = CurrentTile;
        CurrentTile = tile;
        CurrentTile.Occupant = this;

        CreatureMovedEvent?.Invoke(this, new CreatureMoveEventArgs(this, oldTile, tile));

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
        Debug.Log($"{this} took {damage} damage. Remaining health: {CurrentHealth}");
        if (CurrentHealth <= 0) {
            Die();
        }
    }

    private void Die() {
        Debug.Log($"{this} dies");
        DeathEvent?.Invoke(this, new DeathEventArgs(this));
        Destroy();
    }

    public void Destroy() {
        CurrentTile.Occupant = null;
        CurrentTile = null;
    }

    public override string ToString() {
        return $"{IDName} ({CurrentHealth}/{BaseHealth})";
    }

    public class CreatureData : PrototypeData {

        public readonly string CreatureName;
        public readonly int BaseHealth;
        public readonly int Strength;
        public readonly int Defense;
        public readonly int RangedAttack;
        public readonly int Speed;
        public readonly Faction Faction;
        public readonly string SpritePath;

        [JsonConstructor]
        public CreatureData(string idName, string creatureName, int baseHealth, int strength, int defense, int rangedAttack, int speed, Faction faction, string spritePath) : base(idName) {
            CreatureName = creatureName;
            BaseHealth = baseHealth;
            Strength = strength;
            Defense = defense;
            RangedAttack = rangedAttack;
            Speed = speed;
            Faction = faction;
            SpritePath = spritePath;
        }

        public CreatureData(CreatureData other) : base(other.IDName) {
            CreatureName = other.CreatureName;
            BaseHealth = other.BaseHealth;
            Strength = other.Strength;
            Defense = other.Defense;
            RangedAttack = other.RangedAttack;
            Speed = other.Speed;
            Faction = other.Faction;
            SpritePath = other.SpritePath;
        }

        public CreatureData() : base("") {
            CreatureName = "";
            BaseHealth = 0;
            Strength = 0;
            Defense = 0;
            RangedAttack = 0;
            Speed = 0;
            Faction = Faction.Player;
            SpritePath = "";
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

public class CreatureMoveEventArgs : EventArgs {
    public Creature Creature { get; }
    public MapTile FromTile { get; }
    public MapTile ToTile { get; }

    public CreatureMoveEventArgs(Creature creature, MapTile fromTile, MapTile toTile) {
        Creature = creature;
        FromTile = fromTile;
        ToTile = toTile;
    }
}