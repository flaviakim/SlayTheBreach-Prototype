using System;
using JetBrains.Annotations;
using PrototypeSystem;
using UnityEngine;
using Object = UnityEngine.Object;

public class Battle : IInstance {

    public string IDName { get; }

    public event EventHandler OnPlayerHasPlayedCard;

    private readonly int _startHandSize;

    public static Battle CurrentBattle { get; private set; } = null!;

    public BattleMap BattleMap { get; private set; }
    public CardsManager CardsManager { get; private set; }
    public CardEffectHandler CardEffectHandler { get; private set; }
    public CreaturesManager CreaturesManager { get; private set; }
    public EnemyManager EnemyManager { get; private set; }

    public Faction CurrentTurn { get; private set; } = Faction.Player;


    // public static void StartBattle(CardsManager cardsManager, BattleMap battleMap) {

    //     CurrentBattle = new GameObject("CurrentBattle").AddComponent<Battle>();

    //     cardsManager.PlayerDeck.StartNewGame(START_HAND_SIZE);

    // }


    public Battle(string idName, int startHandSize) {
        if (CurrentBattle != null) { // TODO this should be removed, but it is useful for debugging
            Debug.LogWarning("CurrentBattle already initialized, not yet removed.");
        }
        CurrentBattle = this;
        IDName = idName;
        _startHandSize = startHandSize;

        CardsManager = Object.FindFirstObjectByType<CardsManager>(); // TODO change to new CardsManager()
        BattleMap = Object.FindFirstObjectByType<BattleMap>(); // TODO change to new BattleMap()
        CardEffectHandler = new CardEffectHandler(this);
        CreaturesManager = Object.FindFirstObjectByType<CreaturesManager>(); // TODO change to new CreaturesManager()
        EnemyManager = new EnemyManager(this);


        CardEffectHandler.OnCardFinished += OnCardFinishedPlaying;
        CardsManager.PlayerDeck.InitializeNewGame(_startHandSize);
        CurrentTurn = Faction.Player;
        EnemyManager.OnBattleStart();
    }

    public void Update() {
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
        CardsManager.PlayerDeck.DrawCards(_startHandSize);
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

    public void OnGUI() {
        GUI.Label(new Rect(10, 10, 100, 20), $"Current turn: {CurrentTurn}");
        CardEffectHandler.OnGUI();
    }
}

public class BattleData : PrototypeData {
    public string[] EnemyCreatureIdsToSpawn;
    public string[] PlayerCreatureIdsToSpawn;
    public int StartHandSize;

    public BattleData(string idName, int startHandSize, string[] playerCreatureIdsToSpawn, string[] enemyCreatureIdsToSpawn) : base(idName) {
        StartHandSize = startHandSize;
        PlayerCreatureIdsToSpawn = playerCreatureIdsToSpawn;
        EnemyCreatureIdsToSpawn = enemyCreatureIdsToSpawn;
    }

}
