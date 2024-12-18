using System;
using JetBrains.Annotations;
using UnityEngine;

public class MapTile : MonoBehaviour {
    public Vector2Int Position { get; private set; }
    public BattleMap Map { get; private set; } = null!;

    public TileType Type { get; private set; }
    [CanBeNull] public Creature Occupant { get; set; } = null;
    public bool IsOccupied => Occupant != null;

    [SerializeField] private MapTileVisual visual;

    public void Initialize(int x, int y, [NotNull] BattleMap map, TileType type) {
        if (Map != null) {
            throw new System.Exception("MapTile already initialized");
        }

        Position = new Vector2Int(x, y);
        Map = map;
        Type = type;
        gameObject.name = $"MapTile ({x}, {y}) {type}";
        transform.position = new Vector3(x, y, 0);
        transform.parent = map.transform;

        visual.SetTileType(type);

    }
}

public enum TileType {
    Grass,
    Water,
}