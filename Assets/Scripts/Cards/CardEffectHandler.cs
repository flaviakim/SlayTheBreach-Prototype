using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CardEffectHandler {
    public event EventHandler<EffectEventArgs> OnEffectStarted;
    public event EventHandler<EffectEventArgs> OnEffectFinished;
    public event EventHandler<CardEventArgs> OnCardStarted;
    public event EventHandler<CardEventArgs> OnCardFinished;

    public Battle Battle { get; }

    public CardEffectHandler(Battle battle) {
        Battle = battle;
    }

    public Card CurrentCard { get; private set; } = null!;
    public Creature CurrentCardTarget { get; private set; } = null;
    public List<Creature> OtherSelectedCreatures { get; } = new();

    public bool IsPlayingCard => CurrentCard != null;
    public bool IsPlayingEffect => CurrentEffect != null || EffectQueue.Count > 0;
    public readonly Queue<ICardEffect> EffectQueue = new();
    [CanBeNull] private ICardEffect CurrentEffect { get; set; } = null!;

    public void UpdateEffect() {
        if (Battle.CurrentTurn != Faction.Player) {
            Debug.LogError("Trying to update card effect handler, but it is not player's turn");
            return;
        }
        if (!IsPlayingEffect) return;

        if (CheckEffectManuallyEnded()) return;

        CheckTileClicked();
        UpdateCurrentEffect();
        return;

        bool CheckEffectManuallyEnded() { // Player manually ends the card effect
            if (CurrentEffect == null) return false;
            if (!Input.GetKeyDown(KeyCode.Space)) return false;
            FinishUpCurrentEffect();
            return true;
        }

        void CheckTileClicked() {
            if (CurrentEffect == null) return;
            if (!Input.GetMouseButtonUp(0)) return;
            var mouseWorldPosition = CameraController.Instance.GetMouseWorldPosition();
            if (!Battle.BattleMap.TryGetTile(mouseWorldPosition, out var tile)) return;
            CurrentEffect.OnSelectedTile(this, tile, out var effectFinished);
            if (effectFinished) {
                FinishUpCurrentEffect();
            }
        }

        void UpdateCurrentEffect() {
            if (CurrentEffect == null) return;
            CurrentEffect.UpdateEffect(this, out var finished);
            if (finished) {
                FinishUpCurrentEffect();
            }
        }
    }

    private void FinishUpCurrentEffect() {
        if (CurrentEffect == null) {
            Debug.LogWarning("Trying to finish up current effect, but there is no current effect");
            return;
        }

        Debug.Log($"Finishing effect {CurrentEffect.EffectName}");
        CurrentEffect.EndEffect(this);
        OnEffectFinished?.Invoke(this, new EffectEventArgs(CurrentEffect, CurrentCard, CurrentCardTarget, OtherSelectedCreatures));
        CurrentEffect = null;
        PlayNextEffect();
    }

    public void PlayCard(Card card, Creature cardTarget) {
        if (CurrentCard != null) {
            Debug.Log("Trying to play a card, but there is already a card playing, stopping current card. Probably intended behavior.");
            StopCurrentCard();
        }

        CurrentCardTarget = cardTarget;
        foreach (var effect in card.Effects) {
            Debug.Log($"Adding effect {effect.EffectName} from card {card.CardName}");
            EffectQueue.Enqueue(effect);
        }

        CurrentCard = card;
        OnCardStarted?.Invoke(this, new CardEventArgs(card, cardTarget));
        PlayNextEffect();
    }

    public void StopCurrentCard() {
        if (CurrentCard == null) {
            Debug.LogWarning("Trying to stop current card, but there is no current card");
            return;
        }

        Debug.Log($"Stopping current card {CurrentCard.CardName}");
        EffectQueue.Clear(); // To make sure FinishUpCurrentEffect doesn't play the next effect, but instead finishes the card
        FinishUpCurrentEffect();
    }

    private void PlayNextEffect() {
        if (EffectQueue.Count == 0) {
            FinishUpCurrentCard();
            return;
        }

        var effect = EffectQueue.Dequeue();
        CurrentEffect = effect;
        OnEffectStarted?.Invoke(this, new EffectEventArgs(effect, CurrentCard, CurrentCardTarget, OtherSelectedCreatures));
        Debug.Log($"Starting effect {effect.EffectName}");
        effect.StartEffect(this);

        return;

        void FinishUpCurrentCard() {
            Debug.Log($"Finished playing all effects");
            OnCardFinished?.Invoke(this, new CardEventArgs(CurrentCard, CurrentCardTarget));
            CurrentCardTarget = null;
            CurrentCard = null;
            OtherSelectedCreatures.Clear();
        }
    }


    internal void OnGUI() {
        GUILayout.BeginArea(new Rect(10, 50, 200, 100));
        if (CurrentEffect != null) {
            GUILayout.BeginVertical();
            GUILayout.Label(CurrentEffect.EffectName);
            GUILayout.Label(CurrentEffect.InstructionText);
            GUILayout.EndVertical();
        }
        else {
            GUILayout.Label("No effect playing");
        }

        GUILayout.EndArea();
    }
}

public class CardEventArgs : EventArgs {
    public Card Card { get; }
    public Creature Creature { get; }

    public CardEventArgs(Card card, Creature creature) {
        Card = card;
        Creature = creature;
    }
}

public class EffectEventArgs : EventArgs {
    public ICardEffect Effect { get; }
    public Card Card { get; }
    public Creature TargetCreature { get; }
    public List<Creature> OtherSelectedCreatures { get; }

    public EffectEventArgs(ICardEffect effect, Card card, Creature targetCreature, List<Creature> otherSelectedCreatures) {
        Effect = effect;
        Card = card;
        TargetCreature = targetCreature;
        OtherSelectedCreatures = otherSelectedCreatures;
    }
}