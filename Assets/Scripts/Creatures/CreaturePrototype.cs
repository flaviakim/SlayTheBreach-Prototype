using UnityEngine;

public class CreaturePrototype {

    public string CreatureId { get; }
    public string CreatureName { get; }
    public int Health { get; }
    public int Strength { get; }
    public int Defense { get; }
    public int RangedAttack { get; }
    public int Speed { get; }
    public Faction Faction { get; }

    private readonly GameObject _prototypeGameObject;

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
    public CreaturePrototype(string creatureId, string creatureName, int health, int strength, int defense, int rangedAttack, int speed, Faction faction, Sprite sprite, Transform parentTransform) {
        CreatureId = creatureId;
        CreatureName = creatureName;
        Health = health;
        Strength = strength;
        Defense = defense;
        RangedAttack = rangedAttack;
        Speed = speed;
        Faction = faction;

        // TODO this could be created by a factory class.
        _prototypeGameObject = new GameObject($"PrototypeCreature: {CreatureId} ({CreatureName})");
        var spriteRenderer = _prototypeGameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingLayerName = "Creature";
        _prototypeGameObject.transform.parent = parentTransform;
        _prototypeGameObject.SetActive(false);
    }


    public Creature CreateInstance(MapTile tile) {
        var creature = new Creature(this, tile);

        return creature;
    }

    public GameObject CloneGameObject(Creature creature, MapTile tile) {
        var clonedGameObject = Object.Instantiate(_prototypeGameObject, _prototypeGameObject.transform.parent, true);
        clonedGameObject.name = $"{CreatureName} ({CreatureId})";
        clonedGameObject.SetActive(true);
        return clonedGameObject;
    }
}