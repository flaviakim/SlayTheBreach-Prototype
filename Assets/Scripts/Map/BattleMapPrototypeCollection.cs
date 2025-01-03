using PrototypeSystem;

public class BattleMapPrototypeCollection : JsonPrototypeCollection<BattleMap.BattleMapData> {
    private const string DIRECTORY_PATH = "Prototypes/Map/BattleMaps";
    public BattleMapPrototypeCollection() : base(DIRECTORY_PATH) { }
}