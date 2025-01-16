using System.Collections.Generic;
using PrototypeSystem;
using UnityEngine;


public class StarterDeckFactory : InstanceFactory<PlayerDeck, StarterDeckPrototype, StarterDeckFactory> {
    protected override IPrototypeCollection<StarterDeckPrototype> PrototypeCollection { get; } = new StarterDeckPrototypeCollection();

    private readonly CardFactory _cardFactory = new();

    public PlayerDeck CreateStarterDeck(string starterDeckId) {
        var prototype = PrototypeCollection.TryGetPrototypeForName(starterDeckId);
        if (prototype == null) {
            Debug.LogError($"Starter deck prototype with id {starterDeckId} not found.");
            return null;
        }

        var cards = new List<Card>();
        foreach (var cardIdAndCount in prototype.CardIdAndCounts) {
            for (int i = 0; i < cardIdAndCount.Value; i++) {
                var card = _cardFactory.CreateCardInstance(cardIdAndCount.Key);
                if (card == null) {
                    Debug.LogError($"Card with id {cardIdAndCount.Key} not found.");
                    continue;
                }
                cards.Add(card);
            }
        }

        var playerDeck = new PlayerDeck(prototype.IDName, cards);
        return playerDeck;
    }
}

public class StarterDeckPrototypeCollection : JsonPrototypeCollection<StarterDeckPrototype> {
    private const string DIRECTORY_PATH = "Prototypes/StarterDecks";
    public StarterDeckPrototypeCollection() : base(DIRECTORY_PATH) { }
}

public class StarterDeckPrototype : PrototypeData {
    public Dictionary<string, int> CardIdAndCounts { get; }

    public StarterDeckPrototype(string idName, Dictionary<string, int> cardIdAndCounts) : base(idName) {
        CardIdAndCounts = cardIdAndCounts;
    }
}