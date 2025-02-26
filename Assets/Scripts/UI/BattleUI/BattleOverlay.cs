using System;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleOverlay : MonoBehaviour {

    private UIDocument _uiDocument;

    private void Awake() {
        if (!TryGetComponent<UIDocument>(out _uiDocument)) {
            throw new Exception("UIDocument not found");
        }
        _uiDocument.rootVisualElement.Clear();
    }

    private void Start() {
        CreateUI();
    }


    private void CreateUI() {
        var root = _uiDocument.rootVisualElement;
        root.style.flexDirection = FlexDirection.Column;
        root.style.flexGrow = 1;
        root.style.alignItems = Align.Center;
        root.style.justifyContent = Justify.Center;

        var label = new Label("Battle Overlay");
        root.Add(label);

        var endTurnButton = new Button(() => {
            if (Battle.CurrentBattle == null) {
                Debug.LogError("Battle not found, but End Turn button was clicked");
                return;
            }
            Battle.CurrentBattle.EndPlayerTurn();
        }) {
            text = "End Turn"
        };
        root.Add(endTurnButton);
    }


}