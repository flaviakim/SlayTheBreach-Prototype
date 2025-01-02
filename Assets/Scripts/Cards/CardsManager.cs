using System;
using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour {
    public PlayerDeck PlayerDeck { get; private set; } = null!;

    private static CardsManager Instance { get; set; } = null!;

    private readonly CardFactory _cardFactory = new();

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            throw new Exception("CardsManager already initialized");
        }

        Instance = this;

        _cardFactory.PreloadPrototypes();

        PlayerDeck = new PlayerDeck();

        var prototypeNames = _cardFactory.GetPrototypeNames();

        foreach (var card in prototypeNames) {
            for (int i = 0; i < 3; i++) {
                PlayerDeck.AddCard(_cardFactory.CreateCardInstance(card));
            }
        }
    }

}


