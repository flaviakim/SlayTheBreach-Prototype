using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public interface ICreatureValues {
    public string CreatureId { get; }
    public string CreatureName { get; }
    public int Health { get; }
    public int Strength { get; }
    public int Defense { get; }
    public int RangedAttack { get; }
    public int Speed { get; }
    public Faction Faction { get; }
}

public abstract class CreaturePrototype : ICreatureValues {
    public string CreatureId { get; }
    public string CreatureName { get; }
    public int Health { get; }
    public int Strength { get; }
    public int Defense { get; }
    public int RangedAttack { get; }
    public int Speed { get; }
    public Faction Faction { get; }

    internal readonly GameObject GameObject;

    /// <summary>
    /// Constructor for creating a prototype creature
    /// </summary>
    /// <param name="creatureId"> Unique identifier for the creature </param>
    /// <param name="creatureName"> Name of the creature </param>
    /// <param name="health"> Health points of the creature </param>
    /// <param name="strength"> Strength points of the creature </param>
    /// <param name="defense"> Defense points of the creature </param>
    /// <param name="rangedAttack"> Ranged attack points of the creature </param>
    /// <param name="speed"> Speed points of the creature </param>
    /// <param name="faction"> Faction of the creature </param>
    /// <param name="sprite"> Sprite to use for the creature </param>
    /// <param name="parentTransform"> Parent transform for the creature </param>
    /// <returns> A new prototype creature </returns>
    public CreaturePrototype(string creatureId, string creatureName, int health, int strength, int defense, int rangedAttack, int speed, Faction faction, Sprite sprite,
        Transform parentTransform) {
        CreatureId = creatureId;
        CreatureName = creatureName;
        Health = health;
        Strength = strength;
        Defense = defense;
        RangedAttack = rangedAttack;
        Speed = speed;
        Faction = faction;

        GameObject = new GameObject($"PrototypeCreature: {CreatureId} ({CreatureName})");
        var spriteRenderer = GameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingLayerName = "Creature";
        GameObject.transform.parent = parentTransform;
        GameObject.SetActive(false);
    }

    public abstract Creature CreateInstance(MapTile tile);
}

public class PlayerCreaturePrototype : CreaturePrototype {
    public PlayerCreaturePrototype(string creatureId, string creatureName, int health, int strength, int defense, int rangedAttack, int speed, Faction faction, Sprite sprite,
        Transform parentTransform) : base(creatureId, creatureName, health, strength, defense, rangedAttack, speed, faction, sprite, parentTransform) {
    }

    public override Creature CreateInstance(MapTile tile) {
        return new PlayerCreature(this, tile);
    }
}

public class AutonomousCreaturePrototype : CreaturePrototype {
    public AutonomousCreaturePrototype(string creatureId, string creatureName, int health, int strength, int defense, int rangedAttack, int speed, Faction faction, Sprite sprite,
        Transform parentTransform) : base(creatureId, creatureName, health, strength, defense, rangedAttack, speed, faction, sprite, parentTransform) {
    }

    public override Creature CreateInstance(MapTile tile) {
        return new AutonomousCreature(this, tile);
    }
}

public abstract class Creature : ICreatureValues{
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

        _gameObject = Object.Instantiate(prototype.GameObject, prototype.GameObject.transform.parent, true);
        _gameObject.name = $"{CreatureName} ({CreatureId})";
        _gameObject.SetActive(true);

        TryMoveTo(tile);
        Debug.Assert(CurrentTile == tile, "Creature not moved to the correct tile");
        Debug.Assert(CurrentTile.Occupant == this, "Creature not set as the occupant of the tile");
    }

    public bool TryMoveTo([NotNull] MapTile tile) {
        // if we don't have a current tile, it's just initialization we can skip steps 1 & 2, checks must be made in where we call this method
        if (CurrentTile != null) {
            // 1. check if we can move to the new tile
            if (!CanMoveTo(tile)) {
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

    public bool CanMoveTo(MapTile tile) {
        if (tile.IsOccupied) {
            return false;
        }

        if (Vector2Int.Distance(Position, tile.Position) > 1) {
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
        Debug.Assert(damage >= 0, "Damage should be non-negative");
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

    public void Deconstruct() {
        DeathEvent = null;
        Object.Destroy(_gameObject);
    }
}

public class PlayerCreature : Creature {
    public PlayerCreature(PlayerCreaturePrototype prototype, MapTile tile) : base(prototype, tile) {
    }
}

public class AutonomousCreature : Creature {
    public AutonomousCreature(AutonomousCreaturePrototype prototype, MapTile tile) : base(prototype, tile) {
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