using UnityEngine;

public interface ICreatureLoader {
    void LoadAllCreaturePrototypes(Transform parentTransform);
    PlayerCreaturePrototype GetPlayerCreaturePrototypeById(string creatureId);
    AutonomousCreaturePrototype GetAutonomousCreaturePrototypeById(string creatureId);
}