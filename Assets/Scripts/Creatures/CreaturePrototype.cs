using JetBrains.Annotations;
using UnityEngine;

public class CreaturePrototype {

    public readonly Creature.CreatureData Data;
    public string CreatureId => Data.CreatureId;
    public string CreatureName => Data.CreatureName;
    public int BaseHealth => Data.BaseHealth;
    public int Strength => Data.Strength;
    public int Defense => Data.Defense;
    public int RangedAttack => Data.RangedAttack;
    public int Speed => Data.Speed;
    public Faction Faction => Data.Faction;
    public string SpritePath => Data.SpritePath;


    private readonly GameObject _prototypeGameObject;

    public CreaturePrototype(Creature.CreatureData data, [CanBeNull] Transform parentTransform) {
        Data = new Creature.CreatureData(data);

        // TODO this could be created by a factory class.
        _prototypeGameObject = new GameObject($"PrototypeCreature: {CreatureId} ({CreatureName})");
        var spriteRenderer = _prototypeGameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = AssetLoader.LoadSprite(SpritePath);
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