using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck {
    private readonly List<Card> _allPlayerCards = new();

    private readonly Queue<Card> _drawPile = new();
    private readonly List<Card> _discardPile = new();
    private readonly List<Card> _hand = new();

    public List<Card> HandCards => _hand;


    public void AddCard(Card card) {
        _allPlayerCards.Add(card);
    }

    public void RemoveCard(Card card) {
        _allPlayerCards.Remove(card);
    }

    public void InitializeNewGame(int initialHandSize) {
        _drawPile.Clear();
        _discardPile.Clear();
        _hand.Clear();

        _discardPile.AddRange(_allPlayerCards);
        ShuffleDiscardPileIntoDrawPile();

        DrawCards(initialHandSize);
    }

    public void DrawCards(int count) {
        for (int i = 0; i < count; i++) {
            DrawCard();
        }
    }

    public bool GetAndPlayCard(int index, out Card playedCard) {
        if (index < 0 || index >= _hand.Count) {
            playedCard = null;
            return false;
        }

        playedCard = _hand[index];
        _hand.RemoveAt(index);
        _discardPile.Add(playedCard);

        return true;
    }

    private void DrawCard() {
        if (_drawPile.Count == 0) {
            ShuffleDiscardPileIntoDrawPile();
        }

        var card = _drawPile.Dequeue();
        _hand.Add(card);
    }

    private void ShuffleDiscardPileIntoDrawPile() {
        if (_discardPile.Count == 0) {
            return;
        }

        while (_discardPile.Count > 0) {
            var randomIndex = UnityEngine.Random.Range(0, _discardPile.Count);
            var card = _discardPile[randomIndex];
            _discardPile.RemoveAt(randomIndex);
            _drawPile.Enqueue(card);
        }
    }
}