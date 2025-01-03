using System;
using System.Collections.Generic;
using UnityEngine;

public class CardsManager : IBattleManager {
    public PlayerDeck PlayerDeck { get; private set; }

    private readonly CardFactory _cardFactory = new();

    public CardsManager() {
        _cardFactory.PreloadPrototypes();

        PlayerDeck = new PlayerDeck();

        var prototypeNames = _cardFactory.GetPrototypeNames();

        // TODO: Debug, remove
        foreach (var card in prototypeNames) {
            for (int i = 0; i < 3; i++) {
                PlayerDeck.AddCard(_cardFactory.CreateCardInstance(card));
            }
        }
    }

    public void Initialize(Battle battle) { }
}