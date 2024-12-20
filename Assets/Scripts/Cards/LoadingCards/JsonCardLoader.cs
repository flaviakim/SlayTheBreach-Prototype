using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

public class JsonCardLoader : ICardLoader {
    private const string CARDS_DIRECTORY = "Assets/Resources/Cards";
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
        var cardFiles = Directory.GetFiles(CARDS_DIRECTORY, "*.json", SearchOption.AllDirectories);
        return cardFiles.Select(LoadCardFromJson);
    }
}

public class CardData {
    public string CardName { get; set; }
    public string Description { get; set; }
    public List<EffectData> Effects { get; set; }
}

public class EffectData {
    public string Type { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
}