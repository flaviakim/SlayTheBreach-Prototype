using UnityEngine;

public class MapTileVisual : MonoBehaviour {
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetTileType(TileType type) {
        spriteRenderer.color = type switch {
            TileType.Grass => Color.green,
            TileType.Water => Color.blue,
            // TileType.Forest => new Color(0.25f, 0.5f, 0.2f),
            _ => throw new System.ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }
}