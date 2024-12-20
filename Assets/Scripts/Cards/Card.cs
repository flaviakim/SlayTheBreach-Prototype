public class Card {
    public string CardName { get; }
    public string Description { get; }

    public ICardEffect[] Effects { get; }

    public Card(string cardName, string description, params ICardEffect[] effects) {
        CardName = cardName;
        Description = description;
        Effects = effects;
    }
}