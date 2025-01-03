using System;
using System.Collections.Generic;
using UnityEngine;

public class CreaturesManager {
    public event EventHandler<DeathEventArgs> OnCreatureDeathEvent;

    // private readonly CreatureFactory _creatureFactory = new CreatureFactory();
    public List<Creature> CreaturesInBattle { get; } = new();

    private readonly CreatureFactory _creatureFactory = new();

    public CreaturesManager() {
        _creatureFactory.PreloadPrototypes();
    }

    public Creature SpawnCreature(string creatureID, MapTile tile) {
        var creature = _creatureFactory.CreateCreature(creatureID, tile);
        CreaturesInBattle.Add(creature);
        creature.DeathEvent += (sender, e) => {
            CreaturesInBattle.Remove(e.DeadCreature);
            OnCreatureDeathEvent?.Invoke(this, e);
        };
        return creature;
    }

}