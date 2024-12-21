using System;
using System.Collections.Generic;
using UnityEngine;

public class CreaturesManager : MonoBehaviour {
    public event EventHandler<DeathEventArgs> OnCreatureDeathEvent = null!;

    private readonly ICreatureLoader _creatureLoader = new JsonCreatureLoader();
    public List<Creature> CreaturesInBattle { get; } = new();

    private static CreaturesManager Instance { get; set; } = null!;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            throw new System.Exception("CreaturesManager already initialized");
        }
        _creatureLoader.LoadAllCreaturePrototypes(parentTransform: transform);

        Instance = this;
    }



    public Creature SpawnCreature(string creatureID, MapTile tile) {
        var creaturePrototype = _creatureLoader.GetCreaturePrototypeById(creatureID);
        var creature = creaturePrototype.CreateInstance(tile);
        CreaturesInBattle.Add(creature);
        creature.DeathEvent += (sender, e) => {
            CreaturesInBattle.Remove(e.DeadCreature);
            OnCreatureDeathEvent?.Invoke(this, e);
        };
        return creature;
    }

}