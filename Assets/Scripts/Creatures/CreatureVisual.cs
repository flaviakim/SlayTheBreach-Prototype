using JetBrains.Annotations;
using UnityEngine;

public class CreatureVisual : MonoBehaviour {

    private Creature _creature = null!;

    public void Initialize(Creature creature) {
        _creature = creature;
        creature.DeathEvent += OnDeath;
        creature.CreatureMovedEvent += OnCreatureMoved;
        var tile = creature.CurrentTile;
        MoveCreatureVisual(tile, null);
    }

    private void OnDeath(object sender, DeathEventArgs e) {
        var creature = e.DeadCreature;
        creature.DeathEvent -= OnDeath;
        creature.CreatureMovedEvent -= OnCreatureMoved;
        Destroy(gameObject);
    }

    private void OnCreatureMoved(object sender, CreatureMoveEventArgs e) {
        Debug.Assert(e.Creature == _creature, "Received move event for a different creature");
        var fromTile = e.FromTile;
        var toTile = e.ToTile;
        MoveCreatureVisual(toTile, fromTile);
    }

    private void MoveCreatureVisual(MapTile toTile, [CanBeNull] MapTile fromTile) {
        // TODO only set target position, animate the movement
        const float tileOffset = 0;
        transform.position = new Vector3(toTile.Position.x + tileOffset, toTile.Position.y + tileOffset, 0);
        Debug.Log($"{_creature}'s visual moved from {fromTile?.Position.ToString() ?? "null"} to {toTile.Position}");
    }
}