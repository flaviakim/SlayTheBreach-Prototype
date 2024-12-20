using System;
using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour {
    private readonly List<Card> _allCards = new();
    public PlayerDeck PlayerDeck { get; private set; } = null!;

    private static CardsManager Instance { get; set; } = null!;

    private readonly ICardLoader _cardLoader = new JsonCardLoader();

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            throw new Exception("CardsManager already initialized");
        }

        Instance = this;

        LoadCards();

        PlayerDeck = new PlayerDeck();
        foreach (var card in _allCards) {
            for (int i = 0; i < 5; i++) {
                PlayerDeck.AddCard(card);
            }
        }
    }

    private void LoadCards() {
        _allCards.AddRange(_cardLoader.LoadAllCards());
    }
}


