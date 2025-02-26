using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using PrototypeSystem;
using UnityEngine;
using Object = UnityEngine.Object;

public class Battle : IInstance {
    public event EventHandler<CardPlayedEventArgs> PlayerPlayedCardEvent;
    public event EventHandler<CardDiscardedEventArgs> PlayerDiscardedCardEvent;
    public event EventHandler<HandDiscardedEventArgs> PlayerDiscardedHandEvent;
    public event EventHandler<TurnEndedEventArgs> TurnEndedEvent;
    public event EventHandler BattleWonEvent;
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
    private readonly bool _discardHandAtEndOfTurn;

    public bool HasStarted { get; private set; } = false;


    public Battle(string idName, int startHandSize, BattleMap battleMap, [NotNull] IWinCondition[] winConditions, bool discardHandAtEndOfTurn) {
        if (CurrentBattle != null) {
            Debug.LogWarning("CurrentBattle already initialized, not yet removed.");
        }

        CurrentBattle = this;
        IDName = idName;
        _startHandSize = startHandSize;
        _discardHandAtEndOfTurn = discardHandAtEndOfTurn;

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
    }

    public void BattleStart() {
        if (HasStarted) {
            Debug.LogWarning("Battle already started!");
            return;
        }

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


        if (_discardHandAtEndOfTurn) {
            DiscardHand(atEndOfTurn: true);
        }

        CurrentTurn = Faction.Enemy;
        TurnEndedEvent?.Invoke(this, new TurnEndedEventArgs(Faction.Player, Faction.Enemy));
        EnemyManager.StartEnemyTurn();
    }

    public void DiscardHand(bool atEndOfTurn) {
        CardsManager.PlayerDeck.DiscardHand(out var discardedCards);
        PlayerDiscardedHandEvent?.Invoke(this, new HandDiscardedEventArgs(discardedCards, atEndOfTurn));
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
        BattleWonEvent?.Invoke(this, EventArgs.Empty);
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
        TurnEndedEvent?.Invoke(this, new TurnEndedEventArgs(Faction.Player, Faction.Enemy));
        CardsManager.PlayerDeck.DrawCards(_startHandSize);
    }

    private void OnCardFinishedPlaying(object sender, CardPlayedEffectEventArgs e) {
        if (CheckForWin())
            return;

        if (CardsManager.PlayerDeck.HandCards.Count == 0) {
            // TODO this should eventually be called by the player, not automatically
            EndPlayerTurn();
        }
    }

    public bool TryPlayCard(int cardIndex, Vector2 worldPosition) {
        if (!BattleMap.TryGetTile(worldPosition, out var mapTile)) return false;
        return TryPlayCard(cardIndex, mapTile);
    }

    public bool TryPlayCard(int cardIndex, [NotNull] MapTile mapTile) {
        var creature = mapTile.Occupant;
        if (creature == null) {
            Debug.Log("Trying to play card on empty tile");
            return false;
        }
        return TryPlayCard(cardIndex, creature);
    }

    public bool TryPlayCard(int cardIndex, [NotNull] Creature creature) {
        if (CurrentTurn != Faction.Player) {
            Debug.Log("Trying to play card, but it is not player's turn");
            // TODO event for this, for example to play an error sound
            return false;
        }
        if (!creature.IsPlayerControlled) {
            Debug.Log("Trying to play card on enemy creature");
            return false;
        }

        if (!CardsManager.PlayerDeck.GetCardAndRemoveFromHand(cardIndex, out var playedCard)) {
            return false;
        }

        CardEffectHandler.PlayCard(playedCard, creature);
        PlayerPlayedCardEvent?.Invoke(this, new CardPlayedEventArgs(playedCard, cardIndex, creature));

        return true;
    }

    public void OnGUI() {
        GUI.Label(new Rect(10, 10, 100, 20), $"Current turn: {CurrentTurn}");
        CardEffectHandler.OnGUI();
    }


}

public class CardPlayedEventArgs : EventArgs {
    public Card Card { get; }
    public int CardIndex { get; }
    public Creature PlayerCreature { get; }

    public CardPlayedEventArgs(Card card, int cardIndex, Creature playerCreature) {
        Card = card;
        CardIndex = cardIndex;
        PlayerCreature = playerCreature;
    }
}

public class CardDiscardedEventArgs : EventArgs {
    public Card Card { get; }
    public int CardIndex { get; }

    public CardDiscardedEventArgs(Card card, int cardIndex) {
        Card = card;
        CardIndex = cardIndex;
    }
}

public class HandDiscardedEventArgs : EventArgs {
    public List<Card> DiscardedCards { get; }
    public bool AtEndOfTurn { get; }

    public HandDiscardedEventArgs(List<Card> discardedCards, bool atEndOfTurn) {
        DiscardedCards = discardedCards;
        AtEndOfTurn = atEndOfTurn;
    }
}

public class TurnEndedEventArgs : EventArgs {
    public Faction EndedFaction { get; }
    public Faction NextFaction { get; }

    public TurnEndedEventArgs(Faction endedFaction, Faction nextFaction) {
        EndedFaction = endedFaction;
        NextFaction = nextFaction;
    }
}


public class BattleData : PrototypeData {
    public readonly string[] EnemyCreatureIdsToSpawn;
    public readonly string[] PlayerCreatureIdsToSpawn;
    public readonly int StartHandSize;
    public readonly string BattleMapIDName;
    public readonly string[] WinConditionIds;
    public readonly bool DiscardHandAtEndOfTurn;


    public BattleData(string idName, int startHandSize, string[] playerCreatureIdsToSpawn, string[] enemyCreatureIdsToSpawn, string battleMapIDName, string[] winConditionIds, bool discardHandAtEndOfTurn) :
        base(idName) {
        StartHandSize = startHandSize;
        PlayerCreatureIdsToSpawn = playerCreatureIdsToSpawn;
        EnemyCreatureIdsToSpawn = enemyCreatureIdsToSpawn;
        BattleMapIDName = battleMapIDName;
        WinConditionIds = winConditionIds;
        DiscardHandAtEndOfTurn = discardHandAtEndOfTurn;
    }
}