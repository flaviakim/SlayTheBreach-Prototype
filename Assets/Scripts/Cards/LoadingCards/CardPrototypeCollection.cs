using PrototypeSystem;

public class CardPrototypeCollection : JsonPrototypeCollection<Card.CardData> {
    private const string PROTOTYPE_PATH = "Cards";
    public CardPrototypeCollection() : base(PROTOTYPE_PATH) { }
}