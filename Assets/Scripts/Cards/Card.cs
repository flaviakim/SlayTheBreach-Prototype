using System.Collections.Generic;
using System.Linq;
using PrototypeSystem;
using UnityEngine;

public class Card : IInstance {

    protected CardData PrototypeData { get; }
    public string IDName => PrototypeData.IDName;
    public string CardName => PrototypeData.CardName;
    public string Description => PrototypeData.Description;

    public ICardEffect[] Effects { get; }

    public Card(CardData prototypeData, params ICardEffect[] effects) {
        PrototypeData = prototypeData;
        Effects = effects;
        if (effects.Length == 0) {
            Debug.LogError($"No effects added for card {prototypeData.CardName}");
            Effects = prototypeData.Effects.Select(effectData => new EffectFactory().CreateEffect(effectData.Type, effectData.Parameters)).ToArray();
        }
    }

    public class CardData : PrototypeData {
        public string CardName { get; set; }
        public string Description { get; set; }
        public List<EffectData> Effects { get; set; }

        public CardData(string idName, string cardName, string description, List<EffectData> effects) : base(idName) {
            CardName = cardName;
            Description = description;
            Effects = effects;
        }
    }

    public class EffectData {
        public string Type { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }
}