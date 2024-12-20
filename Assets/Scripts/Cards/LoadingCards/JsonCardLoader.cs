using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class JsonCardLoader : ICardLoader {
    private const string CARDS_DIRECTORY_NAME = "Cards";
    private readonly EffectFactory _effectFactory;

    public JsonCardLoader() {
        _effectFactory = new EffectFactory();
    }

    private Card LoadCardFromJson(string jsonFilePath) {
        var cardData = JsonConvert.DeserializeObject<CardData>(File.ReadAllText(jsonFilePath));
        var effects = cardData.Effects
            .Select(e => _effectFactory.CreateEffect(e.Type, e.Parameters))
            .ToArray();

        return new Card(cardData.CardName, cardData.Description, effects);
    }

    public IEnumerable<Card> LoadAllCards() {
        var cardsDirectoryPath = Path.Combine(Application.streamingAssetsPath, CARDS_DIRECTORY_NAME);
        if (!Directory.Exists(cardsDirectoryPath)) {
            Debug.LogError($"Directory '{cardsDirectoryPath}' does not exist.");
            return Enumerable.Empty<Card>();
        }
        var cardFiles = Directory.GetFiles(cardsDirectoryPath, "*.json", SearchOption.AllDirectories);
        return cardFiles.Select(LoadCardFromJson);
    }

    private class CardData {
        public string CardName { get; set; }
        public string Description { get; set; }
        public List<EffectData> Effects { get; set; }
    }

    private class EffectData {
        public string Type { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }
}



