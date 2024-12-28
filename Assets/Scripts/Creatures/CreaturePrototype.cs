using JetBrains.Annotations;
using UnityEngine;

public class CreaturePrototype {

    public readonly Creature.CreatureData Data;
    public string IDName => Data.IDName;
    public string CreatureName => Data.CreatureName;
    public int BaseHealth => Data.BaseHealth;
    public int Strength => Data.Strength;
    public int Defense => Data.Defense;
    public int RangedAttack => Data.RangedAttack;
    public int Speed => Data.Speed;
    public Faction Faction => Data.Faction;
    public string SpritePath => Data.SpritePath;


    private readonly GameObject _prototypeGameObject;

    public CreaturePrototype([NotNull] Creature.CreatureData data, [CanBeNull] Transform parentTransform) {
        Data = new Creature.CreatureData(data);

        // TODO this could be created by a factory class.
        _prototypeGameObject = new GameObject($"PrototypeCreature: {IDName} ({CreatureName})");
        var spriteRenderer = _prototypeGameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = AssetLoader.LoadSprite(SpritePath);
        spriteRenderer.sortingLayerName = "Creature";
        _prototypeGameObject.transform.parent = parentTransform;
        _prototypeGameObject.SetActive(false);
    }


    public Creature CreateInstance([NotNull] MapTile tile) {
        var creature = new Creature(this, tile);

        return creature;
    }

    public GameObject CloneGameObject() {
        var clonedGameObject = Object.Instantiate(_prototypeGameObject, _prototypeGameObject.transform.parent, true);
        clonedGameObject.name = $"{CreatureName} ({IDName})";
        clonedGameObject.SetActive(true);
        return clonedGameObject;
    }
}