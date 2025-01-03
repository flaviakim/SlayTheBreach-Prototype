using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using PrototypeSystem;
using UnityEngine;


public class MapTile : IInstance {

    public string IDName { get; }
    public Vector2Int Position { get; private set; }
    public BattleMap Map { get; private set; }

    [CanBeNull] public Creature Occupant { get; set; } = null;
    public bool IsOccupied => Occupant != null;

    private GameObject _visual;

    public MapTile(int x, int y, [NotNull] BattleMap map, [NotNull] MapTileData data, GameObject visual) {
        Position = new Vector2Int(x, y);
        Map = map;
        IDName = data.IDName;

        _visual = visual;
    }

    public class MapTileData : PrototypeData {
        public string SpritePath { get; }

        [JsonConstructor]
        public MapTileData(string idName, string spritePath) : base(idName) {
            SpritePath = spritePath;
        }
    }
}
