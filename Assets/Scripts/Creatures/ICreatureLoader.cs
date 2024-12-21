using System.Collections.Generic;
using UnityEngine;

public interface ICreatureLoader {
    void LoadAllCreaturePrototypes(Transform parentTransform);
    CreaturePrototype GetCreaturePrototypeById(string creatureId);


}