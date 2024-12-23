using System;
using JetBrains.Annotations;
using UnityEngine;

public class Battle : MonoBehaviour {

    public event EventHandler OnPlayerHasPlayedCard;

    [SerializeField] private int startHandSize = 5;

    public static Battle CurrentBattle { get; private set; } = null!;

    public BattleMap BattleMap { get; private set; } = null!;
    public CardsManager CardsManager { get; private set; } = null!;
    public CardEffectHandler CardEffectHandler { get; private set; } = null!;
    public CreaturesManager CreaturesManager { get; private set; } = null!;
    public EnemyManager EnemyManager { get; private set; } = null!;

    public Faction CurrentTurn { get; private set; } = Faction.Player;

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
        CardEffectHandler = new CardEffectHandler(this);
        CreaturesManager = FindFirstObjectByType<CreaturesManager>();
        EnemyManager = new EnemyManager(this);


        CurrentBattle = this;
    }

    private void Start() {
        CardEffectHandler.OnCardFinished += OnCardFinishedPlaying;
        CardsManager.PlayerDeck.InitializeNewGame(startHandSize);
        CurrentTurn = Faction.Player;

        EnemyManager.OnBattleStart();
    }

    private void Update() {
        if (CurrentTurn == Faction.Enemy) {
            EnemyManager.UpdateEnemies();
        } else if (CurrentTurn == Faction.Player) {
            CardEffectHandler.UpdateEffect();
        }
    }

    public void EndPlayerTurn() {
        if (CurrentTurn != Faction.Player) {
            Debug.LogWarning("Trying to end player turn, but it is not player's turn");
            return;
        }

        CurrentTurn = Faction.Enemy;
        EnemyManager.StartEnemyTurn();
    }

    public void EndEnemyTurn() {
        if (CurrentTurn != Faction.Enemy) {
            Debug.LogWarning("Trying to end enemy turn, but it is not enemy's turn");
            return;
        }

        CurrentTurn = Faction.Player;
        EnemyManager.ChooseEnemyMoves();
        CardsManager.PlayerDeck.DrawCards(startHandSize);
    }

    private void OnCardFinishedPlaying(object sender, CardEventArgs e) {
        // TODO this should eventually be called by the player, not automatically
        if (CardsManager.PlayerDeck.HandCards.Count == 0) {
            EndPlayerTurn();
        }
    }


    public bool PlayCard(int cardIndex, [NotNull] Creature creature) {
        if (CurrentTurn != Faction.Player) {
            Debug.LogWarning("Trying to play card, but it is not player's turn");
            // TODO event for this, for example to play an error sound
            return false;
        }

        if (!CardsManager.PlayerDeck.GetCardAndRemoveFromHand(cardIndex, out var playedCard)) {
            return false;
        }

        CardEffectHandler.PlayCard(playedCard, creature);
        OnPlayerHasPlayedCard?.Invoke(this, EventArgs.Empty);

        return true;
    }

    private void OnGUI() {
        GUI.Label(new Rect(10, 10, 100, 20), $"Current turn: {CurrentTurn}");
        CardEffectHandler.OnGUI();
    }
}