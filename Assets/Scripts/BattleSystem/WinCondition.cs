using System.Collections.Generic;
using PrototypeSystem;
using UnityEngine;

public interface IWinCondition : IInstance {

    public string Name { get; }
    public string Description { get; }

    public bool IsWinConditionMet(Battle battle);

}

public class KillAllEnemiesWinCondition : IWinCondition {
    public string IDName { get; } = "KillAllEnemies";

    public string Name { get; } = "Kill all enemies";
    public string Description { get; } = "Win the battle by killing all enemies";

    public bool IsWinConditionMet(Battle battle) {
        Debug.Assert(battle.EnemyManager.EnemiesCount >= 0, "Trying to check win condition, but enemies count is negative");
        return battle.EnemyManager.EnemiesCount <= 0;
    }
}

public class WinConditionFactory : InstanceFactory<IWinCondition, WinConditionData, WinConditionFactory> {
    protected override IPrototypeCollection<WinConditionData> PrototypeCollection { get; } = new WinConditionPrototypeCollection();

    public IWinCondition CreateWinCondition(string winConditionId) {
        var winConditionData = PrototypeCollection.TryGetPrototypeForName(winConditionId);
        if (winConditionData == null) {
            Debug.LogError($"Win condition with id {winConditionId} not found.");
            return null;
        }

        Debug.Assert(winConditionData.IDName == winConditionId, "Win condition ID name does not match");

        return winConditionData.WinCondition;
    }
}


public class WinConditionPrototypeCollection : DebugValuesPrototypeCollection<WinConditionData> {
    protected override List<WinConditionData> GetDebugPrototypes() {
        return new List<WinConditionData> {
            new WinConditionData("KillAllEnemies", new KillAllEnemiesWinCondition())
        };
    }
}

public class WinConditionData : PrototypeData {
    public IWinCondition WinCondition { get; }

    public WinConditionData(string idName, IWinCondition winCondition) : base(idName) {
        WinCondition = winCondition;
    }
}
