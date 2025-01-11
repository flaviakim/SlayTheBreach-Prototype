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

    /// <summary>
    ///     Get the direction between two tiles, if they are in a straight line.
    ///     Returns false if the tiles are the same or not in a straight line, with direction set to Up.
    /// </summary>
    /// <param name="startTile"> The from tile </param>
    /// <param name="endTile"> The to tile </param>
    /// <param name="direction"> The direction between the tiles, or Up if not in a straight line </param>
    /// <returns> Whether the tiles are in a straight line </returns>
    public bool GetStraightDirectionBetweenTiles(MapTile startTile, MapTile endTile, out StraightDirection direction) {
        var xDiff = endTile.Position.x - startTile.Position.x;
        var yDiff = endTile.Position.y - startTile.Position.y;

        if (xDiff == 0 && yDiff == 0) {
            direction = StraightDirection.Up;
            return false;
        }

        if (xDiff == 0) {
            direction = yDiff > 0 ? StraightDirection.Up : StraightDirection.Down;
            return true;
        }

        if (yDiff == 0) {
            direction = xDiff > 0 ? StraightDirection.Right : StraightDirection.Left;
            return true;
        }

        direction = StraightDirection.Up;
        return false;
    }

    /// <summary>
    ///     Get tiles in a straight line from a tile in a direction
    /// </summary>
    /// <param name="fromTile"> The tile to start from, excluded from the result </param>
    /// <param name="direction"> The direction to go in </param>
    /// <param name="distance"> The number of tiles to get </param>
    /// <param name="reachedOverEdge"> Whether the end of the map was reached, and the resulting list is shorter than the distance </param>
    /// <returns> All tiles in the direction from the start tile, excluding the start tile </returns>
    /// <exception cref="ArgumentOutOfRangeException"> If the direction enum is not a straight direction </exception>
    public List<MapTile> GetTilesInDirection(MapTile fromTile, StraightDirection direction, int distance, out bool reachedOverEdge) {
        reachedOverEdge = false;
        if (distance <= 0) return new List<MapTile>();
        var result = new List<MapTile>();
        var x = fromTile.Position.x;
        var y = fromTile.Position.y;
        for (var i = 0; i < distance; i++) {
            switch (direction) {
                case StraightDirection.Up:
                    y++;
                    break;
                case StraightDirection.Down:
                    y--;
                    break;
                case StraightDirection.Left:
                    x--;
                    break;
                case StraightDirection.Right:
                    x++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            if (x < 0 || x >= Width || y < 0 || y >= Height) {
                reachedOverEdge = true;
                break;
            }

            result.Add(_tiles[x, y]);
        }

        return result;
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

public enum StraightDirection {
    Up,
    Down,
    Left,
    Right
}