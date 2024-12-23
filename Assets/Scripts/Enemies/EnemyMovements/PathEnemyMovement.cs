// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
//
// /// <summary>
// /// The movement that an enemy can perform.
// /// </summary>
// public class PathEnemyMovement : IEnemyMovement {
//
//     public readonly List<MapTile> Path;
//
//     public PathEnemyMovement(List<MapTile> path) {
//         Path = path;
//     }
//
//     public void UpdateMovement(Battle battle, Enemy enemy, out bool movementFinished) {
//         if (Path.Count == 0) {
//             Debug.Log($"{enemy.Creature.CreatureName} has no path to move");
//             movementFinished = true;
//             return;
//         }
//
//         // TODO for now we skip the path and just move to the last tile
//         var targetTile = Path.First();
//
//         if (!enemy.Creature.TryMoveTo(targetTile, allowTeleport: true)) { // TODO allow teleport for now
//             Debug.LogError($"Trying to move {enemy.Creature.CreatureName} along it's path to {targetTile}, but it failed.");
//             movementFinished = true;
//             return;
//         }
//
//         Path.RemoveAt(0);
//         movementFinished = Path.Count == 0;
//     }
//
// }