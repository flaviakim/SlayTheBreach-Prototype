using System;
using System.Collections.Generic;

/// <summary>
/// The <see cref="Card"/> equivalent for autonomous <see cref="Creature"/>s.
/// </summary>
public class EnemyActionSet {

    public string ActionSetName { get; }
    public string Description { get; }
    public IEnemyAction[] EnemyAction { get; }

    public EnemyActionSet(string actionSetName, string description, params IEnemyAction[] enemyAction) {
        ActionSetName = actionSetName;
        Description = description;
        EnemyAction = enemyAction;
    }

}

public class EnemyActionSetEventArgs : EventArgs {
    public Creature MainTarget { get; }
    public List<Creature> OtherTargets { get; }

    public EnemyActionSetEventArgs(Creature mainTarget, List<Creature> otherTargets) {
        MainTarget = mainTarget;
        OtherTargets = otherTargets;
    }
}


public class EnemyActionEffectEventArgs : EventArgs {
    public IEnemyAction Action { get; }
    public Creature MainTarget { get; }
    public List<Creature> OtherTargets { get; }

    public EnemyActionEffectEventArgs(IEnemyAction action, Creature mainTarget, List<Creature> otherTargets) {
        Action = action;
        MainTarget = mainTarget;
        OtherTargets = otherTargets;
    }
}