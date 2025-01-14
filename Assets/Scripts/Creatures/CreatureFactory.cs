using JetBrains.Annotations;
using PrototypeSystem;
using UnityEngine;

public class CreatureFactory : InstanceFactory<Creature, Creature.CreatureData, CreatureFactory> {
    protected override IPrototypeCollection<Creature.CreatureData> PrototypeCollection { get; } = new CreaturePrototypeCollection();

    [CanBeNull] private GameObject _parentGameObject;
    private GameObject ParentGameObject => _parentGameObject ??= new GameObject("Creatures");

    public Creature CreateCreature(string idName, MapTile tile) {
        var prototype = TryGetPrototypeForName(idName);
        if (prototype == null) {
            Debug.LogError($"No prototype found for creature with id {idName}");
            return null;
        }

        var creature = new Creature(prototype, tile);

        CreateCreatureVisual(creature, prototype);

        return creature;
    }

    private CreatureVisual CreateCreatureVisual(Creature creature, Creature.CreatureData prototype) {
        var gameObject = new GameObject($"Creature: {creature.IDName}");
        gameObject.transform.parent = ParentGameObject.transform;
        var spriteRenderer = gameObject.gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = AssetLoader.LoadSprite(prototype.SpritePath);
        spriteRenderer.sortingLayerName = "Creature";

        var visual = gameObject.AddComponent<CreatureVisual>();
        visual.Initialize(creature);

        return visual;
    }
}
