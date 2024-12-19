using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CardEffectHandler : MonoBehaviour {

    public event Action OnCardFinished = null!;

    public CardEffectHandler Instance { get; private set; } = null!;

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
        CheckTileClicked();
        UpdateCurrentEffect();
        return;

        void CheckTileClicked() {
            if (CurrentEffect == null) return;
            if (!Input.GetMouseButtonUp(0)) return;
            var mouseWorldPosition = CameraController.Instance.GetMouseWorldPosition();
            if (!BattleMap.CurrentBattleMap.TryGetTile(mouseWorldPosition, out var tile)) return;
            CurrentEffect.OnSelectedTile(this, tile, out var effectFinished);
            if (effectFinished) {
                HasNewlyFinished();
            }
        }

        void UpdateCurrentEffect() {
            if (CurrentEffect == null) return;
            CurrentEffect.UpdateEffect(this, out var finished);
            if (finished) {
                HasNewlyFinished();
            }
        }

        void HasNewlyFinished() {
            CurrentEffect.EndEffect(this);
            CurrentEffect = null;
            PlayNextEffect();
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
            FinishUpCurrentCard();
            return;
        }

        var effect = EffectQueue.Dequeue();
        CurrentEffect = effect;
        Debug.Log($"Playing effect {effect.EffectName}");
        effect.StartEffect(this);

        return;

        void FinishUpCurrentCard() {
            Debug.Log($"Finished playing all effects");
            CurrentCardTarget = null;
            OnCardFinished?.Invoke();
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