using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CardEffectHandler : MonoBehaviour {

    public CardEffectHandler Instance { get; private set; } = null!;

    [CanBeNull] public Creature CurrentCardTarget { get; private set; } = null;
    public List<Creature> OtherSelectedCreatures { get; } = new();

    public bool IsPlayingEffect => CurrentEffect != null || EffectQueue.Count > 0;
    public readonly Queue<ICardEffect> EffectQueue = new();
    [CanBeNull] private ICardEffect CurrentEffect { get; set; } = null!;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            throw new System.Exception("CardEffectHandler already initialized");
        }
        Instance = this;
    }

    private void Update() {
        UpdateCurrentEffect();
        return;

        void UpdateCurrentEffect() {
            if (CurrentEffect == null) return;
            CurrentEffect.UpdateEffect(this, out var finished);
            if (finished) {
                CurrentEffect.EndEffect(this);
                CurrentEffect = null;
                PlayNextEffect();
            }
        }
    }

    public void PlayCard(Card card, Creature cardTarget) {
        CurrentCardTarget = cardTarget;
        foreach (var effect in card.effects) {
            Debug.Log($"Adding effect {effect.EffectName}");
            EffectQueue.Enqueue(effect);
        }
        PlayNextEffect();
    }

    private void PlayNextEffect() {
        if (EffectQueue.Count == 0) {
            Debug.Log($"Finished playing all effects");
            FinishUpCurrentCard();
            return;
        }

        var effect = EffectQueue.Dequeue();
        CurrentEffect = effect;
        Debug.Log($"Playing effect {effect.EffectName}");
        effect.StartEffect(this);

        return;

        void FinishUpCurrentCard() {
            CurrentCardTarget = null;
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