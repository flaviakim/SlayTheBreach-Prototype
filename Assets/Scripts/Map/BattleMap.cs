using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class BattleMap : MonoBehaviour {

    public static BattleMap CurrentBattleMap { get; private set; } = null!;

    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;

    [SerializeField] private MapTile mapTilePrefab;

    public int Width => width;
    public int Height => height;

    private MapTile[,] _tiles = null!;

    private void Awake() {
        CreateTiles();
        if (CurrentBattleMap != null) {
            Debug.LogWarning("CurrentBattleMap already initialized, not yet removed.");
        }
        CurrentBattleMap = this;

        return;

        void CreateTiles() {
            _tiles = new MapTile[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    var type = (TileType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(TileType)).Length);
                    _tiles[x, y] = CreateTile(x, y, type);
                }
            }
        }

        MapTile CreateTile(int x, int y, TileType type) {
            var tile = Instantiate(mapTilePrefab);
            tile.Initialize(x, y, this, type);
            return tile;
        }
    }

    public bool TryGetTile(Vector2 position, out MapTile tile) {
        return TryGetTile(new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y)), out tile);
    }

    public bool TryGetTile(Vector2Int position, out MapTile tile) {
        if (position.x < 0 || position.x >= width || position.y < 0 || position.y >= height) {
            tile = null!;
            return false;
        }

        tile = _tiles[position.x, position.y];
        return true;
    }

    public List<MapTile> GetTilesWhere(Func<MapTile, bool> predicate) {
        var result = new List<MapTile>();
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                var tile = _tiles[x, y];
                if (predicate(tile)) {
                    result.Add(tile);
                }
            }
        }

        return result;
    }

    public static int DistanceBetweenTiles(MapTile tileA, MapTile tileB) {
        // manhattan distance
        return Mathf.Abs(tileA.Position.x - tileB.Position.x) + Mathf.Abs(tileA.Position.y - tileB.Position.y);
    }

    public List<MapTile> GetTilesInRange(MapTile fromTile, int distance, Func<MapTile, bool> predicate = null) {
        var result = new List<MapTile>();
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                // If the map were ever to be bigger, we should probably use a more efficient way to find the tiles in range
                var tile = _tiles[x, y];
                if (DistanceBetweenTiles(fromTile, tile) <= distance && (predicate == null || predicate(tile))) {
                    result.Add(tile);
                }
            }
        }

        return result;
    }
}