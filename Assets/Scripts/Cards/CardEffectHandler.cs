using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CardEffectHandler : MonoBehaviour {

    public event EventHandler<EffectEventArgs> OnEffectStarted;
    public event EventHandler<EffectEventArgs> OnEffectFinished;
    public event EventHandler<CardEventArgs> OnCardStarted;
    public event EventHandler<CardEventArgs> OnCardFinished;

    public CardEffectHandler Instance { get; private set; } = null!;

    public Card CurrentCard { get; private set; } = null!;
    public Creature CurrentCardTarget { get; private set; } = null;
    public List<Creature> OtherSelectedCreatures { get; } = new();

    public bool IsPlayingEffect => CurrentEffect != null || EffectQueue.Count > 0;
    public readonly Queue<ICardEffect> EffectQueue = new();
    [CanBeNull] private ICardEffect CurrentEffect { get; set; } = null!;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            Debug.LogError("CardEffectHandler already initialized");
            return;
        }
        Instance = this;
    }

    private void Update() {
        if (CheckCardManuallyEnded()) return;

        CheckTileClicked();
        UpdateCurrentEffect();
        return;

        bool CheckCardManuallyEnded() { // Player manually ends the card effect
            if (CurrentEffect == null) return false;
            if (!Input.GetKeyDown(KeyCode.Space)) return false;
            FinishUpCurrentEffect();
            return true;
        }

        void CheckTileClicked() {
            if (CurrentEffect == null) return;
            if (!Input.GetMouseButtonUp(0)) return;
            var mouseWorldPosition = CameraController.Instance.GetMouseWorldPosition();
            if (!BattleMap.CurrentBattleMap.TryGetTile(mouseWorldPosition, out var tile)) return;
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
        if (IsPlayingEffect) {
            FinishUpCurrentEffect();
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


    private void OnGUI() {
        GUILayout.BeginArea(new Rect(10, 10, 200, 100));
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
