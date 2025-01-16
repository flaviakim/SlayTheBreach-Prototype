using System;
using System.Collections.Generic;
using UnityEngine;

public class CardsManager : IBattleManager { // TODO should this be a IBattleManager, or something longer lasting? Maybe sth like a IRunManager?
    public PlayerDeck PlayerDeck { get; private set; }

    private readonly StarterDeckFactory _starterDeckFactory = new();

    public CardsManager() {
        PlayerDeck = _starterDeckFactory.CreateStarterDeck("ShoveStarterDeck");
    }

    public void Initialize(Battle battle) { }
}