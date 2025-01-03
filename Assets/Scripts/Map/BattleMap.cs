using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using PrototypeSystem;
using UnityEngine;

public class BattleMap : IInstance {
    public string IDName { get; }

    public int Width { get; }

    public int Height { get; }

    private MapTile[,] _tiles;


    public BattleMap([NotNull] BattleMapData battleMapData, MapTileFactory mapTileFactory) {
        IDName = battleMapData.IDName;
        Width = battleMapData.Width;
        Height = battleMapData.Height;

        CreateTiles();

        return;

        void CreateTiles() {
            var allNames = mapTileFactory.GetPrototypeNames();
            _tiles = new MapTile[Width, Height];
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    var idName = allNames[UnityEngine.Random.Range(0, allNames.Count)];
                    _tiles[x, y] = mapTileFactory.CreateMapTile(idName, x, y, this);
                }
            }
        }
    }

    public bool TryGetTile(Vector2 position, out MapTile tile) {
        return TryGetTile(new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y)), out tile);
    }

    public bool TryGetTile(Vector2Int position, out MapTile tile) {
        if (position.x < 0 || position.x >= Width || position.y < 0 || position.y >= Height) {
            tile = null!;
            return false;
        }

        tile = _tiles[position.x, position.y];
        return true;
    }

    public List<MapTile> GetTilesWhere(Func<MapTile, bool> predicate) {
        var result = new List<MapTile>();
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
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
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
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


    public void Initialize(Battle battle) {
        foreach (var tile in _tiles) {
            tile.Initialize(battle);
        }
    }

    public class BattleMapData : PrototypeData {
        public int Width { get; }
        public int Height { get; }

        [JsonConstructor]
        public BattleMapData(string idName, int width, int height) : base(idName) {
            Width = width;
            Height = height;
        }
    }
}