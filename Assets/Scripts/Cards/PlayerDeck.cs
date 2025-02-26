using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : IInstance {
    public event EventHandler<CardDeckEventArgs> CardDrawnEvent;
    private readonly List<Card> _allPlayerCards = new();

    private readonly Queue<Card> _drawPile = new();
    private readonly List<Card> _discardPile = new();
    private readonly List<Card> _hand = new();

    public string IDName { get; }
    public List<Card> HandCards => _hand;

    public PlayerDeck(string idName, List<Card> initialCards) {
        IDName = idName;
        _allPlayerCards.AddRange(initialCards);
    }

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

    public bool GetCardAndRemoveFromHand(int index, out Card playedCard) {
        if (index < 0 || index >= _hand.Count) {
            playedCard = null;
            return false;
        }

        playedCard = _hand[index];
        _hand.RemoveAt(index);
        _discardPile.Add(playedCard);

        return true;
    }

    public void DiscardHand(out List<Card> discardedCards) {
        discardedCards = new List<Card>(HandCards.Count);
        while (_hand.Count > 0) {
            DiscardCard(0, out var card);
            discardedCards.Add(card);
        }
    }

    public void DiscardCard(int index, out Card discardedCard) {
        if (index < 0 || index >= _hand.Count) {
            discardedCard = null;
            Debug.LogError($"Trying to discard card at index {index} (there are {_hand.Count} cards in hand)");
            return;
        }

        discardedCard = _hand[index];
        _hand.RemoveAt(index);
        _discardPile.Add(discardedCard);
    }

    private void DrawCard() {
        if (_drawPile.Count == 0) {
            ShuffleDiscardPileIntoDrawPile();
        }

        var card = _drawPile.Dequeue();
        _hand.Add(card);
        OnCardDrawnEvent(card);
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

    protected virtual void OnCardDrawnEvent(Card card) {
        CardDrawnEvent?.Invoke(this, new CardDeckEventArgs(card));
    }
}

public class CardDeckEventArgs : EventArgs {
    public Card Card { get; }

    public CardDeckEventArgs(Card card) {
        Card = card;
    }
}