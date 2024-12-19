using System;
using JetBrains.Annotations;
using UnityEngine;

public class Battle : MonoBehaviour {
    [SerializeField] private int startHandSize = 5;

    public static Battle CurrentBattle { get; private set; } = null!;

    public BattleMap BattleMap { get; private set; } = null!;
    public CardsManager CardsManager { get; private set; } = null!;
    public CardEffectHandler CardEffectHandler { get; private set; } = null!;

    // public static void StartBattle(CardsManager cardsManager, BattleMap battleMap) {
    //     CurrentBattle = new GameObject("CurrentBattle").AddComponent<Battle>();
    //     cardsManager.PlayerDeck.StartNewGame(START_HAND_SIZE);
    // }


    private void Awake() {
        // TODO this is for debugging purposes, start the battle differently

        if (CurrentBattle != null) {
            Debug.LogWarning("CurrentBattle already initialized, not yet removed.");
        }

        CardsManager = FindFirstObjectByType<CardsManager>();
        BattleMap = FindFirstObjectByType<BattleMap>();
        CardEffectHandler = FindFirstObjectByType<CardEffectHandler>();

        CurrentBattle = this;
    }

    private void Start() {
        CardsManager.PlayerDeck.StartNewGame(startHandSize);
    }


    public bool PlayCard(int cardIndex, [NotNull] Creature creature) {
        if (!CardsManager.PlayerDeck.GetAndPlayCard(cardIndex, out var playedCard)) {
            return false;
        }

        CardEffectHandler.PlayCard(playedCard, creature);

        return true;
    }

}