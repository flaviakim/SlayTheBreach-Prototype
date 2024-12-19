using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CardEffectHandler : MonoBehaviour {

    public event Action<ICardEffect, Card> OnEffectStarted = null!;
    public event Action<ICardEffect, Card> OnEffectFinished = null!;
    public event Action<Card> OnCardStarted = null!;
    public event Action<Card> OnCardFinished = null!;

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
        OnEffectFinished?.Invoke(CurrentEffect, CurrentCard);
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
        OnCardStarted?.Invoke(card);
        PlayNextEffect();
    }

    private void PlayNextEffect() {
        if (EffectQueue.Count == 0) {
            FinishUpCurrentCard();
            return;
        }

        var effect = EffectQueue.Dequeue();
        CurrentEffect = effect;
        OnEffectStarted?.Invoke(effect, CurrentCard);
        Debug.Log($"Starting effect {effect.EffectName}");
        effect.StartEffect(this);

        return;

        void FinishUpCurrentCard() {
            Debug.Log($"Finished playing all effects");
            CurrentCardTarget = null;
            OnCardFinished?.Invoke(CurrentCard);
            CurrentCard = null;
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