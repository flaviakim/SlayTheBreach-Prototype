using PrototypeSystem;
using UnityEngine;

public class BattleMapFactory : InstanceFactory<BattleMap, BattleMap.BattleMapData, BattleMapFactory> {
    protected override IPrototypeCollection<BattleMap.BattleMapData> PrototypeCollection { get; } = new BattleMapPrototypeCollection();

    private readonly MapTileFactory _mapTileFactory = new();

    public BattleMap CreateBattleMap(string idName) {
        var prototype = TryGetPrototypeForName(idName);
        if (prototype == null) {
            Debug.LogError($"No prototype found for battle map with id {idName}");
            return null;
        }

        return new BattleMap(prototype, _mapTileFactory);
    }
}