using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardUIManager : MonoBehaviour {
    private Battle _battle;
    private PlayerDeck _playerDeck;
    private CardUI _cardUIPrefab;
    private readonly List<CardUI> _cardUIs = new();

    [SerializeField] private Vector2 cardsOriginPosition;
    [SerializeField] private Vector2 cardScreenCenterPosition;
    [SerializeField] private float maxCardsWidth = 10f;

    private MouseTracker _mouseTracker;
    private int? _draggingCardIndex = null;


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
        _mouseTracker = MouseTracker.Instance;

        _playerDeck.CardDrawnEvent += OnCardDrawn;
        _battle.PlayerHasPlayedCardEvent += OnPlayerHasPlayedCard;
        _mouseTracker.MouseDragEvent += OnMouseDragEvent;

        InitializeCardUI();
        return;

        void FixToCamera() {
            // Set the Camera as parent to make sure the card is always at the same position on the screen
            var mainCamera = Camera.main;
            Debug.Assert(mainCamera != null, "mainCamera != null");
            transform.SetParent(mainCamera.transform);
            // set the z position to 0 to make sure the card is always in front of the camera:
            transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, -1);

            // transform.localPosition = new Vector3(cardScreenCenterPosition.x, cardScreenCenterPosition.y, 0);
        }

        void InitializeCardUI() {
            Debug.Assert(_cardUIPrefab != null, "_cardUIPrefab != null");
            AddCardsToUI(_playerDeck.HandCards);
        }
    }

    private void OnMouseDragEvent(object sender, MouseDragEventArgs e) {
        if (_cardUIs.Count == 0) return;
        if (e.DragTarget is not CardUI cardUI) return;
        var indexOfDraggedCard = _cardUIs.IndexOf(cardUI);
        switch (e.DragState) {
            case MouseDragEventArgs.MouseDragState.Start:
                _draggingCardIndex = indexOfDraggedCard;
                RecalculateCardPositions();
                break;
            case MouseDragEventArgs.MouseDragState.Continuous:
                Debug.Assert(_draggingCardIndex.HasValue, "_draggingCardIndex.HasValue");
                Debug.Assert(indexOfDraggedCard == _draggingCardIndex, "indexOfDraggedCard == _draggingCardIndex");
                break;
            case MouseDragEventArgs.MouseDragState.End:
                Debug.Assert(_draggingCardIndex.HasValue, "_draggingCardIndex.HasValue");
                Debug.Assert(indexOfDraggedCard == _draggingCardIndex, "indexOfDraggedCard == _draggingCardIndex");
                _draggingCardIndex = null;
                Debug.Assert(Battle.CurrentBattle != null, "Battle.CurrentBattle != null");
                var mouseWorldPos = CameraController.Instance.GetMouseWorldPosition();
                var successfulPlay = Battle.CurrentBattle.TryPlayCard(indexOfDraggedCard, mouseWorldPos);
                if (!successfulPlay) {
                    RecalculateCardPositions(); // Move the card back to its original position
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnPlayerHasPlayedCard(object sender, CardPlayedEventArgs e) {
        Debug.Log($"CardUIManager: OnPlayerHasPlayedCard {e.CardIndex}");
        Debug.Assert(_cardUIs.Count > 0, "_cardUIs.Count > 0");
        var cardIndex = e.CardIndex;
        var cardUI = _cardUIs[cardIndex];
        DestroyCardUI(cardUI, cardIndex);
    }

    private void DestroyCardUI(CardUI cardUI, int cardIndex) {
        _cardUIs.RemoveAt(cardIndex);
        cardUI.gameObject.SetActive(false);
        Destroy(cardUI.gameObject, 0.5f); // Delayed, so stuff like OnMouseHoverExit can still be called correctly
        RecalculateCardPositions();
    }

    private void OnCardDrawn(object sender, CardDeckEventArgs e) {
        AddCardToUI(e.Card);
    }

    private void AddCardToUI(Card card) {
        var newIndex = _playerDeck.HandCards.Count - 1;
        var cardUI = CreateCardUI(card);
        _cardUIs.Add(cardUI);

        RecalculateCardPositions();
    }

    private void AddCardsToUI(List<Card> cards) {
        for (var index = 0; index < cards.Count; index++) {
            var card = cards[index];
            var cardUI = CreateCardUI(card);
            _cardUIs.Add(cardUI);
        }

        RecalculateCardPositions();
    }

    private CardUI CreateCardUI(Card card) {
        var cardUI = Instantiate(_cardUIPrefab, transform);
        cardUI.transform.localPosition = cardsOriginPosition;
        cardUI.Initialize(card);
        return cardUI;
    }

    private void RecalculateCardPositions() {
        Debug.Assert(_cardUIs.Count == _playerDeck.HandCards.Count, "_cardUIs.Count == _playerDeck.HandCards.Count");
        Debug.Assert(_cardUIs.Zip(_playerDeck.HandCards, (ui, card) => ui.Card == card).All(inSync => inSync), "CardUIs and HandCards are not in sync");

        var cardCount = _cardUIs.Count;
        var cardCountWithoutDraggedCard = _draggingCardIndex.HasValue ? cardCount - 1 : cardCount;
        var cardWidth = _cardUIPrefab.Width;
        var totalWidth = cardCountWithoutDraggedCard * cardWidth;
        var startX = cardScreenCenterPosition.x - totalWidth / 2 + cardWidth / 2;

        for (var i = 0; i < cardCount; i++) {
            if (i == _draggingCardIndex) continue;
            var cardUI = _cardUIs[i];
            var indexWithoutDraggingCard = i > _draggingCardIndex ? i - 1 : i;
            var x = startX + indexWithoutDraggingCard * cardWidth;
            var targetPosition = new Vector2(x, cardScreenCenterPosition.y);
            cardUI.SetTargetPosition(targetPosition);
            // cardUI.transform.localPosition = new Vector3(x, cardScreenCenterPosition.y, 0);
        }
    }


    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(cardScreenCenterPosition, new Vector3(maxCardsWidth, 1, 1));
        Gizmos.DrawWireSphere(cardsOriginPosition, 0.5f);
    }
}