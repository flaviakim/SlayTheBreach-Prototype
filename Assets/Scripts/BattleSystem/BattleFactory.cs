using PrototypeSystem;
using UnityEngine;

public class BattleFactory : InstanceFactory<Battle, BattleData, BattleFactory> {
    protected override IPrototypeCollection<BattleData> PrototypeCollection { get; } = new BattlePrototypeCollection();

    public Battle CreateBattleInstance(string battleId) {
        var battleData = PrototypeCollection.TryGetPrototypeForName(battleId);
        if (battleData == null) {
            Debug.LogError($"Battle with id {battleId} not found.");
            return null;
        }

        var battle = new Battle(battleId, battleData.StartHandSize);
        return battle;
    }
}