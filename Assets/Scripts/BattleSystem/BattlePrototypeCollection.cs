using PrototypeSystem;

public class BattlePrototypeCollection : JsonPrototypeCollection<BattleData> {
    private const string DIRECTORY_PATH = "Prototypes/Battles";

    public BattlePrototypeCollection() : base(DIRECTORY_PATH) { }
}