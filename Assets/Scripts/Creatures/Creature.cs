using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class Creature : MonoBehaviour {

    [SerializeField] private string creatureName = "Creature";
    [SerializeField] private int health = 10;
    [SerializeField] private int strength = 1;
    [SerializeField] private int defense = 1;
    [SerializeField] private int rangedAttack = 1;
    [SerializeField] private int speed = 1;

    [SerializeField] private Faction faction = Faction.Player;

    private MapTile _currentTile = null!;

    public Vector2Int Position => _currentTile.Position;

    public string CreatureName => creatureName;
    public int Health => health;
    public int Strength => strength;
    public int Defense => defense;
    public int RangedAttack => rangedAttack;
    public int Speed => speed;

    public Faction Faction => faction;

    public void Initialize([NotNull] MapTile tile) {
        Debug.Assert(_currentTile == null, "Creature already initialized");
        MoveTo(tile);
        Debug.Assert(_currentTile == tile && tile.Occupant == this, "Creature not moved to the correct tile");
    }

    public bool MoveTo([NotNull] MapTile tile) {
        // if we don't have a current tile, it's just initialization we can skip steps 1 & 2, checks must be made in where we call this method
        if (_currentTile != null) {
            // 1. check if we can move to the new tile
            if (!CanMoveTo(tile)) {
                return false;
            }

            // 2. move away from the current tile
            _currentTile.Occupant = null;
        }

        // 3. move to the new tile
        _currentTile = tile;
        _currentTile.Occupant = this;
        transform.position = new Vector3(tile.Position.x, tile.Position.y, 0);

        return true;
    }

    public bool CanMoveTo(MapTile tile) {
        if (tile.IsOccupied) {
            return false;
        }

        return true;
    }
}

public enum Faction {
    Player,
    Enemy,
}
