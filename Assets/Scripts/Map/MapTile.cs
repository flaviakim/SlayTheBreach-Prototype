using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using PrototypeSystem;
using UnityEngine;
using Object = UnityEngine.Object;


public class MapTile : IInstance {

    public string IDName { get; }
    public Vector2Int Position { get; private set; }
    public BattleMap Map { get; private set; }

    [CanBeNull] public Creature Occupant { get; set; } = null;
    public bool IsOccupied => Occupant != null;

    private readonly GameObject _visual;

    public MapTile(int x, int y, [NotNull] BattleMap map, [NotNull] MapTileData data, GameObject visual) {
        Position = new Vector2Int(x, y);
        Map = map;
        IDName = data.IDName;

        _visual = visual;
    }

    public void Initialize(Battle battle) {
        battle.OnBattleEnded += OnBattleEnded;
    }

    private void OnBattleEnded(object sender, EventArgs e) {
        Object.Destroy(_visual);
    }

    public class MapTileData : PrototypeData {
        public string SpritePath { get; }

        [JsonConstructor]
        public MapTileData(string idName, string spritePath) : base(idName) {
            SpritePath = spritePath;
        }
    }
}
