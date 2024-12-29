using System;
using System.Collections.Generic;
using UnityEngine;

public class CreaturesManager : MonoBehaviour {
    public event EventHandler<DeathEventArgs> OnCreatureDeathEvent = null!;

    // private readonly CreatureFactory _creatureFactory = new CreatureFactory();
    public List<Creature> CreaturesInBattle { get; } = new();

    private static CreaturesManager Instance { get; set; } = null!;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            throw new System.Exception("CreaturesManager already initialized");
        }
        CreatureFactory.PreloadPrototypes();

        Instance = this;
    }

    public Creature SpawnCreature(string creatureID, MapTile tile) {
        var creature = CreatureFactory.CreateCreature(creatureID, tile, transform);
        CreaturesInBattle.Add(creature);
        creature.DeathEvent += (sender, e) => {
            CreaturesInBattle.Remove(e.DeadCreature);
            OnCreatureDeathEvent?.Invoke(this, e);
        };
        return creature;
    }

}