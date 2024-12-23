using System;
using System.Collections.Generic;
using System.Linq;
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

    public int GetDistanceBetweenTiles([NotNull] MapTile tileA, [NotNull] MapTile tileB) {
        // manhattan distance
        Debug.Assert(tileA.Map == this, "TileA is not on this map");
        Debug.Assert(tileB.Map == this, "TileB is not on this map");
        return Mathf.Abs(tileA.Position.x - tileB.Position.x) + Mathf.Abs(tileA.Position.y - tileB.Position.y);
    }

    public List<MapTile> GetTilesInRange(MapTile fromTile, int distance, Func<MapTile, bool> predicate = null, bool includeFromTile = false) {
        var result = new List<MapTile>();
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                // If the map were ever to be bigger, we should probably use a more efficient way to find the tiles in range
                var tile = _tiles[x, y];
                if (!includeFromTile && tile == fromTile) {
                    continue;
                }
                if (GetDistanceBetweenTiles(fromTile, tile) <= distance && (predicate == null || predicate(tile))) {
                    result.Add(tile);
                }
            }
        }

        return result;
    }

    public List<MapTile> GetPathBetweenTiles(MapTile startTile, MapTile endTile, bool stopNextToTarget, int maxMovement) {
        // TODO DEBUG for now we just return the end tile, but we should add pathfinding at some point
        MapTile actualEndTile;
        if (!stopNextToTarget) {
            actualEndTile = endTile;
        }
        else {
            var possibleEndTiles = GetTilesInRange(endTile, 1, tile => !tile.IsOccupied);
            actualEndTile = possibleEndTiles.FirstOrDefault();
        }

        if (actualEndTile == null) {
            return new List<MapTile>();
        }

        return new List<MapTile> { actualEndTile };
    }
}