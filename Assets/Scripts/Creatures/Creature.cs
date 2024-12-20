using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public class Creature {
    public event EventHandler<DeathEventArgs> DeathEvent;


    private readonly string _creatureId;
    private readonly string _creatureName;
    private int _health;
    private int _strength;
    private int _defense;
    private int _rangedAttack;
    private int _speed;
    private Faction _faction;

    public MapTile CurrentTile { get; private set; } = null!;
    public Vector2Int Position => CurrentTile.Position;

    public string CreatureId => _creatureId;
    public string CreatureName => _creatureName;
    public int Health => _health;
    public int Strength => _strength;
    public int Defense => _defense;
    public int RangedAttack => _rangedAttack;
    public int Speed => _speed;
    public Faction Faction => _faction;
    public bool IsPlayerControlled => _faction == Faction.Player;

    private readonly GameObject _gameObject;
    public bool IsPrototype { get; }

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
    public Creature(string creatureId, string creatureName, int health, int strength, int defense, int rangedAttack, int speed, Faction faction, Sprite sprite, Transform parentTransform) {
        _creatureId = creatureId;
        _creatureName = creatureName;
        _health = health;
        _strength = strength;
        _defense = defense;
        _rangedAttack = rangedAttack;
        _speed = speed;
        _faction = faction;

        IsPrototype = true;

        _gameObject = new GameObject($"PrototypeCreature: {_creatureId} ({_creatureName})");
        var spriteRenderer = _gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingLayerName = "Creature";
        _gameObject.transform.parent = parentTransform;
        _gameObject.SetActive(false);
    }

    public static Creature ClonePrototype(Creature prototype, MapTile tile) {
        if (!prototype.IsPrototype) {
            throw new System.Exception("Can't clone a non-prototype creature");
        }

        var creature = new Creature(prototype);

        creature.TryMoveTo(tile);
        Debug.Assert(creature.CurrentTile == tile, "Creature not moved to the correct tile");
        Debug.Assert(creature.CurrentTile.Occupant == creature, "Creature not set as the occupant of the tile");

        return creature;
    }

    private Creature(Creature prototype) {
        _creatureId = prototype._creatureId;
        _creatureName = prototype._creatureName;
        _health = prototype._health;
        _strength = prototype._strength;
        _defense = prototype._defense;
        _rangedAttack = prototype._rangedAttack;
        _speed = prototype._speed;
        _faction = prototype._faction;

        IsPrototype = false;

        _gameObject = Object.Instantiate(prototype._gameObject, prototype._gameObject.transform.parent, true);
        _gameObject.name = $"{_creatureName} ({_creatureId})";
        _gameObject.SetActive(true);
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
        _health -= damage;
        Debug.Log($"{_creatureName} takes {damage} damage, now has {_health} health");
        if (_health <= 0) {
            Die();
        }
    }

    private void Die() {
        Debug.Log($"{_creatureName} dies");
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