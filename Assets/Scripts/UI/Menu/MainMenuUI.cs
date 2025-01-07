using System;
using GameLoop;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour {

    private UIDocument _uiDocument;
    private BattleLoop _battleLoop;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Awake() {
        _uiDocument = GetComponent<UIDocument>();
        _battleLoop = FindFirstObjectByType<BattleLoop>() ?? Instantiate(new GameObject("NewBattleLoop")).AddComponent<BattleLoop>();
        CreateMenu();
    }

    /// <summary>
    /// Creates a menu with a selector for the player to choose a battle to start.
    /// </summary>
    private void CreateMenu() {
        var battleIDs = _battleLoop.GetAllBattlesIDs();

        var root = _uiDocument.rootVisualElement;
        root.style.flexDirection = FlexDirection.Column;
        root.style.alignItems = Align.Center;
        root.style.justifyContent = Justify.Center;
        root.style.width = 500;
        root.style.height = 500;

        var label = new Label("Choose a battle to start:");
        root.Add(label);

        var selector = new PopupField<string>("Battle", battleIDs, 0);
        root.Add(selector);

        var startButton = new Button(() => {
            _battleLoop.StartBattleInNewScene(selector.value, mainMenuSceneName);
        });
        startButton.text = "Start";
        root.Add(startButton);
    }

}