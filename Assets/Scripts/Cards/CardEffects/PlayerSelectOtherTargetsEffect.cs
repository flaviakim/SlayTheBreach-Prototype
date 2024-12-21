using UnityEngine;

public class PlayerSelectOtherTargetsEffect : SelectOtherTargetsEffect {
    public override string EffectName => "Target Selection";
    public override string InstructionText => $"Select {RemainingTargets} target{(RemainingTargets == 1 ? "" : "s")}";

    public PlayerSelectOtherTargetsEffect(int targetCount, int rangeFromCardTarget, FactionRelationship relationship) : base(targetCount, rangeFromCardTarget,
        relationship) { }

    protected override void SelectionStartEffect(CardEffectHandler handler) { }

    public override void OnSelectedTile(CardEffectHandler handler, MapTile selectedTile, out bool effectFinished) {
        TryAddTileToSelection(handler, selectedTile);

        Debug.Assert(SelectedCreatures.Count <= TargetCount, "Selected more targets than allowed");
        effectFinished = SelectedCreatures.Count >= TargetCount;
    }

    public override void UpdateEffect(CardEffectHandler handler, out bool effectFinished) {
        // we need to wait for the player to select the targets and end it in the OnSelectedTile method
        effectFinished = false;
    }

    public override void EndEffect(CardEffectHandler handler) {
        Debug.Assert(SelectedCreatures.Count == handler.OtherSelectedCreatures.Count, "Selected creatures list should match handler's list");
    }
}