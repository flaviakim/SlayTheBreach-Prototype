using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class EnemyActionHandler {
    public event EventHandler<EnemyActionEffectEventArgs> OnEffectStarted;
    public event EventHandler<EnemyActionEffectEventArgs> OnEffectFinished;
    public event EventHandler<EnemyActionSetEventArgs> OnMoveSetStarted;
    public event EventHandler<EnemyActionSetEventArgs> OnMoveSetFinished;

    public Battle Battle { get; }

    public Creature CurrentEnemyActor { get; private set; } = null;
    public List<Creature> OtherTargets { get; } = new();

    public readonly Queue<IEnemyAction> EnemyActionQueue = new();
    [CanBeNull] public IEnemyAction CurrentAction { get; private set; } = null;
    [CanBeNull] public EnemyActionSet CurrentActionSet { get; private set; } = null;


    public EnemyActionHandler(Battle battle) {
        Battle = battle;
    }

    public void StartEnemyActionSet([NotNull] Creature mainTarget, [NotNull] EnemyActionSet enemyActionSet) {
        Debug.Log($"Playing enemy action set {enemyActionSet.ActionSetName} for {mainTarget.CreatureName}");

        if (CurrentAction != null) {
            Debug.LogWarning($"Trying to play enemy action set {enemyActionSet.ActionSetName}, but there is already an effect playing {CurrentAction.EffectName}.");
            // Force finish up the current action set
            var enemyActionSetGotFinished = false;
            while (!enemyActionSetGotFinished) {
                FinishUpCurrentAction(out enemyActionSetGotFinished);
            }
            FinishUpCurrentActionSet();
        }

        CurrentEnemyActor = mainTarget;
        OtherTargets.Clear();
        EnemyActionQueue.Clear();
        CurrentActionSet = enemyActionSet;
        foreach (var enemyAction in enemyActionSet.EnemyAction) {
            EnemyActionQueue.Enqueue(enemyAction);
        }
        OnMoveSetStarted?.Invoke(this, new EnemyActionSetEventArgs(CurrentEnemyActor, OtherTargets));
        PlayNextEnemyAction();
    }

    internal void UpdateCurrentEnemyActionSet(out bool enemyActionSetGotFinished) {
        if (CurrentAction == null) {
            Debug.LogError($"Trying to update current enemy action set, but there is no current effect {CurrentAction}, {EnemyActionQueue.Count}, {CurrentEnemyActor}");
            enemyActionSetGotFinished = false;
            return;
        }

        CurrentAction.UpdateEffect(this, out var finished);
        if (finished) {
            FinishUpCurrentAction(out enemyActionSetGotFinished);
        } else {
            enemyActionSetGotFinished = false;
        }
    }


    private void FinishUpCurrentAction(out bool enemyActionSetGotFinished) {
        if (CurrentAction == null) {
            Debug.LogWarning("Trying to finish up current effect, but there is no current effect");
            enemyActionSetGotFinished = false;
            return;
        }

        Debug.Log($"Finishing effect {CurrentAction.EffectName}");
        CurrentAction.EndEffect(this);
        OnEffectFinished?.Invoke(this, new EnemyActionEffectEventArgs(CurrentAction, CurrentEnemyActor, OtherTargets));
        CurrentAction = null;
        enemyActionSetGotFinished = !PlayNextEnemyAction();
    }

    private bool PlayNextEnemyAction() {
        if (EnemyActionQueue.Count == 0) {
            FinishUpCurrentActionSet();
            return false;
        }

        var effect = EnemyActionQueue.Dequeue();
        CurrentAction = effect;
        OnEffectStarted?.Invoke(this, new EnemyActionEffectEventArgs(effect, CurrentEnemyActor, OtherTargets));
        Debug.Log($"Starting effect {effect.EffectName}");
        effect.StartEffect(this);

        return true;
    }

    private void FinishUpCurrentActionSet() {
        Debug.Log($"Finished playing all effects");
        OnMoveSetFinished?.Invoke(this, new EnemyActionSetEventArgs(CurrentEnemyActor, OtherTargets));
        CurrentEnemyActor = null;
        CurrentActionSet = null;
        OtherTargets.Clear();
    }
}