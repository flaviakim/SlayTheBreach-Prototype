public class CreaturePrototypeCollection : JsonPrototypeCollection<Creature.CreatureData> {
    private const string DIRECTORY_PATH = "Prototypes/Creatures";
    public CreaturePrototypeCollection() : base(DIRECTORY_PATH) { }
}