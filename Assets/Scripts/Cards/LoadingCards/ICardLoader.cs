using System.Collections.Generic;

public interface ICardLoader {
    IEnumerable<Card> LoadAllCards();
}