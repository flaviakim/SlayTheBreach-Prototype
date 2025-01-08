using System;
using JetBrains.Annotations;
using UnityEngine;

public class MapHoverController : MonoBehaviour {

    public event Action<HoverInfo> OnHoverTileChanged;

    private BattleMap _map = null!;

    public static HoverInfo HoverInfo;

    private void Start() {
        var battle = Battle.CurrentBattle;
        if (battle == null) {
            throw new Exception("Battle not found");
        }
        _map = battle.BattleMap;
        if (_map == null) {
            throw new Exception("BattleMap not found");
        }
    }

    private void Update() {

        var mouseWorldPosition = CameraController.Instance.GetMouseWorldPosition();
        if (!_map.TryGetTile(mouseWorldPosition, out var tile)) {
            HoverInfo = new HoverInfo();
            OnHoverTileChanged?.Invoke(HoverInfo);
            return;
        }

        if (tile == HoverInfo.Tile) {
            return;
        }

        HoverInfo = new HoverInfo {
            Tile = tile,
        };

        OnHoverTileChanged?.Invoke(HoverInfo);

    }
}

public struct HoverInfo {
    [CanBeNull] public MapTile Tile;
}