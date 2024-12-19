using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class Card {

    public string cardName { get; }
    public string description { get; }

    public ICardEffect[] effects { get; }

    public Card(string cardName, string description, params ICardEffect[] effects) {
        this.cardName = cardName;
        this.description = description;
        this.effects = effects;
    }
}
