using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardUIManager : MonoBehaviour {
    private Battle _battle;
    private PlayerDeck _playerDeck;
    private CardUI _cardUIPrefab;
    private readonly List<CardUI> _cardUIs = new();

    [SerializeField] private Vector2 cardScreenCenterPosition;
    [SerializeField] private float maxCardsWidth = 10f;


    private void Awake() {
        _cardUIPrefab = Resources.Load<CardUI>("Prefabs/CardUI");
    }

    private void Start() {
        FixToCamera();

        _battle = Battle.CurrentBattle;
        if (_battle == null) {
            Debug.LogError($"Battle is null {name}");
            Destroy(gameObject);
            return;
        }
        _playerDeck = _battle.CardsManager.PlayerDeck;


        _playerDeck.CardDrawnEvent += OnCardDrawn;
        _battle.PlayerHasPlayedCardEvent += OnPlayerHasPlayedCard;

        InitializeCardUI();
        return;

        void FixToCamera() {
            // Set the Camera as parent to make sure the card is always at the same position on the screen
            var mainCamera = Camera.main;
            Debug.Assert(mainCamera != null, "mainCamera != null");
            transform.SetParent(mainCamera.transform);
            // set the z position to 0 to make sure the card is always in front of the camera:
            transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0);

            // transform.localPosition = new Vector3(cardScreenCenterPosition.x, cardScreenCenterPosition.y, 0);
        }

        void InitializeCardUI() {
            Debug.Assert(_cardUIPrefab != null, "_cardUIPrefab != null");
            AddCardsToUI(_playerDeck.HandCards);
        }
    }

    private void OnPlayerHasPlayedCard(object sender, CardPlayedEventArgs e) {
        Debug.Assert(_cardUIs.Count > 0, "_cardUIs.Count > 0");
        var cardIndex = e.CardIndex;
        var cardUI = _cardUIs[cardIndex];
        _cardUIs.RemoveAt(cardIndex);
        Destroy(cardUI.gameObject);
        RecalculateCardPositions();
    }

    private void OnCardDrawn(object sender, CardDeckEventArgs e) {
        AddCardToUI(e.Card);
    }

    private void AddCardToUI(Card card) {
        var cardUI = CreateCardUI(card);
        _cardUIs.Add(cardUI);
        RecalculateCardPositions();
    }

    private void AddCardsToUI(List<Card> cards) {
        foreach (var card in cards) {
            var cardUI = CreateCardUI(card);
            _cardUIs.Add(cardUI);
        }
        RecalculateCardPositions();
    }

    private CardUI CreateCardUI(Card card) {
        var cardUI = Instantiate(_cardUIPrefab, transform);
        cardUI.Initialize(card);
        return cardUI;
    }

    private void RecalculateCardPositions() {
        Debug.Assert(_cardUIs.Count == _playerDeck.HandCards.Count, "_cardUIs.Count == _playerDeck.HandCards.Count");
        Debug.Assert(_cardUIs.Zip(_playerDeck.HandCards, (ui, card) => ui.Card == card).All(inSync => inSync), "CardUIs and HandCards are not in sync");

        var cardCount = _cardUIs.Count;
        var cardWidth = _cardUIPrefab.Width;
        var totalWidth = cardCount * cardWidth;
        var startX = cardScreenCenterPosition.x - totalWidth / 2 + cardWidth / 2;

        for (var i = 0; i < cardCount; i++) {
            var cardUI = _cardUIs[i];
            cardUI.transform.localPosition = new Vector3(startX + i * cardWidth, cardScreenCenterPosition.y, 0);
        }
    }


    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(cardScreenCenterPosition, new Vector3(maxCardsWidth, 1, 1));

    }
}