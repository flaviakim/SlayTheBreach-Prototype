using System;
using System.Collections.Generic;
using UnityEngine;

public class CreaturesManager : IBattleManager {
    public event EventHandler<DeathEventArgs> OnCreatureDeathEvent;

    // private readonly CreatureFactory _creatureFactory = new CreatureFactory();
    public List<Creature> CreaturesInBattle { get; } = new();

    private readonly CreatureFactory _creatureFactory = new();

    public CreaturesManager() {
        _creatureFactory.PreloadPrototypes();
    }

    public void Initialize(Battle battle) {
        battle.BattleEndedEvent += OnBattleEnded;
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

    private void OnBattleEnded(object sender, EventArgs e) {
        foreach (var creature in CreaturesInBattle) {
            creature.DeathEvent -= OnCreatureDeathEvent;
            creature.Destroy();
        }
        CreaturesInBattle.Clear();
    }

}