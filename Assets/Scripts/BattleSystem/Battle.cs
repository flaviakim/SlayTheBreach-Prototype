using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using PrototypeSystem;
using UnityEngine;
using Object = UnityEngine.Object;

public class Battle : IInstance {
    public event EventHandler OnPlayerHasPlayedCard;
    public event EventHandler OnBattleWon;
    public event EventHandler BattleEndedEvent;

    public string IDName { get; }


    // Not sure if a Singleton is the best way to handle this
    [CanBeNull] public static Battle CurrentBattle { get; private set; } = null!;

    public BattleMap BattleMap { get; private set; }
    public CardsManager CardsManager { get; private set; }
    public CardEffectHandler CardEffectHandler { get; private set; }
    public CreaturesManager CreaturesManager { get; private set; }
    public EnemyManager EnemyManager { get; private set; }

    public Faction CurrentTurn { get; private set; }

    public IWinCondition[] WinConditions { get; }

    private readonly int _startHandSize;


    public Battle(string idName, int startHandSize, BattleMap battleMap, [NotNull] IWinCondition[] winConditions) {
        if (CurrentBattle != null) {
            Debug.LogWarning("CurrentBattle already initialized, not yet removed.");
        }

        CurrentBattle = this;
        IDName = idName;
        _startHandSize = startHandSize;

        CardsManager = new CardsManager();
        BattleMap = battleMap;
        CardEffectHandler = new CardEffectHandler(this);
        CreaturesManager = new CreaturesManager();
        EnemyManager = new EnemyManager(this);

        Debug.Assert(winConditions.Length > 0, "Win conditions are empty");
        WinConditions = winConditions;
    }

    public void Initialize() {
        BattleMap.Initialize(this);
        CreaturesManager.Initialize(this);
        CardsManager.Initialize(this);
        CardEffectHandler.Initialize(this);
        EnemyManager.Initialize(this);

        CardEffectHandler.OnCardFinished += OnCardFinishedPlaying;
        CardsManager.PlayerDeck.InitializeNewGame(_startHandSize);
        CurrentTurn = Faction.Player;
        EnemyManager.OnBattleStart();
    }

    public void Update() {
        if (CurrentTurn == Faction.Enemy) {
            EnemyManager.UpdateEnemies();
        }
        else if (CurrentTurn == Faction.Player) {
            CardEffectHandler.UpdateEffect();
        }
    }

    public void EndPlayerTurn() {
        if (CurrentTurn != Faction.Player) {
            Debug.LogWarning("Trying to end player turn, but it is not player's turn");
            return;
        }

        if (CheckForWin()) return;

        CurrentTurn = Faction.Enemy;
        EnemyManager.StartEnemyTurn();
    }

    private bool CheckForWin() {
        var isBattleWon = IsAnyWinConditionMet(out var winConditionsMet);
        if (isBattleWon) {
            HandleBattleWon(winConditionsMet);
            return true;
        }

        return false;
    }

    private void HandleBattleWon(IWinCondition[] winConditionsMet) {
        Debug.Log("Battle won");
        Debug.Assert(winConditionsMet.Length > 0, "Win conditions met, but none found");
        foreach (var winCondition in winConditionsMet) {
            Debug.Log($"Win condition met: {winCondition.Name}");
        }
        OnBattleWon?.Invoke(this, EventArgs.Empty);
        BattleEndedEvent?.Invoke(this, EventArgs.Empty);
        CurrentBattle = null!;
    }

    private bool IsAnyWinConditionMet(out IWinCondition[] winConditionsMet) {
        var winConditionsMetList = new List<IWinCondition>();
        foreach (var winCondition in WinConditions) {
            if (winCondition.IsWinConditionMet(this)) {
                Debug.Log($"Win condition {winCondition.Name} met");
                winConditionsMetList.Add(winCondition);
            }
        }

        winConditionsMet = winConditionsMetList.ToArray();
        return winConditionsMet.Length > 0;
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
        if (CheckForWin())
            return;

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
    public readonly string[] EnemyCreatureIdsToSpawn;
    public readonly string[] PlayerCreatureIdsToSpawn;
    public readonly int StartHandSize;
    public readonly string BattleMapIDName;
    public readonly string[] WinConditionIds;


    public BattleData(string idName, int startHandSize, string[] playerCreatureIdsToSpawn, string[] enemyCreatureIdsToSpawn, string battleMapIDName, string[] winConditionIds) :
        base(idName) {
        StartHandSize = startHandSize;
        PlayerCreatureIdsToSpawn = playerCreatureIdsToSpawn;
        EnemyCreatureIdsToSpawn = enemyCreatureIdsToSpawn;
        BattleMapIDName = battleMapIDName;
        WinConditionIds = winConditionIds;
    }
}