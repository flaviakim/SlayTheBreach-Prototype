using PrototypeSystem;

public class MapTilePrototypeCollection : JsonPrototypeCollection<MapTile.MapTileData> {
    private const string DIRECTORY_PATH = "Prototypes/Map/MapTiles";
    public MapTilePrototypeCollection() : base(DIRECTORY_PATH) { }
}