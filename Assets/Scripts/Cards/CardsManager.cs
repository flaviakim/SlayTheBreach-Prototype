using System;
using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour {
    private readonly List<Card> _allCards = new();

    public PlayerDeck PlayerDeck { get; private set; } = null!;

    private static CardsManager Instance { get; set; } = null!;

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
        // TODO: Load cards from json files
        _allCards.Add(new Card(
            "Slash",
            "Deal 5 damage to target creature",
            new PlayerSelectOtherTargetsEffect(1, 1, FactionRelationship.Enemy),
            new DamageEffect(5)
        ));

        _allCards.Add(new Card(
            "Shoot Arrow",
            "Deal 3 damage to target creature",
            new PlayerSelectOtherTargetsEffect(1, 7, FactionRelationship.Enemy),
            new DamageEffect(3)
        ));

        _allCards.Add(new Card(
            "Move",
            "Move 4 tiles",
            new MoveCardEffect(4)
        ));

        _allCards.Add(new Card(
            "Charge",
            "Move 3 tiles and deal 3 damage",
            new MoveCardEffect(3),
            new PlayerSelectOtherTargetsEffect(1, 1, FactionRelationship.Enemy),
            new DamageEffect(3)
        ));

        _allCards.Add(new Card(
            "Axe Swing",
            "Deal 3 damage to all adjacent creatures",
            new AutoSelectAllOtherTargetsEffect(1, FactionRelationship.Enemy),
            new DamageEffect(3)
        ));
    }
}