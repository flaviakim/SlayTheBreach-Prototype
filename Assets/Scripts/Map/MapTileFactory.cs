using PrototypeSystem;
using UnityEngine;

public class MapTileFactory : InstanceFactory<MapTile, MapTile.MapTileData, MapTileFactory> {
    protected override IPrototypeCollection<MapTile.MapTileData> PrototypeCollection { get; } = new MapTilePrototypeCollection();

    private GameObject _parentGameObject;

    public MapTile CreateMapTile(string idName, int x, int y, BattleMap map) {
        var prototype = TryGetPrototypeForName(idName);
        if (prototype == null) {
            Debug.LogError($"No prototype found for map tile with id {idName}");
            return null;
        }

        var tile = new MapTile(x, y, map, prototype, CreateVisual(prototype.SpritePath, x, y));
        return tile;
    }

    private MapTileVisual CreateVisual(string spritePath, int x, int y) {
        if (_parentGameObject == null) {
            _parentGameObject = new GameObject("MapTiles");
        }
        var gameObject = new GameObject($"MapTile ({x}, {y})");
        gameObject.transform.position = new Vector3(x, y, 0);
        gameObject.transform.parent = _parentGameObject.transform;

        var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = AssetLoader.LoadSprite(spritePath);
        spriteRenderer.sortingLayerName = "MapTile";

        var mapTileVisual = gameObject.AddComponent<MapTileVisual>();

        var collider = gameObject.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1, 1);
        collider.isTrigger = true;

        return mapTileVisual;
    }
}