// /// <summary>
// /// The <see cref="Card"/> equivalent for <see cref="AutonomousCreature"/>s.
// /// </summary>
// public class MoveSet {
//
//     public string MoveSetName { get; }
//     public string Description { get; }
//     public IMoveEffect[] Effects { get; }
//
//     public MoveSet(string moveSetName, string description, params IMoveEffect[] effects) {
//         MoveSetName = moveSetName;
//         Description = description;
//         Effects = effects;
//     }
//
// }
//
// public interface IMoveEffect {
//     string EffectName { get; }
//     string ExplanationText { get; }
//
//     void StartEffect(MoveEffectHandler handler);
//
//     void OnSelectedTile(MoveEffectHandler handler, MapTile selectedTile, out bool effectFinished);
//
//     void UpdateEffect(MoveEffectHandler handler, out bool effectFinished);
//
//     void EndEffect(MoveEffectHandler handler);
// }