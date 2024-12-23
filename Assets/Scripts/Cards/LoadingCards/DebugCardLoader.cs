using System.Collections.Generic;

public class DebugCardLoader : ICardLoader {
    public IEnumerable<Card> LoadAllCards() {
        var allCards = new List<Card> {
            new Card(
                "Slash",
                "Deal 5 damage to target creature",
                new PlayerSelectOtherTargetsCardCardEffect(1, 1, FactionRelationship.Enemy),
                new DamageCardEffect(5)
            ),
            new Card(
                "Shoot Arrow",
                "Deal 3 damage to target creature",
                new PlayerSelectOtherTargetsCardCardEffect(1, 7, FactionRelationship.Enemy),
                new DamageCardEffect(3)
            ),
            new Card(
                "Move",
                "Move 4 tiles",
                new MoveCardEffect(4)
            ),
            new Card(
                "Charge",
                "Move 3 tiles and deal 3 damage",
                new MoveCardEffect(3),
                new PlayerSelectOtherTargetsCardCardEffect(1, 1, FactionRelationship.Enemy),
                new DamageCardEffect(3)
            ),
            new Card(
                "Axe Swing",
                "Deal 3 damage to all adjacent creatures",
                new AutoSelectAllOtherTargetsCardEffect(1, FactionRelationship.Enemy),
                new DamageCardEffect(3)
            )
        };

        return allCards;
    }
}