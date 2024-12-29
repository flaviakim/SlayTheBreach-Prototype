using PrototypeSystem;
using UnityEngine;

public class CreatureFactory : InstanceFactory<Creature, Creature.CreatureData, CreatureFactory> {
    protected override IPrototypeCollection<Creature.CreatureData> PrototypeCollection { get; } = new CreaturePrototypeCollection();

    public static Creature CreateCreature(string idName, MapTile tile, Transform transform) {
        var prototype = TryGetPrototypeForName(idName);
        if (prototype == null) {
            Debug.LogError($"No prototype found for creature with id {idName}");
            return null;
        }

        return new Creature(prototype, tile, transform);
    }

}
