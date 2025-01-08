using System;
using System.Linq;
using GameLoop;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This creates a BattleLoop and starts a battle, when the BattleScene is loaded directly, instead of through the BattleLoop in a menu.
/// </summary>
public class BattleTestStarter : MonoBehaviour {

    [SerializeField] private string battleSelectionScene = "BattleSelectionScene";

    private void Awake() {
        if (Application.isEditor == false) {
            Debug.LogWarning("BattleTestStarter should only be used in the editor");
            Destroy(gameObject);
            return;
        }
        if (SceneManager.GetActiveScene().name != "BattleScene") {
            Debug.LogError("BattleTestStarter should only be used in the BattleScene");
            Destroy(gameObject);
            return;
        }

        var existingBattleLoop = FindFirstObjectByType<BattleLoop>();
        if (existingBattleLoop != null) {
            Debug.Log("BattleLoop already exists, this is not a test");
            Destroy(gameObject);
            return;
        }

        SceneManager.LoadScene(battleSelectionScene);
    }
}