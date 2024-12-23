using UnityEngine;

public class AutoSelectAllOtherTargetsCardEffect : SelectOtherTargetsCardEffect {
    public override string EffectName => "Auto Target Selection";
    public override string InstructionText => null;

    public AutoSelectAllOtherTargetsCardEffect(int rangeFromCardTarget, FactionRelationship relationship) : base(-1, rangeFromCardTarget, relationship) { }

    protected override void SelectionStartEffect(CardEffectHandler handler) {
        foreach (var tile in Battle.CurrentBattle.BattleMap.GetTilesWhere(tile => IsTileValidTarget(handler, tile))) {
            TryAddTileToSelection(handler, tile);
        }
    }

    public override void OnSelectedTile(CardEffectHandler handler, MapTile selectedTile, out bool effectFinished) {
        // Debug.LogWarning("AutoSelectAllOtherTargetsEffect should not be waiting for player input");
        effectFinished = false;
    }

    public override void UpdateEffect(CardEffectHandler handler, out bool effectFinished) {
        effectFinished = true;
    }

    public override void EndEffect(CardEffectHandler handler) {
        Debug.Assert(SelectedCreatures.Count == handler.OtherSelectedCreatures.Count, "Selected creatures list should match handler's list");
    }
}