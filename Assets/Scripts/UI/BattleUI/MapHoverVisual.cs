using System;
using TMPro;
using UnityEngine;

public class MapHoverVisual : MonoBehaviour {
    [SerializeField] private GameObject visual;
    private string _currentText = "";

    private void Start() {
        var mapHoverController = FindFirstObjectByType<MapHoverController>();
        mapHoverController.OnHoverTileChanged += OnHoverTileChanged;
    }

    private void OnHoverTileChanged(HoverInfo info) {

        if (info.Tile == null) {
            _currentText = "";
            return;
        }

        visual.transform.position = info.Tile.Position + new Vector2(0.5f, 0.5f);
        _currentText = GetInfoTextForTile(info.Tile);

    }

    private static string GetInfoTextForTile(MapTile infoTile) {
        var stringBuilder = new System.Text.StringBuilder();
        stringBuilder.AppendLine($"Tile ({infoTile.Position}) {infoTile.IDName}");
        var occupant = infoTile.Occupant;
        if (occupant != null) {
            stringBuilder.AppendLine($"{occupant.CreatureName}");
            stringBuilder.AppendLine($"HP: {occupant.CurrentHealth}/{occupant.BaseHealth}");
            stringBuilder.AppendLine($"Strength: {occupant.Strength}");
            stringBuilder.AppendLine($"Speed: {occupant.Speed}");
            stringBuilder.AppendLine($"Faction: {occupant.Faction}");
        }
        return stringBuilder.ToString();
    }

    private void OnGUI() {
        if (_currentText == "") {
            return;
        }

        var tileSizeInPixels = Screen.height / Battle.CurrentBattle.BattleMap.Height;
        var rect = new Rect(Input.mousePosition.x + 10, Screen.height - Input.mousePosition.y, tileSizeInPixels, tileSizeInPixels);
        GUILayout.BeginArea(rect);
        GUILayout.Label(_currentText);
        GUILayout.EndArea();
    }
}