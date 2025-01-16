using PrototypeSystem;
using UnityEngine;

public class StatusEffectFactory : InstanceFactory<StatusEffect, StatusEffect.StatusEffectData, StatusEffectFactory> {
    protected override IPrototypeCollection<StatusEffect.StatusEffectData> PrototypeCollection { get; } = new StatusEffectPrototypeCollection();

    public StatusEffect CreateStatusEffect(string idName) {
        var prototype = TryGetPrototypeForName(idName);
        if (prototype == null) {
            Debug.LogError($"No prototype found for status effect with id {idName}");
            return null;
        }

        return new StatusEffect(prototype);
    }
}

public class StatusEffectPrototypeCollection : JsonPrototypeCollection<StatusEffect.StatusEffectData> {
    private const string DIRECTORY_PATH = "StatusEffects";
    public StatusEffectPrototypeCollection() : base(DIRECTORY_PATH) { }
}

public class StatusEffect : IInstance {
    public StatusEffectData PrototypeData { get; }
    public string IDName => PrototypeData.IDName;
    public string Name => PrototypeData.Name;
    public string Description => PrototypeData.Description;
    public string IconSpritePath => PrototypeData.IconSpritePath;

    public StatusEffect(StatusEffectData prototypeData) {
        PrototypeData = prototypeData;
    }

    public class StatusEffectData : PrototypeData {
        public string Name { get; }
        public string Description { get; }
        public string IconSpritePath { get; }

        public StatusEffectData(string idName, string name, string description, string iconSpritePath) : base(idName) {
            Name = name;
            Description = description;
            IconSpritePath = iconSpritePath;
        }
    }
}
