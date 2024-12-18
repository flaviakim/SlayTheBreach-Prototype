using TMPro;
using UnityEngine;

public class MapHoverVisual : MonoBehaviour {
    [SerializeField] private GameObject visual;
    [SerializeField] private TextMeshPro text;

    private void Start() {
        var mapHoverController = FindFirstObjectByType<MapHoverController>();
        mapHoverController.OnHoverTileChanged += OnHoverTileChanged;
    }

    private void OnHoverTileChanged(HoverInfo info) {

        if (info.Tile == null) {
            text.text = "";
            return;
        }

        visual.transform.position = info.Tile.Position + new Vector2(0.5f, 0.5f);
        text.text = GetInfoTextForTile(info.Tile);

    }

    private static string GetInfoTextForTile(MapTile infoTile) {
        var stringBuilder = new System.Text.StringBuilder();
        stringBuilder.AppendLine($"Tile ({infoTile.Position}) {infoTile.Type}");
        var occupant = infoTile.Occupant;
        if (occupant != null) {
            stringBuilder.AppendLine($"{occupant.CreatureName}");
            stringBuilder.AppendLine($"HP: {occupant.Health}");
            stringBuilder.AppendLine($"Strength: {occupant.Strength}");
            stringBuilder.AppendLine($"Defense: {occupant.Defense}");
            stringBuilder.AppendLine($"Ranged Attack: {occupant.RangedAttack}");
            stringBuilder.AppendLine($"Speed: {occupant.Speed}");
            stringBuilder.AppendLine($"Faction: {occupant.Faction}");
        }
        return stringBuilder.ToString();
    }
}