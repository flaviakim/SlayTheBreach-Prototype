using System.Linq;
using PrototypeSystem;
using UnityEngine;

public class CardFactory : InstanceFactory<Card, Card.CardData, CardFactory> {
    protected override IPrototypeCollection<Card.CardData> PrototypeCollection { get; } = new CardPrototypeCollection();
    private readonly EffectFactory _effectFactory = new EffectFactory();

    public Card CreateCardInstance(string idName) {
        var prototype = TryGetPrototypeForName(idName);
        if (prototype == null) {
            Debug.LogError($"No prototype found for card with id {idName}");
            return null;
        }

        return new Card(prototype, prototype.Effects.Select(effectData => _effectFactory.CreateEffect(effectData.Type, effectData.Parameters)).ToArray());
    }
}